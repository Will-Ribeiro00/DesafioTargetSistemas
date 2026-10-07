/* =====================================================================
   Limpa os dados de teste e popula o banco com os JSONs do desafio,
   com os vínculos de sale_item e stock_movement, e já deixa algumas
   contas vencidas e pagas para a tela de Contas a Receber mostrar
   os três estados possíveis.
   NÃO mexe em dbo.app_user (login admin preservado).

   O que foi INVENTADO (os jsons não trazem isso):
   - Cada venda ganha 1 item só, com quantity = 1 e unit_price = total_price
     da própria venda. Assim o item bate exatamente com o total já validado
     (495.69 etc. continuam corretos) e ainda cria o vínculo real com um
     produto, disparando a saída de estoque. O produto é escolhido em
     rodízio entre os 5 do catálogo.
   - due_date da conta a receber = sale_date + 30 dias (o próprio limite
     máximo que a API aplica).
   - 7 das 36 vendas têm sale_date, movement_date e due_date empurrados
     para o passado (mantendo due_date = sale_date + 30), só para existir
     conta vencida e conta paga para mostrar na tela.
   ===================================================================== */

/* =====================================================================
   1) LIMPEZA (ordem respeita as FKs: filhos antes dos pais)
   ===================================================================== */
DELETE FROM dbo.account_receivable;
DELETE FROM dbo.stock_movement;
DELETE FROM dbo.sale_item;
DELETE FROM dbo.sale;
DELETE FROM dbo.product;
DELETE FROM dbo.seller;

DBCC CHECKIDENT ('dbo.account_receivable', RESEED, 0);
DBCC CHECKIDENT ('dbo.stock_movement',     RESEED, 0);
DBCC CHECKIDENT ('dbo.sale_item',          RESEED, 0);
DBCC CHECKIDENT ('dbo.sale',               RESEED, 0);
DBCC CHECKIDENT ('dbo.product',            RESEED, 0);
DBCC CHECKIDENT ('dbo.seller',             RESEED, 0);
GO


/* =====================================================================
   2) VENDEDORES  (json "vendas" do desafio)
   ===================================================================== */
INSERT INTO dbo.seller (name) VALUES
    (N'João Silva'), (N'Maria Souza'), (N'Carlos Oliveira'), (N'Ana Lima');
GO


/* =====================================================================
   3) PRODUTOS  (json "estoque" do desafio)
   current_stock começa com o valor do json. É decrementado mais abaixo,
   conforme as vendas "consomem" produto, e reconstituído em histórico
   pela movimentação IN de "Estoque inicial".

   ATENÇÃO: o json não traz preço. Os valores abaixo são um placeholder
   — ajuste para os preços reais antes de usar a tela "Nova venda" a sério.
   ===================================================================== */
INSERT INTO dbo.product (code, description, price, current_stock) VALUES
    ('101', N'Caneta Azul',               3.50,  150),
    ('102', N'Caderno Universitário',    25.90,   75),
    ('103', N'Borracha Branca',           2.00,  200),
    ('104', N'Lápis Preto HB',            1.50,  320),
    ('105', N'Marcador de Texto Amarelo', 6.90,   90);
GO


/* =====================================================================
   4) VENDAS  (json "vendas" do desafio — comissão)
   Regra: < 100 => 0% | < 500 => 1% | >= 500 => 5%
   ===================================================================== */
;WITH vendas AS (
    SELECT Vendedor, Valor, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Seq
    FROM (VALUES
        (N'João Silva', 1200.50), (N'João Silva',  950.75), (N'João Silva', 1800.00), (N'João Silva', 1400.30),
        (N'João Silva', 1100.90), (N'João Silva', 1550.00), (N'João Silva', 1700.80), (N'João Silva',  250.30),
        (N'João Silva',  480.75), (N'João Silva',  320.40),

        (N'Maria Souza', 2100.40), (N'Maria Souza', 1350.60), (N'Maria Souza',  950.20), (N'Maria Souza', 1600.75),
        (N'Maria Souza', 1750.00), (N'Maria Souza', 1450.90), (N'Maria Souza',  400.50), (N'Maria Souza',  180.20),
        (N'Maria Souza',   90.75),

        (N'Carlos Oliveira',  800.50), (N'Carlos Oliveira', 1200.00), (N'Carlos Oliveira', 1950.30),
        (N'Carlos Oliveira', 1750.80), (N'Carlos Oliveira', 1300.60), (N'Carlos Oliveira',  300.40),
        (N'Carlos Oliveira',  500.00), (N'Carlos Oliveira',  125.75),

        (N'Ana Lima', 1000.00), (N'Ana Lima', 1100.50), (N'Ana Lima', 1250.75), (N'Ana Lima', 1400.20),
        (N'Ana Lima', 1550.90), (N'Ana Lima', 1650.00), (N'Ana Lima',   75.30), (N'Ana Lima',  420.90),
        (N'Ana Lima',  315.40)
    ) AS t (Vendedor, Valor)
)
INSERT INTO dbo.sale (seller_id, sale_date, total_price, commission_percentage, commission_amount)
SELECT  s.id,
        DATEADD(MINUTE, -v.Seq, SYSUTCDATETIME()),   -- só para as vendas terem horários distintos
        v.Valor,
        pct.Percentage,
        ROUND(v.Valor * pct.Percentage / 100, 2)
FROM vendas v
JOIN dbo.seller s ON s.name = v.Vendedor
CROSS APPLY (
    SELECT CASE WHEN v.Valor < 100 THEN 0.00
                WHEN v.Valor < 500 THEN 1.00
                ELSE 5.00 END AS Percentage
) pct;
GO


/* =====================================================================
   5) ITENS DA VENDA (vínculo inventado — ver cabeçalho do arquivo)
   1 item por venda, quantity = 1, unit_price = total_price da venda.
   Produto em rodízio entre os 5 do catálogo, pela ordem do code.
   ===================================================================== */
;WITH ranked_sales AS (
    SELECT id, total_price, ROW_NUMBER() OVER (ORDER BY id) AS rn
    FROM dbo.sale
),
product_cycle AS (
    SELECT id AS product_id, ROW_NUMBER() OVER (ORDER BY code) AS idx
    FROM dbo.product
)
INSERT INTO dbo.sale_item (sale_id, product_id, quantity, unit_price)
SELECT rs.id, pc.product_id, 1, rs.total_price
FROM ranked_sales rs
JOIN product_cycle pc ON pc.idx = ((rs.rn - 1) % 5) + 1;
GO


/* =====================================================================
   6) SAÍDA DE ESTOQUE de cada item (o que uma venda real dispara)
   stock_balance é calculado com soma corrida por produto, partindo do
   current_stock atual de dbo.product (ainda igual ao valor do json,
   porque a tabela só será atualizada no passo 8).
   ===================================================================== */
;WITH item_rank AS (
    SELECT si.id AS sale_item_id, si.sale_id, si.product_id, si.quantity, s.sale_date,
           ROW_NUMBER() OVER (PARTITION BY si.product_id ORDER BY si.sale_id) AS rn
    FROM dbo.sale_item si
    JOIN dbo.sale s ON s.id = si.sale_id
)
INSERT INTO dbo.stock_movement (product_id, sale_id, type, description, quantity, stock_balance, movement_date)
SELECT  ir.product_id,
        ir.sale_id,
        'OUT',
        N'Sale registered',
        ir.quantity,
        p.current_stock - SUM(ir.quantity) OVER (PARTITION BY ir.product_id ORDER BY ir.rn ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW),
        ir.sale_date
FROM item_rank ir
JOIN dbo.product p ON p.id = ir.product_id;
GO


/* =====================================================================
   7) ENTRADA INICIAL DE ESTOQUE (histórico; type = 'IN', sale_id nulo)
   Datada 60 dias atrás: precisa ficar antes de QUALQUER venda, inclusive
   as que o passo 10 vai empurrar para até 55 dias no passado.
   ===================================================================== */
INSERT INTO dbo.stock_movement (product_id, sale_id, type, description, quantity, stock_balance, movement_date)
SELECT id, NULL, 'IN', N'Estoque inicial', current_stock, current_stock, DATEADD(DAY, -60, SYSUTCDATETIME())
FROM dbo.product;
GO


/* =====================================================================
   8) ATUALIZA O SALDO ATUAL DO PRODUTO (reflete as saídas do passo 6)
   ===================================================================== */
;WITH totals AS (
    SELECT product_id, SUM(quantity) AS qty_out
    FROM dbo.stock_movement
    WHERE type = 'OUT'
    GROUP BY product_id
)
UPDATE p
SET p.current_stock = p.current_stock - ISNULL(t.qty_out, 0)
FROM dbo.product p
LEFT JOIN totals t ON t.product_id = p.id;
GO


/* =====================================================================
   9) CONTA A RECEBER DE CADA VENDA
   due_date = sale_date + 30 dias (limite máximo da regra de negócio).
   ===================================================================== */
INSERT INTO dbo.account_receivable (sale_id, amount, due_date)
SELECT id, total_price, CAST(DATEADD(DAY, 30, sale_date) AS DATE)
FROM dbo.sale;
GO


/* =====================================================================
   10) AJUSTE DE DEMONSTRAÇÃO: vencidas, pagas e em aberto
   Move sale_date, o movement_date da saída de estoque e o due_date da
   conta a receber JUNTOS (mantendo due_date = sale_date + 30), para a
   venda aparecer no mês certo em "Vendas" e a conta aparecer coerente
   em "Contas a Receber". As outras 29 continuam como vendas de hoje,
   com vencimento em aberto no futuro.
   ===================================================================== */
DECLARE @sale_id INT, @new_sale_date DATETIME2(0);

-- ---- VENCIDAS: payment_date continua nulo -----------------------------
SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 1;
SET @new_sale_date = DATEADD(DAY, -31, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable SET due_date = CAST(DATEADD(DAY, -1, SYSUTCDATETIME()) AS DATE) WHERE id = 1;  -- 1 dia de atraso

SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 2;
SET @new_sale_date = DATEADD(DAY, -34, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable SET due_date = CAST(DATEADD(DAY, -4, SYSUTCDATETIME()) AS DATE) WHERE id = 2;  -- 4 dias

SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 3;
SET @new_sale_date = DATEADD(DAY, -40, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable SET due_date = CAST(DATEADD(DAY, -10, SYSUTCDATETIME()) AS DATE) WHERE id = 3; -- 10 dias

SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 4;
SET @new_sale_date = DATEADD(DAY, -55, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable SET due_date = CAST(DATEADD(DAY, -25, SYSUTCDATETIME()) AS DATE) WHERE id = 4; -- 25 dias

-- ---- PAGAS: payment_date preenchido ------------------------------------
SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 5;
SET @new_sale_date = DATEADD(DAY, -35, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable
SET due_date = CAST(DATEADD(DAY, -5, SYSUTCDATETIME()) AS DATE),
    payment_date = CAST(DATEADD(DAY, -6, SYSUTCDATETIME()) AS DATE)   -- pagou 1 dia antes do vencimento
WHERE id = 5;

SELECT @sale_id = sale_id FROM dbo.account_receivable WHERE id = 6;
SET @new_sale_date = DATEADD(DAY, -38, SYSUTCDATETIME());
UPDATE dbo.sale SET sale_date = @new_sale_date WHERE id = @sale_id;
UPDATE dbo.stock_movement SET movement_date = @new_sale_date WHERE sale_id = @sale_id AND type = 'OUT';
UPDATE dbo.account_receivable
SET due_date = CAST(DATEADD(DAY, -8, SYSUTCDATETIME()) AS DATE),
    payment_date = CAST(DATEADD(DAY, -2, SYSUTCDATETIME()) AS DATE)   -- pagou 6 dias atrasado
WHERE id = 6;

-- id 7: sale_date e due_date ficam como vieram do passo 9 (venda de hoje,
-- vence daqui a 30 dias) — só ganha o pagamento, mostrando uma conta paga
-- bem antes do vencimento.
UPDATE dbo.account_receivable
SET payment_date = CAST(SYSUTCDATETIME() AS DATE)
WHERE id = 7;
GO


/* =====================================================================
   11) CONFERÊNCIA
   Comissão esperada por vendedor:
     João Silva 495.69 | Maria Souza 465.96 | Carlos Oliveira 379.38 | Ana Lima 404.99
   ===================================================================== */
SELECT  s.name, COUNT(*) AS QtdVendas, SUM(sa.total_price) AS TotalVendido, SUM(sa.commission_amount) AS TotalComissao
FROM dbo.sale sa
JOIN dbo.seller s ON s.id = sa.seller_id
GROUP BY s.name
ORDER BY s.name;

-- Cada venda tem exatamente 1 item, e o item bate com o total da venda
SELECT COUNT(*) AS QtdVendas, COUNT(DISTINCT sa.id) AS QtdComItem,
       SUM(CASE WHEN sa.total_price <> si.quantity * si.unit_price THEN 1 ELSE 0 END) AS Divergencias
FROM dbo.sale sa
JOIN dbo.sale_item si ON si.sale_id = sa.id;
-- Esperado: QtdVendas = QtdComItem = 36, Divergencias = 0

SELECT code, description, price, current_stock FROM dbo.product ORDER BY code;
-- Esperado: 101->142, 102->68, 103->193, 104->313, 105->83

-- Situação de cada conta a receber (as 7 ajustadas + as 29 que seguem em aberto no futuro)
SELECT
    ar.id,
    ar.sale_id,
    s.sale_date,
    ar.amount,
    ar.due_date,
    ar.payment_date,
    CASE WHEN ar.payment_date IS NOT NULL THEN N'Paga'
         WHEN ar.due_date < CAST(SYSUTCDATETIME() AS DATE) THEN N'Vencida'
         ELSE N'Em aberto' END AS Situacao,
    CASE WHEN ar.payment_date IS NULL
         THEN DATEDIFF(DAY, ar.due_date, CAST(SYSUTCDATETIME() AS DATE))
         ELSE 0 END AS DiasAtraso,
    CASE WHEN ar.payment_date IS NULL AND ar.due_date < CAST(SYSUTCDATETIME() AS DATE)
         THEN ROUND(ar.amount * 0.025 * DATEDIFF(DAY, ar.due_date, CAST(SYSUTCDATETIME() AS DATE)), 2)
         ELSE 0 END AS Juros
FROM dbo.account_receivable ar
JOIN dbo.sale s ON s.id = ar.sale_id
ORDER BY ar.id;

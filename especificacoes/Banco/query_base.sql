/* =====================================================================
   query_base - Azure SQL Database (T-SQL)
   Estrutura ATUAL do banco (reflete o schema_azure_sql.sql + a migração
   de ENTRADA/SAIDA para IN/OUT). Somente estrutura, sem dados.
   ===================================================================== */

/* ---------- RESET (descomente para recriar do zero) ------------------
DROP TABLE IF EXISTS dbo.account_receivable;
DROP TABLE IF EXISTS dbo.stock_movement;
DROP TABLE IF EXISTS dbo.sale_item;
DROP TABLE IF EXISTS dbo.sale;
DROP TABLE IF EXISTS dbo.product;
DROP TABLE IF EXISTS dbo.seller;
DROP TABLE IF EXISTS dbo.app_user;
----------------------------------------------------------------------- */


/* =====================================================================
   TABELAS
   ===================================================================== */

-- Acesso ao sistema. Independente do resto do modelo.
CREATE TABLE dbo.app_user (
    id              INT IDENTITY(1,1) NOT NULL,
    email           NVARCHAR(150)     NOT NULL,
    password_hash   NVARCHAR(255)     NOT NULL,   -- sempre o hash, nunca a senha pura
    user_type       VARCHAR(20)       NOT NULL,
    CONSTRAINT PK_app_user       PRIMARY KEY (id),
    CONSTRAINT UQ_app_user_email UNIQUE (email)
);

CREATE TABLE dbo.seller (
    id      INT IDENTITY(1,1) NOT NULL,
    name    NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_seller PRIMARY KEY (id)
);

CREATE TABLE dbo.product (
    id              INT IDENTITY(1,1) NOT NULL,
    code            VARCHAR(30)       NOT NULL,
    description     NVARCHAR(150)     NOT NULL,
    price           DECIMAL(18,2)     NOT NULL,
    current_stock   INT               NOT NULL CONSTRAINT DF_product_current_stock DEFAULT 0,
    CONSTRAINT PK_product       PRIMARY KEY (id),
    CONSTRAINT UQ_product_code  UNIQUE (code),
    CONSTRAINT CK_product_price CHECK (price >= 0),
    CONSTRAINT CK_product_stock CHECK (current_stock >= 0)   -- o banco impede estoque negativo
);

-- Comissão gravada na hora da venda (se a regra mudar, o histórico não muda)
CREATE TABLE dbo.sale (
    id                      INT IDENTITY(1,1) NOT NULL,
    seller_id               INT               NOT NULL,
    sale_date               DATETIME2(0)      NOT NULL CONSTRAINT DF_sale_date DEFAULT SYSUTCDATETIME(),
    total_price             DECIMAL(18,2)     NOT NULL,
    commission_percentage   DECIMAL(5,2)      NOT NULL CONSTRAINT DF_sale_comm_pct DEFAULT 0,
    commission_amount       DECIMAL(18,2)     NOT NULL CONSTRAINT DF_sale_comm_amt DEFAULT 0,
    CONSTRAINT PK_sale        PRIMARY KEY (id),
    CONSTRAINT FK_sale_seller FOREIGN KEY (seller_id) REFERENCES dbo.seller (id),
    CONSTRAINT CK_sale_total  CHECK (total_price >= 0),
    CONSTRAINT CK_sale_pct    CHECK (commission_percentage BETWEEN 0 AND 100),
    CONSTRAINT CK_sale_amount CHECK (commission_amount >= 0)
);

CREATE TABLE dbo.sale_item (
    id          INT IDENTITY(1,1) NOT NULL,
    sale_id     INT               NOT NULL,
    product_id  INT               NOT NULL,
    quantity    INT               NOT NULL,
    unit_price  DECIMAL(18,2)     NOT NULL,   -- preço do produto NA HORA da venda
    CONSTRAINT PK_sale_item         PRIMARY KEY (id),
    CONSTRAINT FK_sale_item_sale    FOREIGN KEY (sale_id)    REFERENCES dbo.sale (id),
    CONSTRAINT FK_sale_item_product FOREIGN KEY (product_id) REFERENCES dbo.product (id),
    CONSTRAINT CK_sale_item_qty     CHECK (quantity > 0),
    CONSTRAINT CK_sale_item_price   CHECK (unit_price >= 0)
);

-- O id (IDENTITY) é o "número identificador único" da movimentação.
CREATE TABLE dbo.stock_movement (
    id              INT IDENTITY(1,1) NOT NULL,
    product_id      INT               NOT NULL,
    sale_id         INT               NULL,       -- nulo quando a movimentação não vem de uma venda
    type            VARCHAR(10)       NOT NULL,   -- 'IN' ou 'OUT'
    description     NVARCHAR(200)     NULL,
    quantity        INT               NOT NULL,
    stock_balance   INT               NOT NULL,   -- saldo do produto após esta movimentação
    movement_date   DATETIME2(0)      NOT NULL CONSTRAINT DF_stock_movement_date DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_stock_movement         PRIMARY KEY (id),
    CONSTRAINT FK_stock_movement_product FOREIGN KEY (product_id) REFERENCES dbo.product (id),
    CONSTRAINT FK_stock_movement_sale    FOREIGN KEY (sale_id)    REFERENCES dbo.sale (id),
    CONSTRAINT CK_stock_movement_type    CHECK (type IN ('IN', 'OUT')),
    CONSTRAINT CK_stock_movement_qty     CHECK (quantity > 0),
    CONSTRAINT CK_stock_movement_balance CHECK (stock_balance >= 0),
    -- Regra do sale_id: IN nunca tem venda. OUT pode ter (venda) ou não (perda, ajuste).
    CONSTRAINT CK_stock_movement_sale_by_type CHECK (type <> 'IN' OR sale_id IS NULL)
);

-- Atraso e juros NÃO ficam gravados: são calculados pela aplicação.
CREATE TABLE dbo.account_receivable (
    id              INT IDENTITY(1,1) NOT NULL,
    sale_id         INT               NOT NULL,
    amount          DECIMAL(18,2)     NOT NULL,
    due_date        DATE              NOT NULL,
    payment_date    DATE              NULL,
    CONSTRAINT PK_account_receivable      PRIMARY KEY (id),
    CONSTRAINT FK_account_receivable_sale FOREIGN KEY (sale_id) REFERENCES dbo.sale (id),
    CONSTRAINT CK_account_receivable_amt  CHECK (amount >= 0)
);


/* =====================================================================
   ÍNDICES (nas FKs; o SQL Server não cria isso sozinho)
   ===================================================================== */
CREATE INDEX IX_sale_seller_id                ON dbo.sale (seller_id);
CREATE INDEX IX_sale_sale_date                ON dbo.sale (sale_date DESC);
CREATE INDEX IX_sale_item_sale_id             ON dbo.sale_item (sale_id);
CREATE INDEX IX_sale_item_product_id          ON dbo.sale_item (product_id);
CREATE INDEX IX_stock_movement_product_date   ON dbo.stock_movement (product_id, movement_date DESC);
CREATE INDEX IX_stock_movement_sale_id        ON dbo.stock_movement (sale_id);
CREATE INDEX IX_account_receivable_sale_id    ON dbo.account_receivable (sale_id);
CREATE INDEX IX_account_receivable_due_open   ON dbo.account_receivable (due_date) WHERE payment_date IS NULL;

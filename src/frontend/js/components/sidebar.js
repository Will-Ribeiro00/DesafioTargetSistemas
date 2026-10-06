import { clearToken } from "../services/auth.js";

const MENU = [
  { key: "vendas", href: "../pags/vendas.html", label: "Vendas" },
  { key: "nova-venda", href: "../pags/nova-venda.html", label: "Nova venda" },
  { key: "estoque", href: "../pags/estoque.html", label: "Estoque" },
  { key: "comissoes", href: "../pags/comissoes.html", label: "Comissões" },
  {
    key: "contas-a-receber",
    href: "../pags/contas-a-receber.html",
    label: "Contas a receber",
  },
];

export function renderSidebar(activeKey) {
  const links = MENU.map((item) => {
    const isActive = item.key === activeKey;
    return `<a class="nav-link${isActive ? " active" : ""}" href="${item.href}"${isActive ? ' aria-current="page"' : ""}>${item.label}</a>`;
  }).join("");

  document.getElementById("sidebar").innerHTML = `
    <div class="brand">Sistema Comercial</div>
    ${links}
    <div class="nav-spacer"></div>
    <button type="button" class="nav-link nav-logout" id="logout">Sair</button>`;

  document.getElementById("logout").addEventListener("click", () => {
    clearToken();
    window.location.replace("../index.html");
  });
}

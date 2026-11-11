import { clearToken, getUserEmail } from "../services/auth.js";

const MENU = [
  { key: "vendas", href: "../pags/vendas.html", label: "Vendas", icon: "shopping-cart" },
  { key: "nova-venda", href: "../pags/nova-venda.html", label: "Nova venda", icon: "file-plus" },
  { key: "estoque", href: "../pags/estoque.html", label: "Estoque", icon: "package" },
  { key: "comissoes", href: "../pags/comissoes.html", label: "Comissões", icon: "wallet" },
  { key: "contas-a-receber", href: "../pags/contas-a-receber.html", label: "Contas a receber", icon: "bank" },
];

const icon = (name) => `<span class="icon icon-${name}" aria-hidden="true"></span>`;

export function renderSidebar(activeKey) {
  const links = MENU.map((item) => {
    const isActive = item.key === activeKey;
    return `<a class="nav-link${isActive ? " active" : ""}" href="${item.href}"${isActive ? ' aria-current="page"' : ""}>${icon(item.icon)}<span>${item.label}</span></a>`;
  }).join("");

  const sidebar = document.getElementById("sidebar");

  sidebar.innerHTML = `
    <div class="brand">
      <img class="brand-logo" src="../img/logo/target-logo.png" alt="Target Sistemas" width="32" height="32">
      <span class="brand-name">Sistema<br>Comercial</span>
    </div>
    ${links}
    <div class="nav-spacer"></div>
    <div class="nav-footer">
      <span class="nav-user"></span>
      <button type="button" class="nav-link nav-logout" id="logout">${icon("sign-out")}<span>Sair</span></button>
    </div>`;

  const email = getUserEmail() ?? "";
  const user = sidebar.querySelector(".nav-user");
  user.textContent = email;
  user.title = email;

  document.getElementById("logout").addEventListener("click", () => {
    clearToken();
    window.location.replace("../index.html");
  });
}
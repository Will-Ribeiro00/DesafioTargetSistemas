import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { showAlert } from "../components/alert.js";
import { escapeHtml, formatDateTime, formatMoney, formatPercent } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const tbody = document.getElementById("sales-body");

async function loadSales() {
  try {
    const sales = await api("/sales");

    if (sales.length === 0) {
      tbody.innerHTML = `<tr><td colspan="6" class="empty">Nenhuma venda registrada ainda.</td></tr>`;
      return;
    }

    tbody.innerHTML = sales
      .map(
        (sale) => `
        <tr>
          <td>${sale.id}</td>
          <td>${formatDateTime(sale.saleDate)}</td>
          <td>${escapeHtml(sale.sellerName)}</td>
          <td class="num">${formatMoney(sale.totalPrice)}</td>
          <td class="num">${formatPercent(sale.commissionPercentage)}</td>
          <td class="num">${formatMoney(sale.commissionAmount)}</td>
        </tr>`
      )
      .join("");
  } catch (error) {
    tbody.innerHTML = "";
    showAlert(alertBox, "error", error.messages);
  }
}

if (requireLogin()) {
  renderSidebar("vendas");
  loadSales();
}

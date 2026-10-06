import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { showAlert, hideAlert } from "../components/alert.js";
import { escapeHtml, formatMoney } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const tbody = document.getElementById("commissions-body");
const tfoot = document.getElementById("commissions-foot");
const fromInput = document.getElementById("from");
const toInput = document.getElementById("to");

const toCents = (value) => Math.round(value * 100);

async function loadCommissions() {
  hideAlert(alertBox);

  const params = new URLSearchParams();
  if (fromInput.value) params.set("from", fromInput.value);
  if (toInput.value) params.set("to", toInput.value);
  const query = params.toString() ? `?${params}` : "";

  try {
    const rows = await api(`/sellers/commissions${query}`);

    if (rows.length === 0) {
      tbody.innerHTML = `<tr><td colspan="4" class="empty">Nenhuma venda no período.</td></tr>`;
      tfoot.hidden = true;
      return;
    }

    tbody.innerHTML = rows
      .map(
        (r) => `
        <tr>
          <td>${escapeHtml(r.sellerName)}</td>
          <td class="num">${r.salesCount}</td>
          <td class="num">${formatMoney(r.totalSold)}</td>
          <td class="num">${formatMoney(r.totalCommission)}</td>
        </tr>`
      )
      .join("");

    const salesCount = rows.reduce((sum, r) => sum + r.salesCount, 0);
    const totalSold = rows.reduce((sum, r) => sum + toCents(r.totalSold), 0) / 100;
    const totalCommission = rows.reduce((sum, r) => sum + toCents(r.totalCommission), 0) / 100;

    tfoot.innerHTML = `
      <tr>
        <td>Total</td>
        <td class="num">${salesCount}</td>
        <td class="num">${formatMoney(totalSold)}</td>
        <td class="num">${formatMoney(totalCommission)}</td>
      </tr>`;
    tfoot.hidden = false;
  } catch (error) {
    tbody.innerHTML = "";
    tfoot.hidden = true;
    showAlert(alertBox, "error", error.messages);
  }
}

document.getElementById("filter-form").addEventListener("submit", (event) => {
  event.preventDefault();
  loadCommissions();
});

document.getElementById("clear-filter").addEventListener("click", () => {
  fromInput.value = "";
  toInput.value = "";
  loadCommissions();
});

if (requireLogin()) {
  renderSidebar("comissoes");
  loadCommissions();
}

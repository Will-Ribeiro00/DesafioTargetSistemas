import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { showAlert } from "../components/alert.js";
import { formatDate, formatMoney } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const tbody = document.getElementById("receivables-body");

async function loadReceivables() {
  try {
    const accounts = await api("/accountsreceivable");

    if (accounts.length === 0) {
      tbody.innerHTML = `<tr><td colspan="7" class="empty">Nenhuma conta em aberto.</td></tr>`;
      return;
    }

    tbody.innerHTML = accounts
      .map((a) => {
        const isLate = a.daysOverdue > 0;

        return `
        <tr>
          <td>${a.saleId}</td>
          <td>${formatDate(a.dueDate)}</td>
          <td class="num">${formatMoney(a.amount)}</td>
          <td class="num">${a.daysOverdue}</td>
          <td class="num">${formatMoney(a.interest)}</td>
          <td class="num"><strong>${formatMoney(a.totalAmount)}</strong></td>
          <td><span class="pill ${isLate ? "pill-late" : "pill-ok"}">${isLate ? "Em atraso" : "Em dia"}</span></td>
        </tr>`;
      })
      .join("");
  } catch (error) {
    tbody.innerHTML = "";
    showAlert(alertBox, "error", error.messages);
  }
}

if (requireLogin()) {
  renderSidebar("contas-a-receber");
  loadReceivables();
}

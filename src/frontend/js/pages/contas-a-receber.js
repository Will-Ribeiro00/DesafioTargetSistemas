import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { showAlert, hideAlert } from "../components/alert.js";
import { formatDate, formatMoney, todayUtc } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const tbody = document.getElementById("receivables-body");
const dialog = document.getElementById("payment-dialog");
const dialogSale = document.getElementById("payment-dialog-sale");
const dialogDate = document.getElementById("payment-dialog-date");
const dialogTotal = document.getElementById("payment-dialog-total");

let selectedAccount = null;

async function loadReceivables() {
  try {
    const accounts = await api("/accountsreceivable");

    if (accounts.length === 0) {
      tbody.innerHTML = `<tr><td colspan="8" class="empty">Nenhuma conta em aberto.</td></tr>`;
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
          <td>
            <button type="button" class="btn btn-sm btn-outline" data-id="${a.id}" data-sale="${a.saleId}" data-total="${a.totalAmount}">
              Baixar conta
            </button>
          </td>
        </tr>`;
      })
      .join("");
  } catch (error) {
    tbody.innerHTML = "";
    showAlert(alertBox, "error", error.messages);
  }
}

function openPaymentDialog(event) {
  const button = event.target.closest("button[data-id]");
  if (!button) return;

  const { id, sale, total } = button.dataset;
  selectedAccount = { id, sale, button };

  dialogSale.textContent = sale;
  dialogDate.textContent = formatDate(todayUtc());
  dialogTotal.textContent = formatMoney(Number(total));

  dialog.returnValue = "";
  dialog.showModal();
}

async function handleDialogClose() {
  const account = selectedAccount;
  selectedAccount = null;

  if (dialog.returnValue !== "confirm" || !account) return;

  account.button.disabled = true;
  hideAlert(alertBox);

  try {
    const payment = await api(`/accountsreceivable/${account.id}/payment`, { method: "PATCH" });

    await loadReceivables();
    showAlert(
      alertBox,
      "success",
      `Conta da venda ${account.sale} baixada em ${formatDate(payment.paymentDate)} com valor total de ${formatMoney(payment.paidAmount)}.`
    );
  } catch (error) {
    if (error.status === 404 || error.status === 409) {
      await loadReceivables();
    } else {
      account.button.disabled = false;
    }

    showAlert(alertBox, "error", error.messages);
  }
}

if (requireLogin()) {
  renderSidebar("contas-a-receber");
  tbody.addEventListener("click", openPaymentDialog);
  dialog.addEventListener("close", handleDialogClose);
  loadReceivables();
}

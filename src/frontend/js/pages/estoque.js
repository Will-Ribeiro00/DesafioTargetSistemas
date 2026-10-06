import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { showAlert, hideAlert } from "../components/alert.js";
import { escapeHtml, formatMoney } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const tbody = document.getElementById("products-body");
const datalist = document.getElementById("products-list");
const form = document.getElementById("movement-form");
const submitButton = document.getElementById("movement-button");

async function loadProducts() {
  try {
    const products = await api("/products");

    datalist.innerHTML = products
      .map((p) => `<option value="${escapeHtml(p.code)}">${escapeHtml(p.description)}</option>`)
      .join("");

    if (products.length === 0) {
      tbody.innerHTML = `<tr><td colspan="4" class="empty">Nenhum produto cadastrado.</td></tr>`;
      return;
    }

    tbody.innerHTML = products
      .map(
        (p) => `
        <tr>
          <td>${escapeHtml(p.code)}</td>
          <td>${escapeHtml(p.description)}</td>
          <td class="num">${formatMoney(p.price)}</td>
          <td class="num">${p.currentStock}</td>
        </tr>`
      )
      .join("");
  } catch (error) {
    tbody.innerHTML = "";
    showAlert(alertBox, "error", error.messages);
  }
}

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  hideAlert(alertBox);
  submitButton.disabled = true;

  try {
    const movement = await api("/stockmovements", {
      method: "POST",
      body: {
        code: document.getElementById("code").value.trim(),
        type: document.getElementById("type").value, // "IN" ou "OUT"
        description: document.getElementById("description").value.trim() || null,
        quantity: Number(document.getElementById("quantity").value),
      },
    });

    showAlert(alertBox, "success", `Movimentação nº ${movement.id} registrada. Saldo final: ${movement.stockBalance}.`);
    form.reset();
    await loadProducts();
  } catch (error) {
    showAlert(alertBox, "error", error.messages);
  } finally {
    submitButton.disabled = false;
  }
});

if (requireLogin()) {
  renderSidebar("estoque");
  loadProducts();
}

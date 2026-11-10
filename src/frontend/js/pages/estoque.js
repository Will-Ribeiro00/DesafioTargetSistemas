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
const dialog = document.getElementById("movement-dialog");
const dialogProduct = document.getElementById("movement-dialog-product");
const dialogType = document.getElementById("movement-dialog-type");
const dialogQuantity = document.getElementById("movement-dialog-quantity");
const dialogDescription = document.getElementById("movement-dialog-description");

let products = [];
let pendingMovement = null;

async function loadProducts() {
  try {
    products = await api("/products");

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

function openMovementDialog(event) {
  event.preventDefault();
  hideAlert(alertBox);

  const typeSelect = document.getElementById("type");

  const movement = {
    code: document.getElementById("code").value.trim(),
    type: typeSelect.value,
    description: document.getElementById("description").value.trim() || null,
    quantity: Number(document.getElementById("quantity").value),
  };

  const errors = [];

  if (!movement.code) errors.push("Informe o código do produto.");
  if (!Number.isInteger(movement.quantity) || movement.quantity < 1)
    errors.push("Informe uma quantidade maior que zero.");

  if (errors.length > 0) return showAlert(alertBox, "error", errors);

  const product = products.find((p) => p.code === movement.code);

  dialogProduct.textContent = product ? `${product.code} — ${product.description}` : movement.code;
  dialogType.textContent = typeSelect.options[typeSelect.selectedIndex].text;
  dialogQuantity.textContent = movement.quantity;
  dialogDescription.textContent = movement.description ?? "—";

  pendingMovement = movement;
  dialog.returnValue = "";
  dialog.showModal();
}

async function handleDialogClose() {
  const movement = pendingMovement;
  pendingMovement = null;

  if (dialog.returnValue !== "confirm" || !movement) return;

  submitButton.disabled = true;

  try {
    const result = await api("/stockmovements", { method: "POST", body: movement });

    showAlert(alertBox, "success", `Movimentação nº ${result.id} registrada. Saldo final: ${result.stockBalance}.`);
    form.reset();
    await loadProducts();
  } catch (error) {
    showAlert(alertBox, "error", error.messages);
  } finally {
    submitButton.disabled = false;
  }
}

if (requireLogin()) {
  renderSidebar("estoque");
  form.addEventListener("submit", openMovementDialog);
  dialog.addEventListener("close", handleDialogClose);
  loadProducts();
}

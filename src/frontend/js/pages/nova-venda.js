import { api } from "../services/api.js";
import { requireLogin } from "../services/auth.js";
import { renderSidebar } from "../components/sidebar.js";
import { MAX_DUE_DAYS } from "../config.js";
import { showAlert, hideAlert } from "../components/alert.js";
import { addDaysUtc, escapeHtml, formatMoney, formatPercent, todayUtc } from "../utils/format.js";

const alertBox = document.getElementById("alert");
const form = document.getElementById("sale-form");
const sellerField = document.getElementById("seller-field");
const dueDateInput = document.getElementById("due-date");
const itemsBody = document.getElementById("items-body");
const totalText = document.getElementById("total");
const submitButton = document.getElementById("submit-button");

let products = [];

async function loadSellers() {
  try {
    const sellers = await api("/sellers");
    const options = sellers.map((s) => `<option value="${s.id}">${escapeHtml(s.name)}</option>`).join("");

    sellerField.innerHTML = `
      <label for="seller">Vendedor</label>
      <select id="seller"><option value="">Selecione o vendedor</option>${options}</select>`;
  } catch {
    sellerField.innerHTML = `
      <label for="seller">ID do vendedor</label>
      <input id="seller" type="number" min="1" placeholder="Ex.: 1">
      <span class="hint">A lista de vendedores ainda não existe na API.</span>`;
  }
}

async function loadProducts() {
  try {
    products = await api("/products");
  } catch (error) {
    showAlert(alertBox, "error", error.messages);
  }
}

function productOptions() {
  return products
    .map((p) => `<option value="${escapeHtml(p.code)}">${escapeHtml(p.code)} — ${escapeHtml(p.description)}</option>`)
    .join("");
}

function addItemRow() {
  const row = document.createElement("tr");
  row.innerHTML = `
    <td><select class="item-product" aria-label="Produto">${productOptions()}</select></td>
    <td><input class="item-quantity" type="number" min="1" value="1" aria-label="Quantidade"></td>
    <td class="num item-price"></td>
    <td class="num item-subtotal"></td>
    <td><button type="button" class="btn-link remove-item">Remover</button></td>`;

  itemsBody.appendChild(row);
  updateRow(row);
}

function updateRow(row) {
  const code = row.querySelector(".item-product").value;
  const product = products.find((p) => p.code === code);
  const quantity = Number(row.querySelector(".item-quantity").value) || 0;
  const price = product ? product.price : 0;

  row.querySelector(".item-price").textContent = formatMoney(price);
  row.querySelector(".item-subtotal").textContent = formatMoney(price * quantity);
  updateTotal();
}

function updateTotal() {
  let cents = 0;

  itemsBody.querySelectorAll("tr").forEach((row) => {
    const product = products.find((p) => p.code === row.querySelector(".item-product").value);
    const quantity = Number(row.querySelector(".item-quantity").value) || 0;
    if (product) cents += Math.round(product.price * 100) * quantity;
  });

  totalText.textContent = formatMoney(cents / 100);
}

itemsBody.addEventListener("input", (event) => updateRow(event.target.closest("tr")));
itemsBody.addEventListener("change", (event) => updateRow(event.target.closest("tr")));
itemsBody.addEventListener("click", (event) => {
  if (event.target.classList.contains("remove-item")) {
    event.target.closest("tr").remove();
    updateTotal();
  }
});

document.getElementById("add-item").addEventListener("click", addItemRow);

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  hideAlert(alertBox);

  const body = {
    sellerId: Number(document.getElementById("seller").value),
    dueDate: dueDateInput.value,
    items: [...itemsBody.querySelectorAll("tr")].map((row) => ({
      code: row.querySelector(".item-product").value,
      quantity: Number(row.querySelector(".item-quantity").value),
    })),
  };

  if (!body.sellerId) return showAlert(alertBox, "error", "Selecione o vendedor.");
  if (!body.dueDate) return showAlert(alertBox, "error", "Informe a data de vencimento.");
  if (body.items.length === 0) return showAlert(alertBox, "error", "Adicione pelo menos um item.");

  submitButton.disabled = true;

  try {
    const sale = await api("/sales", { method: "POST", body });

    showAlert(
      alertBox,
      "success",
      `Venda nº ${sale.id} registrada. Total de ${formatMoney(sale.totalPrice)}, ` +
        `comissão de ${formatPercent(sale.commissionPercentage)} (${formatMoney(sale.commissionAmount)}).`
    );

    itemsBody.innerHTML = "";
    addItemRow();
    dueDateInput.value = "";
  } catch (error) {
    showAlert(alertBox, "error", error.messages);
  } finally {
    submitButton.disabled = false;
  }
});

async function init() {
  const today = todayUtc();
  dueDateInput.min = today;
  dueDateInput.max = addDaysUtc(today, MAX_DUE_DAYS);

  await Promise.all([loadSellers(), loadProducts()]);
  addItemRow();
}

if (requireLogin()) {
  renderSidebar("nova-venda");
  init();
}

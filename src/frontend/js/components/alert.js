import { escapeHtml } from "../utils/format.js";

export function showAlert(element, type, messages) {
  const list = Array.isArray(messages) ? messages : [messages];

  element.className = `alert alert-${type}`;
  element.setAttribute("role", type === "error" ? "alert" : "status");
  element.innerHTML =
    list.length === 1
      ? escapeHtml(list[0])
      : "<ul>" + list.map((m) => `<li>${escapeHtml(m)}</li>`).join("") + "</ul>";
  element.hidden = false;
}

export function hideAlert(element) {
  element.hidden = true;
  element.textContent = "";
}

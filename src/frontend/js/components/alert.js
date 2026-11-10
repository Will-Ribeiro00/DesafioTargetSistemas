import { escapeHtml } from "../utils/format.js";

export function showAlert(element, type, messages) {
  const list = Array.isArray(messages) ? messages : [messages];

  element.className = `alert alert-${type}`;
  element.setAttribute("role", type === "error" ? "alert" : "status");

  if (list.length === 1) {
    element.textContent = list[0];
  } else {
    const ul = document.createElement("ul");

    for (const message of list) {
      const li = document.createElement("li");
      li.textContent = message;
      ul.append(li);
    }

    element.replaceChildren(ul);
  }

  element.hidden = false;
}

export function hideAlert(element) {
  element.hidden = true;
  element.textContent = "";
}

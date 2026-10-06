import { api } from "../services/api.js";
import { getToken, setToken } from "../services/auth.js";
import { showAlert, hideAlert } from "../components/alert.js";

if (getToken()) window.location.href = "pags/vendas.html";

const form = document.getElementById("login-form");
const alertBox = document.getElementById("alert");
const submitButton = document.getElementById("login-button");

if (new URLSearchParams(window.location.search).has("expired")) {
  showAlert(alertBox, "error", "Sua sessão expirou. Entre novamente.");
}

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  hideAlert(alertBox);
  submitButton.disabled = true;

  try {
    const response = await api("/login", {
      method: "POST",
      auth: false,
      body: {
        email: document.getElementById("email").value.trim(),
        password: document.getElementById("password").value,
      },
    });

    setToken(response.accessToken);
    window.location.href = "pags/vendas.html";
  } catch (error) {
    showAlert(alertBox, "error", error.messages ?? ["Erro inesperado. Tente novamente."]);
  } finally {
    submitButton.disabled = false;
  }
});

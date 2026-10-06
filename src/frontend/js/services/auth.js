const TOKEN_KEY = "access_token";

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

export function requireLogin() {
  window.addEventListener("pageshow", (event) => {
    if (event.persisted && !getToken()) {
      window.location.replace("../index.html");
    }
  });

  if (!getToken()) {
    window.location.replace("../index.html");
    return false;
  }
  return true;
}
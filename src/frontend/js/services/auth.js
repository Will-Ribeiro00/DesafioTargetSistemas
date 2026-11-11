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

export function getUserEmail() {
  const token = getToken();
  if (!token) return null;

  try {
    const payload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const claims = JSON.parse(atob(payload));

    return claims.email ?? claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ?? null;
  } catch {
    return null;
  }
}
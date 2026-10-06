import { API_URL } from "../config.js";
import { getToken, clearToken } from "./auth.js";

export class ApiError extends Error {
  constructor(messages, status) {
    super(messages.join(" "));
    this.messages = messages;
    this.status = status;
  }
}

export async function api(path, { method = "GET", body, auth = true } = {}) {
  const headers = { "Accept-Language": "pt-BR" };
  if (body !== undefined) headers["Content-Type"] = "application/json";
  if (auth && getToken()) headers["Authorization"] = `Bearer ${getToken()}`;

  let response;
  try {
    response = await fetch(API_URL + path, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
  } catch {
    throw new ApiError(["Não foi possível conectar à API. Verifique se ela está rodando."], 0);
  }

  const data = await response.json().catch(() => null);

  if (response.status === 401 && auth) {
    clearToken();
    window.location.href = data?.tokenIsExpired ? "../index.html?expired=1" : "../index.html";
    throw new ApiError(data?.errors ?? ["Sessão inválida."], 401);
  }

  if (!response.ok) {
    throw new ApiError(data?.errors ?? ["Erro inesperado. Tente novamente."], response.status);
  }

  return data;
}

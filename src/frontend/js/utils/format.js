const moneyFormatter = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });

export function formatMoney(value) {
  return moneyFormatter.format(value);
}

export function formatPercent(value) {
  return value.toLocaleString("pt-BR", { maximumFractionDigits: 2 }) + "%";
}

export function formatDate(dateOnly) {
  const [year, month, day] = dateOnly.split("-");
  return `${day}/${month}/${year}`;
}

export function formatDateTime(utcText) {
  const hasZone = /(Z|[+-]\d{2}:\d{2})$/.test(utcText);
  const date = new Date(hasZone ? utcText : utcText + "Z");

  return date
    .toLocaleString("pt-BR", { day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit" })
    .replace(",", "");
}

export function escapeHtml(text) {
  return String(text).replace(/[&<>"']/g, (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
}

export function todayUtc() {
  return new Date().toISOString().slice(0, 10);
}

export function addDaysUtc(isoDate, days) {
  const date = new Date(isoDate + "T00:00:00Z");
  date.setUTCDate(date.getUTCDate() + days);
  return date.toISOString().slice(0, 10);
}

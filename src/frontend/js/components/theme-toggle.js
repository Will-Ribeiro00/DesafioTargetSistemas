const STORAGE_KEY = "theme";
const systemDark = window.matchMedia("(prefers-color-scheme: dark)");

function savedTheme() {
  try {
    return localStorage.getItem(STORAGE_KEY);
  } catch {
    return null;
  }
}

function saveTheme(theme) {
  try {
    localStorage.setItem(STORAGE_KEY, theme);
  } catch {
    // Sem acesso ao armazenamento
  }
}

function currentTheme() {
  return document.documentElement.dataset.theme === "dark" ? "dark" : "light";
}

function applyTheme(theme) {
  if (theme === "dark") {
    document.documentElement.dataset.theme = "dark";
  } else {
    delete document.documentElement.dataset.theme;
  }
}

export function renderThemeToggle(container) {
  const button = document.createElement("button");
  button.type = "button";
  button.className = "theme-toggle";

  const icon = document.createElement("span");
  icon.setAttribute("aria-hidden", "true");
  button.append(icon);

  const update = () => {
    const dark = currentTheme() === "dark";
    const label = dark ? "Ativar modo claro" : "Ativar modo escuro";

    icon.className = `icon ${dark ? "icon-sun" : "icon-moon"}`;
    button.setAttribute("aria-label", label);
    button.title = label;
  };

  button.addEventListener("click", () => {
    const next = currentTheme() === "dark" ? "light" : "dark";
    applyTheme(next);
    saveTheme(next);
    update();
  });

  systemDark.addEventListener("change", (event) => {
    if (savedTheme()) return;
    applyTheme(event.matches ? "dark" : "light");
    update();
  });

  update();
  container.append(button);
}
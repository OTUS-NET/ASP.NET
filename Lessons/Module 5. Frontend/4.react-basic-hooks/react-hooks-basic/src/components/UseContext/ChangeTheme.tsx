import React, { createContext, useContext, useState } from "react";

// Определяем тип для темы
type Theme = "light" | "dark";

// Создаем контекст для темы
const ThemeContext = createContext<{
  theme: Theme;
  toggleTheme: () => void;
}>({
  theme: "light",
  toggleTheme: () => {},
});

// Компонент Header
function Header() {
  const { theme } = useContext(ThemeContext);

  return (
    <header
      style={{
        backgroundColor: theme === "light" ? "#f0f0f0" : "#333",
        color: theme === "light" ? "#000" : "#fff",
        padding: "1rem",
        textAlign: "center",
      }}
    >
      <h1>Header</h1>
    </header>
  );
}

// Компонент Content
function Content() {
  const { theme } = useContext(ThemeContext);

  return (
    <div
      style={{
        backgroundColor: theme === "light" ? "#fff" : "#222",
        color: theme === "light" ? "#000" : "#fff",
        padding: "1rem",
        minHeight: "200px",
      }}
    >
      <p>This is the main content.</p>
    </div>
  );
}

// Компонент ThemeSwitcher
function ThemeSwitcher() {
  const { theme, toggleTheme } = useContext(ThemeContext);

  return (
    <button
      onClick={toggleTheme}
      style={{
        backgroundColor: theme === "light" ? "#333" : "#f0f0f0",
        color: theme === "light" ? "#fff" : "#000",
        padding: "0.5rem 1rem",
        border: "none",
        borderRadius: "4px",
        cursor: "pointer",
      }}
    >
      Switch to {theme === "light" ? "Dark" : "Light"} Theme
    </button>
  );
}

// Главный компонент App
function ChangeThemeApp() {
  const [theme, setTheme] = useState<Theme>("light");

  // Функция для переключения темы
  const toggleTheme = () => {
    setTheme((prevTheme) => (prevTheme === "light" ? "dark" : "light"));
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      <div>
        <Header />
        <Content />
        <div style={{ textAlign: "center", margin: "1rem" }}>
          <ThemeSwitcher />
        </div>
      </div>
    </ThemeContext.Provider>
  );
}

export default ChangeThemeApp;
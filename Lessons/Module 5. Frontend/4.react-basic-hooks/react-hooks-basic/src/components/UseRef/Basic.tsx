import { useRef } from "react";

function UseRefInputApp() {
  // Создаем ref для input
  const inputRef = useRef<HTMLInputElement>(null);

  const handleFocusAndColor = () => {
    // Проверяем, что inputRef.current существует
    if (inputRef.current) {
      // Фокусируемся на input
      inputRef.current.focus();
      // Меняем цвет текста в input
      inputRef.current.style.color = "red"; // Можно использовать любой цвет
    }
  };

  return (
    <div>
      <input
        ref={inputRef}
        type="text"
        placeholder="Введите текст"
        style={{ color: "black" }} // Начальный цвет текста
      />
      <button onClick={handleFocusAndColor}>Фокус и цвет</button>
    </div>
  );
}

export default UseRefInputApp;
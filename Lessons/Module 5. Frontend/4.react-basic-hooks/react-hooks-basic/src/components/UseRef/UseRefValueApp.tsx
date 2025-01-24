import { useEffect, useRef, useState } from "react";

function UseRefValueApp() {
  const [name, setName] = useState("123");
  const prevNameRef = useRef<string>();

  useEffect(() => {
    prevNameRef.current = name; // Сохраняем текущее значение name
  }, [name]); // Эффект срабатывает при изменении name

  return (
    <div>
      <input
        type="text"
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Введите ваше имя"
      />
      <p>Текущее имя: {name}</p>
      <p>Предыдущее имя: {prevNameRef.current}</p>
    </div>
  );
}

export default UseRefValueApp;
import React, { useState, useCallback, useEffect } from "react";

// Дочерний компонент, который принимает функцию и вызывает её при нажатии на кнопку
const Button = React.memo(({ onClick, label }: { onClick: () => void; label: string }) => {
  console.log(`Рендер кнопки: ${label}`); // Логируем рендер кнопки
  return <button onClick={onClick}>{label}</button>;
});

function UseCallbackCounterApp() {
  const [count, setCount] = useState(0);
  const [text, setText] = useState("");

  // Функция для увеличения счётчика
  const increment = useCallback(() => {
    setCount((prevCount) => prevCount + 1);
  }, []); // Зависимостей нет, функция создаётся только один раз

  // Функция для сброса счётчика
  const reset = useCallback(() => {
    setCount(0);
  }, []); // Зависимостей нет, функция создаётся только один раз

  
  const simpleFunc = () => "";

  // Логируем изменение ссылок на функции increment, reset с помощью useEffect
  useEffect(() => {
    console.log("Ссылка на increment изменилась");
  }, [increment]);
  
  useEffect(() => {
    console.log("Ссылка на reset изменилась");
  }, [reset]);

  useEffect(() => {
    console.log("Ссылка на simpleFunc изменилась");
  }, [simpleFunc]);


  console.log(`Рендер корневого компонента`);
  return (
    <div>
      <h1>Счётчик: {count}</h1>
      <Button onClick={increment} label="Увеличить" />
      <Button onClick={reset} label="Сбросить" />

      <input
        type="text"
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Введите текст"
      />
      <p>Текст: {text}</p>
    </div>
  );
}

export default UseCallbackCounterApp;
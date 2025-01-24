import React, { useState, useMemo } from "react";

// Тип для пользователя
type User = {
  id: number;
  name: string;
  age: number;
};

// Начальные данные пользователей
const initialUsers: User[] = [
  { id: 1, name: "Alice", age: 25 },
  { id: 2, name: "Bob", age: 30 },
  { id: 3, name: "Charlie", age: 35 },
  { id: 4, name: "David", age: 40 },
];

function UserList() {
  const [users, setUsers] = useState<User[]>(initialUsers);
  const [filter, setFilter] = useState("");
  const [renderCount, setRenderCount] = useState(0); // Счётчик рендеров

  // Фильтрация пользователей с использованием useMemo
  const filteredUsers = useMemo(() => {
    console.log("Вычисление отфильтрованного списка..."); // Логируем вычисления
    return users.filter((user) =>
      user.name.toLowerCase().includes(filter.toLowerCase())
    );
  }, [users, filter]); // Зависимости: users и filter


  //Фильтрация без useMemo будет выполняться при каждом рендере
  /* const filteredUsers = users.filter((user) => {
    console.log("Вычисление отфильтрованного списка..."); // Логируем вычисления
    return user.name.toLowerCase().includes(filter.toLowerCase())
  }
  ); */


  return (
    <div>
      <h1>Список пользователей</h1>
      <input
        type="text"
        value={filter}
        onChange={(e) => setFilter(e.target.value)}
        placeholder="Поиск по имени"
      />
      <ul>
        {filteredUsers.map((user) => (
          <li key={user.id}>
            {user.name} (Возраст: {user.age})
          </li>
        ))}
      </ul>
      {/* Кнопка для принудительного ререндера */}
      <button onClick={() => setRenderCount(renderCount + 1)}>Вызов рендера: {renderCount}</button>
    </div>
  );
}

export default UserList;
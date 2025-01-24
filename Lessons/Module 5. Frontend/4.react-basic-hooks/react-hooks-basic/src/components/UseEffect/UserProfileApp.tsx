import React, { useEffect, useState } from "react";

function UserProfileApp() {
  const [user, setUser] = useState({ name: "Andrei", age: 32 });
  const [message, setMessage] = useState("");

  // Эффект, который срабатывает при изменении объекта user
  useEffect(() => {
    setMessage(`User data updated: ${user.name}, ${user.age} years old`);
    console.log("User data changed:", user);
  }, [user]); // Зависимость от объекта user

// Альтернативный эффект, который срабатывает при изменении полей name или age у user
  useEffect(() => {
   setMessage(`User data updated 2: ${user.name}, ${user.age} years old`);
   console.log("User data changed 2:", user);
 }, [user.name]); // Зависимость от полей name и age

  const updateUser = () => {
    // Изменяем объект user (создаем новый объект, чтобы изменить ссылку)
    setUser({ name: "Andrei", age: user.age + 1 });
  };

  return (
    <div>
      <h1>User Profile</h1>
      <p>{message}</p>
      <button onClick={updateUser}>Increase Age</button>
    </div>
  );
}

export default UserProfileApp;
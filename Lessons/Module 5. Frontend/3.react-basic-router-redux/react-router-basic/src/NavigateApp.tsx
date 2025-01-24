import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

const PrivatePage: React.FC = () => {
  const isAuthenticated = false; // Пример проверки авторизации

  if (!isAuthenticated) {
    return <Navigate to="/login" />; // Перенаправление на страницу входа
  }

  return <h1>Приватная страница</h1>;
};

const Login: React.FC = () => {
  return <h1>Страница входа</h1>;
};

const NavigateApp: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/private" element={<PrivatePage />} />
        <Route path="/login" element={<Login />} />
      </Routes>
    </Router>
  );
};

export default NavigateApp;
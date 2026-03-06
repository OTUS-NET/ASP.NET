import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useNavigate, Link } from 'react-router-dom';

const Home: React.FC = () => {
  return <>
    <div className="page">
      <h2>Главная страница!</h2>
      <Link to="/private">Приватная страница</Link>
    </div>
  </>;
};

// Приватная страница с примером использования Navigate - перенаправление на страницу входа
const PrivatePage: React.FC<{ isAuthenticated: boolean }> = ({ isAuthenticated }) => {
  if (!isAuthenticated) {
    return <Navigate to="/login" />; // Перенаправление на страницу входа если не авторизован
  }

  return <div className='page'>
    <h2>Приватная страница</h2>
    <Link to="/">Главная страница</Link>
  </div>;
};

const Login: React.FC<{ onLogin: () => void }> = ({ onLogin }) => {
  const navigate = useNavigate();

  const handleLogin = () => {
    onLogin();
    navigate('/private');
  };

  return (
    <div className="page">
      <h2>Авторизация</h2>
      <p>Введите свои учетные данные:</p>
      <input type="text" placeholder="Логин" />
      <input type="password" placeholder="Пароль" />
      <br />
      <button onClick={handleLogin}>Войти</button>
    </div>
  );
};

const NavigateApp: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const handleLogin = () => {
    setIsAuthenticated(true);
  };

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/private" element={<PrivatePage isAuthenticated={isAuthenticated} />} />
        <Route path="/login" element={<Login onLogin={handleLogin} />} />
      </Routes>
    </Router>
  );
};

export default NavigateApp;
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useParams } from 'react-router-dom';
import './App.css';

// Компонент для страницы пользователя
const User: React.FC = () => {
  const { id } = useParams<{ id: string  }>(); // Извлекаем параметр id из URL с помощью хука useParams
  return <h1>Пользователь с ID: {id}</h1>; 
};

const RouteParamsApp: React.FC = () => {
  return (
    <Router>
      <nav className="navbar">
        <ul className="navbar-links">
          <li>
            <Link to="/user/1">Пользователь 1</Link>
          </li>
          
          <li>
            <Link to="/user/2">Пользователь 2</Link>
          </li>
        </ul>
      </nav>
      <p>Страница  <Link to="/user/1">Пользователь 1</Link></p>

      <Routes>
        <Route path="/user/setting/:id" element={<User />} />
      </Routes>
    </Router>
  );
};

export default RouteParamsApp;
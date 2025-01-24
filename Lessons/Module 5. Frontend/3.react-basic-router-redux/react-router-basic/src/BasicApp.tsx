import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import './App.css';

// Компонент для главной страницы
const Home: React.FC = () => {
  return <>
    <div className="page">
      <h1>Добро пожаловать на главную страницу!</h1>
    </div>
  </>;
};

// Компонент для страницы "О нас"
const About: React.FC = () => {
  return <h1>О нас</h1>;
};

// Компонент для страницы 404
const NotFound: React.FC = () => {
  return <h1>Страница не найдена</h1>;
};

// Основной компонент приложения
const BasicApp: React.FC = () => {
  return (
    <Router>
      <nav className="navbar">
        <ul className="navbar-links">
          <li>
            <Link to="/">Главная</Link>
          </li>
          <li>
            <Link to="/about">О нас</Link>
          </li>
        </ul>
      </nav>

      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route path="*" element={<NotFound />} /> {/* Страница 404 */}
      </Routes>
    </Router>
  );
};

export default BasicApp;
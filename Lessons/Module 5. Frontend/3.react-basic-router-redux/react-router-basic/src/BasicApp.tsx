import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import './App.css';
import { NotFound } from './Components/NotFound';

// Компонент главной страницы
const Home: React.FC = () => {
  return <>
    <div className="page">
      <h1>Добро пожаловать!</h1>
    </div>
  </>;
};

// Компонент страницы "О нас"
const About: React.FC = () => {
  return <div className="page"><h2>Тема занятия:</h2><div>Изучаем React Router</div></div>;
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
            <Link to="/topic">Тема занятия</Link>
          </li>
        </ul>
      </nav>

      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/topic" element={<About />} />
        <Route path="*" element={<NotFound />} /> {/* Страница 404 */}
      </Routes>
    </Router>
  );
};

export default BasicApp;
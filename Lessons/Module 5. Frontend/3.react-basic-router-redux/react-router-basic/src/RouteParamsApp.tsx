import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useParams } from 'react-router-dom';
import './App.css';
import { NotFound } from './Components/NotFound';

// Компонент для страницы пользователя
const Main: React.FC = () => {

  return <>
    <div className='page'>
      <p style={{ fontSize: 22 }}>Текст главной страницы с примером <Link to="/user/1">ссылки</Link> в тексте</p>
    </div>
  </>;
};

// Компонент для страницы пользователя
const User: React.FC = () => {
  const { id, name } = useParams<{ id: string, name:string }>(); // Извлекаем параметр id из URL с помощью хука useParams
  return <div className='page'>
    <h3>Пользователь с ID: {id}</h3>
    <b>Детали пользователя {id}</b>
  </div>;
};

const RouteParamsApp: React.FC = () => {
  return (
    <Router>
      <nav className="navbar">
        <ul className="navbar-links">
          <li>
            <Link to="/">Главная</Link>
          </li>
          <li>
            <Link to="/user/1">Пользователь 1</Link>
          </li>
          <li>
            <Link to="/user/2">Пользователь 2</Link>
          </li>
        </ul>
      </nav>

      <Routes>
        <Route path="/user/:id" element={<User />} />
        <Route path="/" element={<Main />} />
        <Route path="*" element={<NotFound />} /> {/* Страница 404 */}
      </Routes>


    </Router>
  );
};

export default RouteParamsApp;
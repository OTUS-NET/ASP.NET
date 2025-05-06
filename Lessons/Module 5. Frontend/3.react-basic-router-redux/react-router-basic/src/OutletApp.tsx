import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Outlet } from 'react-router-dom';
import './App.css';

// Компонент для главной страницы
const Home: React.FC = () => {
  return <>
    <div className="page">
      <h2>Добро пожаловать!</h2>
    </div>
  </>;
};

//Компонент дашборда
const Dashboard: React.FC = () => {
  return (
    <div className='page'>
      <h2>Компонент Dashboard</h2>
      <nav className="navbar">
        <ul className="navbar-links">
          <li>
            <Link to="/">Главная</Link>
          </li>
          <li>
            <Link to="profile">Профиль</Link>
          </li>
          <li>
            <Link to="settings">Настройки</Link>
          </li>
        </ul>
      </nav>
      <h3>История заказов</h3>
      
      <table>
        <thead>
          <tr>
            <th>Дата</th>
            <th>Заказ</th>
            <th>Сумма</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>01.01.2023</td>
            <td>Заказ 1</td>
            <td>$100</td>
          </tr>
          <tr>
            <td>02.01.2023</td>
            <td>Заказ 2</td>
            <td>$200</td>
          </tr>
        </tbody>
      </table>
      {/* Outlet рендерит дочерние маршруты */}
      <Outlet />
    </div>
  );
};

const Profile: React.FC = () => {
  return <div className='page'><h2>Компонент Profile</h2>

  <p>Имя: <b>Иван</b></p>
  <p>Фамилия: <b>Иванов</b></p>
  <p>Возраст: <b>30</b></p>
  <p>Страна: <b>Россия</b></p>
  <p>Город: <b>Москва</b></p>
  </div>;
};

const Settings: React.FC = () => {
  return <div className='page'>
  <h2>Компонент Settings</h2>
  Открыть <Link to="./options">опции</Link>
    <Outlet />
  </div>
};

const Options: React.FC = () => {
  return <div className='page'><h2>Компонент Options</h2>
  <p> <Link to="../">Закрыть</Link></p></div>;
};


const OutletApp: React.FC = () => {
  return (
    <Router>
      <nav className="navbar">
        <ul className="navbar-links">
          <li>
            <Link to="/">Главная</Link>
          </li>
          <li>
            <Link to="/dashboard">Дашборд</Link>
          </li>
        </ul>
      </nav>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/dashboard" element={<Dashboard />}>
          <Route path="profile" element={<Profile />} />
          <Route path="settings" element={<Settings />}>
            <Route path="options" element={<Options />} />
          </Route>
        </Route>
      </Routes>
    </Router>
  );
};

export default OutletApp;
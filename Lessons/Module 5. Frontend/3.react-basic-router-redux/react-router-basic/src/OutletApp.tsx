import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Outlet } from 'react-router-dom';
import './App.css';

// Компонент для главной страницы
const Home: React.FC = () => {
  return <>
    <div className="page">
      <h1>Добро пожаловать на главную страницу!</h1>
    </div>
  </>;
};

//Компонент дашборда
const Dashboard: React.FC = () => {
  return (
    <div>
      <h1>Дашборд</h1>
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

      {/* Outlet рендерит дочерние маршруты */}
      [][][]
      <Outlet />
      [][][]
    </div>
  );
};

const Profile: React.FC = () => {
  return <h2>Профиль</h2>;
};

const Settings: React.FC = () => {
  return <>
  <h2>Настройки2</h2>;
  <Outlet/>
  </>
};

const Settings3Level: React.FC = () => {
  return <h2>Настройки3</h2>;
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
          <Route path="settings3" element={<Settings3Level />}/>
          </Route>
          
        </Route>
      </Routes>
    </Router>
  );
};

export default OutletApp;
import React from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import './App.css';

const Home: React.FC = () => {
  const navigate = useNavigate();

  const goToAboutPage = () => {
    navigate('/about'); // Переход на страницу "О нас"
  };

  return (
    <div>
      <h1>Главная страница</h1>
      <button onClick={goToAboutPage}>Перейти на страницу "О нас"</button>
    </div>
  );
};

const About: React.FC = () => {
  return <h1>О нас</h1>;
};

const UseNavigateApp: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/about" element={<About />} />
      </Routes>
    </Router>
  );
};

export default UseNavigateApp;
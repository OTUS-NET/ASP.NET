import React from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import './App.css';

const Home: React.FC = () => {
  const navigate = useNavigate();

  const goToAboutPage = () => {
    navigate('/about'); // Переход на страницу "О нас"
  };

  return (
    <div className="page">
      <h2>Главная страница</h2>
      <hr />
      <br />
      <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Corporis asperiores soluta incidunt officia eum reprehenderit modi repudiandae pariatur quidem dolore?</p>
      <br />
      <button onClick={goToAboutPage}>Узнать про "useNavigate"</button>
    </div>
  );
};

const About: React.FC = () => {
  const navigate = useNavigate();
  const goToHomePage = () => {
    navigate('/'); // Переход на страницу "О нас"
  };
  return <div className="page">
    <h2>Про useNavigate</h2>
    <hr />
    <br />
    <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Fuga, ipsam sapiente omnis dicta mollitia quo id quaerat delectus culpa dolor!</p>
    <br />
    <button onClick={goToHomePage}>Перейти на страницу "Главная"</button>
  </div>;
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
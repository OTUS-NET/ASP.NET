import React from 'react';
import FeedbackForm from './components/FeedbackForm';
import Dashboard from './components/Dashboard';
import ProductsCatalog from './components/ProductsCatalog';
import CustomNavbar from './components/CustomNavbar';
import { Route, Routes } from 'react-router-dom';

const App: React.FC = () => {
  return (
    <>
      <CustomNavbar />
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/catalog" element={<ProductsCatalog />} />
        <Route path="/feedback" element={<FeedbackForm />} />
      </Routes>
    </>
  );
};

export default App;
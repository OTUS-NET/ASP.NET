import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Container, CssBaseline, AppBar, Toolbar, Typography, Button } from '@mui/material';
import FeedbackForm from './components/FeedbackForm';
import ProductCatalog from './components/ProductCatalog';
import CRMDashboard from './components/Dashboard';

const App: React.FC = () => {
  return (
    <Router>
      <CssBaseline />
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Мое приложение
          </Typography>
          <Button color="inherit" component={Link} to="/">
            Обратная связь
          </Button>
          <Button color="inherit" component={Link} to="/catalog">
            Каталог товаров
          </Button>
          <Button color="inherit" component={Link} to="/dashboard">
            Дашборд CRM
          </Button>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Routes>
          <Route path="/" element={<FeedbackForm />} />
          <Route path="/catalog" element={<ProductCatalog />} />
          <Route path="/dashboard" element={<CRMDashboard />} />
        </Routes>
      </Container>
    </Router>
  );
};

export default App;
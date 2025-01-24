import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Navbar, Nav, Container } from 'react-bootstrap';
import Counter from './Components/Counter';
import ProductList from './Components/ProductList';
import FeedbackForm from './Components/FeedbackForm';

console.log("VITE_API_BASE_URL:",import.meta.env.VITE_API_BASE_URL)
const App: React.FC = () => {
    return (
        <Router>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand as={Link} to="/">Мое приложение</Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto">
                            <Nav.Link as={Link} to="/">Счетчик</Nav.Link>
                            <Nav.Link as={Link} to="/products">Каталог товаров</Nav.Link>
                            <Nav.Link as={Link} to="/feedback">Обратная связь</Nav.Link>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>
            <Container className="mt-4">
                <Routes>
                    <Route path="/" element={<Counter />} />
                    <Route path="/products" element={<ProductList />} />
                    <Route path="/feedback" element={<FeedbackForm />} />
                </Routes>
                <div>
                  {    }
                </div>
            </Container>
        </Router>
    );
};

export default App;
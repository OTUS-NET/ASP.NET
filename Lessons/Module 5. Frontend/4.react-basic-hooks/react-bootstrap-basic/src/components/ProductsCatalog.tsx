import React, { useState } from 'react';
import { Container, Row, Col, Card, ListGroup, Button } from 'react-bootstrap';

const ProductsCatalog: React.FC = () => {
  // Состояние для выбранной категории
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  // Пример данных для категорий и товаров
  const categories = [
    { id: '1', name: 'Электроника' },
    { id: '2', name: 'Одежда' },
    { id: '3', name: 'Книги' },
  ];

  const products = [
    { id: '1', name: 'Смартфон', category: '1', price: 25000 },
    { id: '2', name: 'Ноутбук', category: '1', price: 75000 },
    { id: '3', name: 'Футболка', category: '2', price: 1500 },
    { id: '4', name: 'Джинсы', category: '2', price: 3000 },
    { id: '5', name: 'JavaScript для начинающих', category: '3', price: 1200 },
    { id: '6', name: 'React для профессионалов', category: '3', price: 2500 },
  ];

  // Фильтрация товаров по выбранной категории
  const filteredProducts = selectedCategory
    ? products.filter((product) => product.category === selectedCategory)
    : products;

  return (
    <Container fluid>
      <Row>
        {/* Сайдбар с категориями */}
        <Col md={3} className="bg-light p-4">
          <h4>Категории</h4>
          <ListGroup>
            {categories.map((category) => (
              <ListGroup.Item
                key={category.id}
                action
                active={selectedCategory === category.id}
                onClick={() => setSelectedCategory(category.id)}
              >
                {category.name}
              </ListGroup.Item>
            ))}
          </ListGroup>
          <Button
            variant="secondary"
            className="mt-3"
            onClick={() => setSelectedCategory(null)}
          >
            Сбросить фильтр
          </Button>
        </Col>

        {/* Каталог товаров */}
        <Col md={9} className="p-4">
          <h2>Каталог товаров</h2>
          <Row>
            {filteredProducts.map((product) => (
              <Col md={4} key={product.id} className="mb-4">
                <Card>
                  <Card.Body>
                    <Card.Title>{product.name}</Card.Title>
                    <Card.Text>
                      Цена: {product.price} руб.
                    </Card.Text>
                    <Button variant="primary">Купить</Button>
                  </Card.Body>
                </Card>
              </Col>
            ))}
          </Row>
        </Col>
      </Row>
    </Container>
  );
};

export default ProductsCatalog;
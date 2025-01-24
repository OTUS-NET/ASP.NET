import React from 'react';
import { Container, Row, Col, Card, Table, Nav } from 'react-bootstrap';

const Dashboard: React.FC = () => {
  return (
    <Container fluid>
      {/* Навигация */}
      <Nav className="bg-light mb-4 p-3">
        <Nav.Item>
          <Nav.Link href="#home">Главная</Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link href="#reports">Отчеты</Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link href="#settings">Настройки</Nav.Link>
        </Nav.Item>
      </Nav>

      {/* Карточки с метриками */}
      <Row className="mb-4">
        <Col md={4}>
          <Card>
            <Card.Body>
              <Card.Title>Пользователи</Card.Title>
              <Card.Text>
                1,234
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card>
            <Card.Body>
              <Card.Title>Заказы</Card.Title>
              <Card.Text>
                567
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card>
            <Card.Body>
              <Card.Title>Доход</Card.Title>
              <Card.Text>
                $12,345
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Таблица с данными */}
      <Row>
        <Col>
          <Card>
            <Card.Body>
              <Card.Title>Последние заказы</Card.Title>
              <Table striped bordered hover>
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Имя</th>
                    <th>Email</th>
                    <th>Статус</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>1</td>
                    <td>Иван Иванов</td>
                    <td>ivan@example.com</td>
                    <td>Завершен</td>
                  </tr>
                  <tr>
                    <td>2</td>
                    <td>Мария Петрова</td>
                    <td>maria@example.com</td>
                    <td>В процессе</td>
                  </tr>
                  <tr>
                    <td>3</td>
                    <td>Алексей Сидоров</td>
                    <td>alex@example.com</td>
                    <td>Отменен</td>
                  </tr>
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Dashboard;
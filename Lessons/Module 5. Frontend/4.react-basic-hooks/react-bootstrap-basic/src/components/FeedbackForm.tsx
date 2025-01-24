import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';

const FeedbackForm: React.FC = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Здесь можно добавить логику отправки данных
    setSubmitted(true);
  };

  return (
    <div className="container mt-5">
      <h2>Форма обратной связи</h2>
      {submitted && (
        <Alert variant="success">Спасибо за ваш отзыв!</Alert>
      )}
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3" controlId="formName">
          <Form.Label>Имя</Form.Label>
          <Form.Control
            type="text"
            placeholder="Введите ваше имя"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formEmail">
          <Form.Label>Email</Form.Label>
          <Form.Control
            type="email"
            placeholder="Введите ваш email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formMessage">
          <Form.Label>Сообщение</Form.Label>
          <Form.Control
            as="textarea"
            rows={3}
            placeholder="Введите ваше сообщение"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            required
          />
        </Form.Group>

        <Button variant="primary" type="submit">
          Отправить
        </Button>
      </Form>
    </div>
  );
};

export default FeedbackForm;
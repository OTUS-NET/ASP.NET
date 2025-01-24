import React, { useState } from 'react';
import { Form, Button, Alert, Card } from 'react-bootstrap';
import apiClient from '../Api/Client'; // Импортируем настроенный axios


const FeedbackForm: React.FC = () => {
    const [name, setName] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [message, setMessage] = useState<string>('');
    const [submitted, setSubmitted] = useState<boolean>(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await apiClient.post<string>('/feedback', {
                name,
                email,
                message
            });
            console.log(response.data);
            setSubmitted(true);
        } catch (error) {
            console.error('Ошибка при отправке формы:', error);
        }
    };

    return (
        <Card>
            <Card.Header>Форма обратной связи</Card.Header>
            <Card.Body>
                {submitted ? (
                    <Alert variant="success">Спасибо за ваш отзыв!</Alert>
                ) : (
                    <Form onSubmit={handleSubmit}>
                        <Form.Group className="mb-3">
                            <Form.Label>Имя:</Form.Label>
                            <Form.Control
                                type="text"
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                                required
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Email:</Form.Label>
                            <Form.Control
                                type="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Сообщение:</Form.Label>
                            <Form.Control
                                as="textarea"
                                rows={3}
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                required
                            />
                        </Form.Group>
                        <Button variant="primary" type="submit">
                            Отправить
                        </Button>
                    </Form>
                )}
            </Card.Body>
        </Card>
    );
};

export default FeedbackForm;
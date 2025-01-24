import React, { useState, useEffect } from 'react';
import { Card, Button } from 'react-bootstrap';
import apiClient from '../Api/Client'; // Импортируем настроенный axios

const Counter: React.FC = () => {
    const [count, setCount] = useState<number>(0);

    const fetchCounter = async () => {
        try {
            const response = await apiClient.get<number>('/counter');
             // пример с использованием fetch
             /* const response2 = await fetch('/api/counter');
             const data = await response2.json();
             setCount(data); */
            setCount(response.data);
        } catch (error) {
            console.error('Ошибка при загрузке счетчика:', error);
        }
    };

    const incrementCounter = async () => {
        try {
            const response = await apiClient.post<number>('/counter/increment');
            setCount(response.data);
        } catch (error) {
            console.error('Ошибка при увеличении счетчика:', error);
        }
    };

    const decrementCounter = async () => {
        try {
            const response = await apiClient.post<number>('/counter/decrement');
            setCount(response.data);
        } catch (error) {
            console.error('Ошибка при уменьшении счетчика:', error);
        }
    };

    const resetCounter = async () => {
        try {
            const response = await apiClient.post<number>('/counter/reset');
            setCount(response.data);
        } catch (error) {
            console.error('Ошибка при сбросе счетчика:', error);
        }
    };

    useEffect(() => {
        fetchCounter();
    }, []);

    return (
        <Card className="text-center">
            <Card.Header>Счетчик</Card.Header>
            <Card.Body>
                <Card.Title>Текущее значение: {count}</Card.Title>
                <Button variant="success" onClick={incrementCounter} className="m-2">
                    Увеличить
                </Button>
                <Button variant="warning" onClick={decrementCounter} className="m-2">
                    Уменьшить
                </Button>
                <Button variant="danger" onClick={resetCounter} className="m-2">
                    Сбросить
                </Button>
            </Card.Body>
        </Card>
    );
};

export default Counter;
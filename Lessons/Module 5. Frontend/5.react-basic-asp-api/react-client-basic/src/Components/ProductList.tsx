import React, { useState, useEffect } from 'react';
import { Card, ListGroup } from 'react-bootstrap';
import apiClient from '../Api/Client'; // Импортируем настроенный axios

interface Product {
    id: number;
    name: string;
    price: number;
}

const ProductList: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await apiClient.get<Product[]>('/product');
                setProducts(response.data);
            } catch (error) {
                console.error('Ошибка при загрузке товаров:', error);
            }
        };

        fetchProducts();
    }, []);

    return (
        <Card>
            <Card.Header>Каталог товаров</Card.Header>
            <Card.Body>
                <ListGroup>
                    {products.map(product => (
                        <ListGroup.Item key={product.id}>
                            <h3>{product.name}</h3>
                            <p>Цена: {product.price} руб.</p>
                        </ListGroup.Item>
                    ))}
                </ListGroup>
            </Card.Body>
        </Card>
    );
};

export default ProductList;
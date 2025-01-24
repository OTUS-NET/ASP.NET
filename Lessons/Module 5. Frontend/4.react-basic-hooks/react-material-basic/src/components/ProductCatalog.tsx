import React, { useState } from 'react';
import { Box, Typography, Grid, Select, MenuItem, FormControl, InputLabel } from '@mui/material';

interface Product {
  id: number;
  name: string;
  category: string;
}

const products: Product[] = [
  { id: 1, name: 'Товар 1', category: 'Категория 1' },
  { id: 2, name: 'Товар 2', category: 'Категория 2' },
  { id: 3, name: 'Товар 3', category: 'Категория 1' },
  { id: 4, name: 'Товар 4', category: 'Категория 3' },
];

const ProductCatalog: React.FC = () => {
  const [category, setCategory] = useState('');

  const filteredProducts = category
    ? products.filter((product) => product.category === category)
    : products;

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Каталог товаров
      </Typography>
      <FormControl fullWidth sx={{ mb: 3 }}>
        <InputLabel>Категория</InputLabel>
        <Select value={category} onChange={(e) => setCategory(e.target.value as string)}>
          <MenuItem value="">Все категории</MenuItem>
          <MenuItem value="Категория 1">Категория 1</MenuItem>
          <MenuItem value="Категория 2">Категория 2</MenuItem>
          <MenuItem value="Категория 3">Категория 3</MenuItem>
        </Select>
      </FormControl>
      <Grid container spacing={3}>
        {filteredProducts.map((product) => (
          <Grid item xs={12} sm={6} md={4} key={product.id}>
            <Box sx={{ p: 2, border: '1px solid #ddd', borderRadius: 1 }}>
              <Typography variant="h6">{product.name}</Typography>
              <Typography variant="body2">{product.category}</Typography>
            </Box>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default ProductCatalog;
import React, { useState } from 'react';
import { TextField, Button, Box, Typography } from '@mui/material';

const FeedbackForm: React.FC = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log({ name, email, message });
    // Здесь можно добавить логику отправки формы
  };

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <Box component="form" onSubmit={handleSubmit} sx={{ maxWidth: 600, width: '100%', mt: 4 }}>
        <Typography variant="h5" gutterBottom>
          Обратная связь
        </Typography>
        <TextField
          label="Имя"
          fullWidth
          margin="normal"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <TextField
          label="Email"
          fullWidth
          margin="normal"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <TextField
          label="Сообщение"
          fullWidth
          margin="normal"
          multiline
          rows={4}
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />
        <Button type="submit" variant="contained" color="primary" sx={{ mt: 2 }}>
          Отправить
        </Button>
      </Box>
    </Box>
  );
};

export default FeedbackForm;
import React from 'react';
import { Box, Typography, Paper, Grid, Button, ButtonGroup, FormControl, InputLabel, MenuItem, Select } from '@mui/material';

const CRMDashboard: React.FC = () => {
  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Дашборд CRM
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Статистика продаж</Typography>
            <Typography variant="body1">Графики и метрики продаж</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Последние заказы</Typography>
            <Typography variant="body1">Список последних заказов</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Аналитика</Typography>
            <Typography variant="body1">Детальная аналитика по клиентам и продуктам</Typography>
          </Paper>
        </Grid>
      </Grid>
      <FormControl fullWidth>
  <InputLabel id="demo-simple-select-label">Age</InputLabel>
  <Select
    labelId="demo-simple-select-label"
    id="demo-simple-select"
    label="Age"
    
  >
    <MenuItem value={10}>Ten</MenuItem>
    <MenuItem value={20}>Twenty</MenuItem>
    <MenuItem value={30}>Thirty</MenuItem>
  </Select>
</FormControl>
    </Box>
  );
};

export default CRMDashboard;
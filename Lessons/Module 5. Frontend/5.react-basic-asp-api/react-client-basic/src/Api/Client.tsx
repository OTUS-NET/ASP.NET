import axios from 'axios';

// Создаем экземпляр axios с базовым URL
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5687/api", // Базовый URL вашего API
  headers: {
    'Content-Type': 'application/json',
    //'Secret-Token': '123123213213' // нестандартный заголовок вызовет preflight запросы к вашему API
  },
});

export default apiClient;
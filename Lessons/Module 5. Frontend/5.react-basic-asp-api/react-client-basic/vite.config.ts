import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  base: "/",
  plugins: [react()],
  preview: {
    port: 5272,
    strictPort: true,
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `@import "~bootstrap/scss/bootstrap";`,
      },
    },
  },
  build: {
    outDir: path.resolve(__dirname, 'ClientApp'), // Указываем целевую директорию
    emptyOutDir: true, // Очищать целевую директорию перед сборкой
  },
  server: {
    port: 5272, // Порт для разработки
    strictPort: true, // Запретить переадресацию на другой порт
    host: true, // Разрешать доступ к серверу с любых IP-адресов
    origin: "http://localhost:5272",
  },
})

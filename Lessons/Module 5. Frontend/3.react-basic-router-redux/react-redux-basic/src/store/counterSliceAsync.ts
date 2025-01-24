import { createSlice, PayloadAction, createAsyncThunk } from '@reduxjs/toolkit';

// Определяем интерфейс для состояния счетчика
interface CounterStateAsync {
  value: number;
  status: 'idle' | 'loading' | 'succeeded' | 'failed'; // Состояние загрузки
  error: string | null; // Ошибка, если она есть
}

// Начальное состояние счетчика
const initialState: CounterStateAsync = {
  value: 0,
  status: 'idle',
  error: null,
};

// Создаем асинхронное действие (Thunk) с использованием setTimeout
export const fetchRandomNumber = createAsyncThunk(
  'counter/fetchRandomNumber', // Уникальное имя действия
  async (_, { rejectWithValue }) => {
    try {
      // Имитируем задержку в 2 секунды
      await new Promise((resolve) => setTimeout(resolve, 2000));

      // Генерируем случайное число от 1 до 100
      const randomNumber = Math.floor(Math.random() * 100) + 1;
      return randomNumber; // Возвращаем случайное число
    } catch (error) {
      // В случае ошибки возвращаем её с помощью rejectWithValue
      return rejectWithValue('Не удалось загрузить случайное число');
    }
  }
);

// Создаем слайс (slice) для счетчика
const counterSliceAsync = createSlice({
  name: 'counter',
  initialState,
  reducers: {
    increment: (state) => {
      state.value += 1;
    },
    decrement: (state) => {
      state.value -= 1;
    },
    incrementByAmount: (state, action: PayloadAction<number>) => {
      state.value += action.payload;
    },
  },
  // Обрабатываем дополнительные действия (extraReducers)
  // extraReducers — это объект, который содержит обработчики действий, созданных с помощью createAsyncThunk
  extraReducers: (builder) => {
    builder
      // Загрузка началась
      .addCase(fetchRandomNumber.pending, (state) => {
        state.status = 'loading';
        state.error = null; // Сбрасываем ошибку
      })
      // Загрузка завершена успешно
      .addCase(fetchRandomNumber.fulfilled, (state, action: PayloadAction<number>) => {
        state.status = 'succeeded';
        state.value = action.payload; // Обновляем значение счетчика
      })
      // Загрузка завершена с ошибкой
      .addCase(fetchRandomNumber.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload as string; // Сохраняем ошибку
      });
  },
});

// Экспортируем действия (actions)
export const { increment, decrement, incrementByAmount } = counterSliceAsync.actions;

// Экспортируем редьюсер
export default counterSliceAsync.reducer;
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import axios from 'axios';

interface CurrencyState {
  rates: { [key: string]: number };
  baseCurrency: string; 
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: CurrencyState = {
  rates: {},
  baseCurrency: 'USD', // Начальная основная валюта
  status: 'idle',
  error: null,
};

// Асинхронный thunk для получения курсов валют
export const fetchCurrencyRates = createAsyncThunk(
  'currency/fetchCurrencyRates',
  async (baseCurrency: string) => {
    const response = await axios.get(`https://api.exchangerate-api.com/v4/latest/${baseCurrency}`);
    const data = response.data as { rates: { [key: string]: number } };
    return { rates: data.rates, baseCurrency };
  }
);

const currencySlice = createSlice({
  name: 'currency',
  initialState,
  reducers: {
    // редьюсер для изменения основной валюты
    setBaseCurrency: (state, action: PayloadAction<string>) => {
      state.baseCurrency = action.payload;
    },
  },
  // Добавляем обработчики для асинхронного thunk
  extraReducers: (builder) => {
    builder
    // Обработчики для состояния загрузки
      .addCase(fetchCurrencyRates.pending, (state) => {
        state.status = 'loading';
      })
      // Обработчики для успешного получения данных
      .addCase(fetchCurrencyRates.fulfilled, (state, action: PayloadAction<{ rates: { [key: string]: number }, baseCurrency: string }>) => {
        state.status = 'succeeded';
        state.rates = action.payload.rates;
        state.baseCurrency = action.payload.baseCurrency;
      })
      // Обработчики для ошибки
      .addCase(fetchCurrencyRates.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Something went wrong';
      });
  },
});

export const { setBaseCurrency } = currencySlice.actions; // Экспортируем действие
export default currencySlice.reducer;
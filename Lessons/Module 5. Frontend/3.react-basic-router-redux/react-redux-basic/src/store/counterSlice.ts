import { createSlice, PayloadAction } from '@reduxjs/toolkit'; // Импортируем функцию createSlice и тип PayloadAction из библиотеки @reduxjs/toolkit

// CounterState — это тип, который представляет состояние счетчика
interface CounterState {
  value: number;
}

// initialState — это начальное состояние счетчика
const initialState: CounterState = {
  value: 0,
};

//Слайс — это часть хранилища, которая содержит редьюсер и действия
//createSlice — это функция, которая создает слайс
// name — это имя слайса
// initialState — это начальное состояние слайса
// reducers — это объект, который содержит редьюсеры
// increment — это редьюсер, который увеличивает значение счетчика на 1
// decrement — это редьюсер, который уменьшает значение счетчика на 1
// incrementByAmount — это редьюсер, который увеличивает значение счетчика на заданное число
// PayloadAction — это тип, который представляет действие с данными
// action.payload — это данные действия
const counterSlice = createSlice({
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
});

export const { increment, decrement, incrementByAmount } = counterSlice.actions; // Экспортируем действия из слайса
export default counterSlice.reducer; // Экспортируем редьюсер из слайса
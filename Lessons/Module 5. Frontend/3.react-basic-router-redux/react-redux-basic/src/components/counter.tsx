import React from 'react';
import { useSelector, useDispatch } from 'react-redux'; // Импортируем хуки useSelector и useDispatch из библиотеки react-redux. Хук - это функция, которая позволяет вам использовать состояние и другие возможности React без написания классов
import { RootState } from '../store/store'; // Импортируем тип RootState из хранилища
import { increment, decrement, incrementByAmount } from '../store/counterSlice'; // Импортируем действия increment, decrement и incrementByAmount из слайса

const Counter: React.FC = () => {
  const count = useSelector((state: RootState) => state.counter.value); // Получаем значение счетчика из хранилища, хук useSelector позволяет получить доступ к состоянию хранилища и выбрать из него нужные данные
  const dispatch = useDispatch(); // Хук useDispatch позволяет получить доступ к функции dispatch, которая отправляет действия в хранилище

  return (
    <div>
      <h1>{count}</h1>
      <button onClick={() => dispatch(increment())}>Увеличить</button>
      <button onClick={() => dispatch(decrement())}>Уменьшить</button>
      <button onClick={() => dispatch(incrementByAmount(5))}>Увеличить на 5</button>
    </div>
  );
};

export default Counter;
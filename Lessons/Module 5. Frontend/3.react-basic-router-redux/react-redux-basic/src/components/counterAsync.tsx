import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { RootState, AppDispatch } from '../store/store';
import { increment, decrement, incrementByAmount, fetchRandomNumber } from '../store/counterSliceAsync';

const Counter: React.FC = () => {
  const count = useSelector((state: RootState) => state.counterAsync.value);
  const status = useSelector((state: RootState) => state.counterAsync.status);
  const error = useSelector((state: RootState) => state.counterAsync.error);
  const dispatch: AppDispatch = useDispatch();
 
  return (
    <div>
      <h1>Счетчик: {count}</h1>
      {/* Отображаем состояние загрузки */}
      {status === 'loading' && <p>Загрузка...</p>}
      {/* Отображаем ошибку, если она есть */}
      {status === 'failed' && <p style={{ color: 'red' }}>{error}</p>}
      <button onClick={() => dispatch(increment())}>Увеличить</button>
      <button onClick={() => dispatch(decrement())}>Уменьшить</button>
      <button onClick={() => dispatch(incrementByAmount(5))}>Увеличить на 5</button>
      {/* Кнопка для повторной загрузки случайного числа */}
      <button onClick={() => dispatch(fetchRandomNumber())}>Загрузить случайное число (Async)</button>
    </div>
  );
};

export default Counter;
import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { RootState, AppDispatch } from '../store/store';
import { increment, decrement, incrementByAmount, fetchRandomNumber } from '../store/counterSliceAsync';

const Counter: React.FC = () => {
  const count = useSelector((state: RootState) => state.counter.value);
  const status = useSelector((state: RootState) => state.counter.status);
  const error = useSelector((state: RootState) => state.counter.error);
  const dispatch: AppDispatch = useDispatch();

  // Загружаем случайное число при монтировании компонента с помощью useEffect
  useEffect(() => {
    dispatch(fetchRandomNumber());
  }, [dispatch]);

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
      <button onClick={() => dispatch(fetchRandomNumber())}>Загрузить случайное число</button>
    </div>
  );
};

export default Counter;
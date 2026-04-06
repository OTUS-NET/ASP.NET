// CurrencyRates.tsx
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchCurrencyRates, setBaseCurrency } from '../store/currencySlice';
import { RootState, AppDispatch } from '../store/store';

const CurrencyRates: React.FC = () => {
  const dispatch: AppDispatch = useDispatch();
  const { rates, baseCurrency, status, error } = useSelector((state: RootState) => state.currency);
  const [selectedCurrency, setSelectedCurrency] = useState(baseCurrency);

  useEffect(() => {
    if (status === 'idle') {
      dispatch(fetchCurrencyRates(baseCurrency));
    }
  }, [status, baseCurrency, dispatch]);

  const handleCurrencyChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const newCurrency = event.target.value;
    setSelectedCurrency(newCurrency);
    dispatch(setBaseCurrency(newCurrency));
    dispatch(fetchCurrencyRates(newCurrency));
  };

  if (status === 'loading') {
    return <div>Загрузка...</div>;
  }

  if (status === 'failed') {
    return <div>Ошибка: {error}</div>;
  }

  return (
    <div>
      <h1>Курсы валют</h1>
      <div>
        <label htmlFor="baseCurrency">Валюта: </label>
        <select id="baseCurrency" value={selectedCurrency} onChange={handleCurrencyChange}>
          {Object.keys(rates).map((currency) => (
            <option key={currency} value={currency}>
              {currency}
            </option>
          ))}
        </select>
      </div>
      <ul>
        {Object.entries(rates).map(([currency, rate]) => (
          <li key={currency}>
            {currency}: {rate}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default CurrencyRates;
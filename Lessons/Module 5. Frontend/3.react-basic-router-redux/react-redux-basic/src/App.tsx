import React from 'react';
import Counter from './components/counter';
import CounterAsync from './components/counterAsync';
import CurrencyRates from './components/currencyRatesAdvanced';

const App: React.FC = () => {
  return (
    <div>
      <h1>Redux + React</h1>
      <Counter />
     {/*  <CounterAsync /> */}
     {/*  <CurrencyRates /> */}
    </div>
  );
};

export default App;
import React from 'react';
import Counter from './components/counter';
import CounterAsync from './components/counterAsync';

const App: React.FC = () => {
  return (
    <div>
      <h1>Redux + React</h1>
      {/* <Counter /> */}
      <CounterAsync />
    </div>
  );
};

export default App;
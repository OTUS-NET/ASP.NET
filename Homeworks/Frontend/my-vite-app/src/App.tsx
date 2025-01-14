import { useState } from 'react';
import CatFact from './components/CatFact';
import ErrorComponent from './components/ErrorComponent';
import GetCatFactButton from './components/GetCatFactButton';

function App() {
  const [fact, setFact] = useState('');
  const [error, setError] = useState('');

  const handleFactReceived = (fact: string) => {
    setFact(fact);
    setError('');
  };

  const handleError = (error: string) => {
    setError(error);
    setFact('');
  };

  return (
    <div>
      <div>
        <GetCatFactButton btnName='Получить факт о кошках' url='https://cat-fact.herokuapp.com/facts/' onFactReceived={handleFactReceived} onError={handleError} />
      </div>
      <div>
        <GetCatFactButton btnName='Получить ошибку' url='https://cat-fact.herokuapp.com/facts/1' onFactReceived={handleFactReceived} onError={handleError} />
      </div>
      <div>
        {fact && <CatFact fact={fact} />}
        {error && <ErrorComponent error={error} />}
      </div>
    </div>
  );
}

export default App;

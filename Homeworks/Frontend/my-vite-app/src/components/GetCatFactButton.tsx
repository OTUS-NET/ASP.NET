import React, { useState } from 'react';
import axios from 'axios';
import CatFact from './CatFact';
import ErrorComponent from './ErrorComponent';

interface GetCatFactButtonProps {
  btnName: string;
  url: string;
}

const GetCatFactButton: React.FC<GetCatFactButtonProps> = ({ btnName, url }) => {
  const [loading, setLoading] = useState(false);
  const [fact, setFact] = useState('');
  const [error, setError] = useState('');

  const handleButtonClick = async () => {
    setLoading(true);
    try {
      const response = await axios.get(url);
      if (response.status >= 200 && response.status < 300) {
        setFact(response.data[0].text);
      } else {
        throw new Error('Status error: '.concat(response.status.toString(), ' ', response.statusText));
      }
    } catch (ex) {
      setError(ex.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <button onClick={handleButtonClick} disabled={loading}>
        {loading? 'Загрузка...' : btnName}
      </button>
      <div>
        {fact && <CatFact fact={fact} />}
        {error && <ErrorComponent error={error} />}
      </div>
    </div>
  );
};

export default GetCatFactButton;
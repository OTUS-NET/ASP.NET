import React, { useState } from 'react';
import axios from 'axios';

interface GetCatFactButtonProps {
  btnName: string;
  url: string;
  onFactReceived: (fact: string) => void;
  onError: (error: string) => void;
}

const GetCatFactButton: React.FC<GetCatFactButtonProps> = ({ btnName, url, onFactReceived, onError }) => {
  const [loading, setLoading] = useState(false);

  const handleButtonClick = async () => {
    setLoading(true);
    try {
      const response = await axios.get(url);
      if (response.status >= 200 && response.status < 300) {
        onFactReceived(response.data[0].text);
      } else {
        throw new Error('Status error: '.concat(response.status.toString(), ' ', response.statusText));
      }
    } catch (ex) {
      onError(ex.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <button onClick={handleButtonClick} disabled={loading}>
      {loading? 'Загрузка...' : btnName}
    </button>
  );
};

export default GetCatFactButton;
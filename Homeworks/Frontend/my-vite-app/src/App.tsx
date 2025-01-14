import GetCatFactButton from './components/GetCatFactButton';
import './App.css';

function App() {
  return (
    <div>
      <div>
        <GetCatFactButton btnName='Получить факт о кошках' url='https://cat-fact.herokuapp.com/facts/' />
      </div>
      <div>
        <GetCatFactButton btnName='Получить ошибку' url='https://cat-fact.herokuapp.com/facts/1' />
      </div>
    </div>
  );
}

export default App;

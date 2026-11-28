import CatFactFetcher from './components/CatFactFetcher'
import './App.css'

function App() {
  return (
    <main className="page">
      <section className="panel" aria-labelledby="page-title">
        <p className="eyebrow">Маленький факт дня</p>
        <h1 id="page-title">Мир кошек</h1>
        <p className="intro">
          Узнайте что-нибудь новое о кошках — нажмите на кнопку, чтобы получить случайный факт.
        </p>
        <CatFactFetcher />
      </section>
      <footer className="footer">Данные предоставляет Cat Fact API</footer>
    </main>
  )
}

export default App
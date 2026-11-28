import { useState } from 'react'
import axios from 'axios'
import CatFactCard from './CatFactCard'
import ErrorMessage from './ErrorMessage'

interface CatFactResponse {
  fact: string
  length: number
}

function CatFactFetcher() {
  const [fact, setFact] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  async function loadFact() {
    setIsLoading(true)
    setFact(null)
    setError(null)

    try {
      const response = await axios.get<CatFactResponse>('https://catfact.ninja/fact')

      if (!response.data.fact?.trim()) {
        throw new Error('Сервис вернул пустой факт. Попробуйте ещё раз.')
      }

      setFact(response.data.fact)
    } catch (requestError) {
      if (axios.isAxiosError(requestError)) {
        const status = requestError.response?.status
        setError(
          status
            ? `Сервис ответил с ошибкой (${status}). Попробуйте ещё раз.`
            : 'Не удалось связаться с сервисом. Проверьте подключение к интернету и попробуйте ещё раз.',
        )
      } else if (requestError instanceof Error) {
        setError(requestError.message)
      } else {
        setError('Произошла неизвестная ошибка. Попробуйте ещё раз.')
      }
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="fact-fetcher">
      <button className="fetch-button" type="button" onClick={loadFact} disabled={isLoading}>
        {isLoading ? 'Загружаем факт…' : 'Показать факт'}
      </button>
      {fact && <CatFactCard fact={fact} />}
      {error && <ErrorMessage message={error} />}
    </div>
  )
}

export default CatFactFetcher
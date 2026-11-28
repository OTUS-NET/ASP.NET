interface CatFactCardProps {
  fact: string
}

function CatFactCard({ fact }: CatFactCardProps) {
  return (
    <section className="message message--success" role="status" aria-live="polite">
      <h2>Интересный факт</h2>
      <p>{fact}</p>
    </section>
  )
}

export default CatFactCard
interface ErrorMessageProps {
  message: string
}

function ErrorMessage({ message }: ErrorMessageProps) {
  return (
    <section className="message message--error" role="alert">
      <h2>Не удалось загрузить факт</h2>
      <p>{message}</p>
    </section>
  )
}

export default ErrorMessage
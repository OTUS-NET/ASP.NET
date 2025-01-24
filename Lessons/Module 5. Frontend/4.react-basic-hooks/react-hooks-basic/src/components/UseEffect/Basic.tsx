import { useEffect, useState } from "react"

//Базовый пример
const UseEffectBasic = () => {
  
    const [seconds, setSeconds] = useState(0);
 
    useEffect(() => {
      // Устанавливаем интервал для обновления состояния каждую секунду (аналог componentDidMount)
      const interval = setInterval(() => {
        setSeconds(seconds => seconds + 1);
      }, 1000);
  
      // Очищаем интервал при размонтировании компонента (аналог  componentWillUnmount)
      return () => clearInterval(interval);
    }, []); // Пустой массив зависимостей означает, что эффект выполнится только один раз
  
    return <p>Прошло секунд: {seconds}</p>;
  
  }


  export default UseEffectBasic
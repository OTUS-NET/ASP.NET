import { memo, useCallback, useState } from "react"

// 
const Age = ({ age, handleClick }) => {
  return (
    <div>
      <p>Мне {age} лет.</p>
      <p>Нажми на кнопку 👇</p>
      <button onClick={handleClick}>Стать старше!</button>
    </div>
  )
}

// `React.memo()` позволяет зафиксировать состояние компонента до изменения его props
const Guide = memo(({ getRandomColor }) => {

  const color = getRandomColor()
  console.log("Guide was rendered")
  return (
    <div style={{ background: color, padding: '.4rem' }}>
      <p style={{ color: color, filter: 'invert()' }}>
        Изучай хуки внимательно!
      </p>
    </div>
  )
})

const UseCallbackBasic = () => {
  const [age, setAge] = useState(20)
  const handleClick = () => { setAge(age < 100 ? age + 10 : age) }

  const getRandomColor = useCallback(
    () => {

      console.log('getRandomColor executed');
      return `#${((Math.random() * 0xfff) << 0).toString(16)}`;
    },
    //[age] // dependencies отсутствуют - вызыв функции связанной с useCallback при каждом рендеринге (изменение age)
    [] // dependencies отсутствуют - вызыв функции связанной с useCallback при первом рендеринге
  )

  return (
    <>
      <Age age={age} handleClick={handleClick} />
      <Guide getRandomColor={getRandomColor} />
    </>
  )
}



export default UseCallbackBasic;
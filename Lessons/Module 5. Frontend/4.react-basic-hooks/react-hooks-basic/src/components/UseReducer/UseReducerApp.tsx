import { useEffect, useState, useReducer, Dispatch } from "react";

/* Хук useReducer  используется для управления сложным состоянием */

// Определяем типы для действий
type Action = 
  | { type: "set"; text: string } // Действие для установки текста
  | { type: "setToGlobalState" }  // Действие для установки значения из глобального состояния
  | { type: "asdasdzx" }; // тест ошибки в reducer

// Тип для функции dispatch, которая принимает действие типа Action
type DispatchType = Dispatch<Action>;

// Пропсы для компонента MyControls
interface MyControlsProps {
  dispatch: DispatchType; // Функция dispatch для отправки действий
}

// Компонент MyControls, который отображает кнопки и обрабатывает события
function MyControls({ dispatch }: MyControlsProps) {
  // Эффект для добавления обработчика события resize
  useEffect(() => {
    function onResize() {
      // При изменении размера окна отправляем действие "set" с текстом "Resize"
      dispatch({ type: "set", text: "Resize" });
    }

    // Добавляем обработчик события resize
    window.addEventListener("resize", onResize);
    // Очищаем обработчик при размонтировании компонента (return)
    return () => window.removeEventListener("resize", onResize);
  }, [dispatch]); // Зависимость от dispatch, чтобы эффект не пересоздавался

  return (
    <>
      {/* Кнопка вызывающая dispatch для установки текста "ABC" */}
      <button onClick={() => dispatch({ type: "set", text: "ABC" })}>
        Set to "ABC"
      </button>
      {/* Кнопка для установки значения из глобального состояния */}
      <button onClick={() => dispatch({ type: "setToGlobalState" })}>
        Set to globalAppState
      </button>
      {/* Подсказка для пользователя */}
      <div>Resize to set to "Resized"</div>
      {/* Вызов несуществующего типа экшна */}
      <button onClick={() => dispatch({ type: "asdasdzx" })}>
        Set wrong type
      </button>
      {/* Подсказка для пользователя */}
      <div>Error on wrong acton.type</div>
    </>
  );
}

// Пропсы для компонента MyComponent
interface MyComponentProps {
  globalAppState: string; // Глобальное состояние, передаваемое из App
}

// Компонент MyComponent, который управляет состоянием headlineText
function MyComponent({ globalAppState }: MyComponentProps) {
  // Используем useReducer для управления состоянием headlineText
  const [headlineText, dispatch] = useReducer(reducer, "ABC");

  // Редуктор для обработки действий
  function reducer(state: string, action: Action): string {
    switch (action.type) {
      case "set":
        console.log("reduer.set action executed")
        // Устанавливаем новое значение текста
        return action.text;
      case "setToGlobalState":
        console.log("reduer.setToGlobalState action executed")
        // Устанавливаем значение из глобального состояния
        return globalAppState;
      default:
        // Обработка неизвестного действия
        throw new Error("Unknown action type");
    }
  }

  return (
    <div>
      {/* Отображаем текущее значение headlineText */}
      <h1>{headlineText}</h1>
      {/* Передаем dispatch в компонент MyControls */}
      <MyControls dispatch={dispatch} />
    </div>
  );
}

// Основной компонент App
export default function UseReducerApp() {
  // Состояние для глобального состояния приложения
  const [globalAppState, setGlobalAppState] = useState<string>("");

  return (
    <div>
      {/* Поле ввода для изменения глобального состояния */}
      global app state:{" "}
      <input
        value={globalAppState}
        onChange={(e) => setGlobalAppState(e.target.value)}
      />
      {/* Передаем глобальное состояние в MyComponent */}
      <MyComponent globalAppState={globalAppState} />
    </div>
  );
}
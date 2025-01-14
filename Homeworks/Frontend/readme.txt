-- Создать 
> npx create-react-app my-app







// -- // -- // 

Ты в роли фронтэнд разработчика. Необходимо создать сайт React на windows 10.

создать проект с помощью команды create-react-app;
Добавить компонент, делающий запрос к API.
На компоненте кнопка, при нажатии на которую проходит вызов API (например, https://api.agify.io/?name=Denis)
данные от запроса отображаются на форме в отдельной компоненте (например, плашка с зеленым фоном и текстом)
Задача со звездочкой: Кроме обычной компоненты, есть еще и обработка ошибок: если с API пришел код ответа, отличный от 2**, выводить отдельный компонент с красным фоном и текстом ошибки. Отображать только в случае ошибок
В результате должна быть пошаговая инструкция с кодом и командами для установки дополнительного ПО, при необходимости.

Шаг 1: Создание проекта с помощью create-react-app

Откройте командную строку Windows и выполните следующую команду:

npx create-react-app my-app

Это создаст новый проект React с именем "my-app".

Шаг 2: Переход в папку проекта

Перейдите в папку проекта:

cd my-app

Шаг 3: Установка дополнительного ПО

Для работы с API нам понадобится библиотека axios. Установите ее с помощью команды:

npm install axios

Шаг 4: Создание компонента с запросом к API

Создайте новый файл ApiComponent.js в папке src:

import React, { useState } from 'eact';
import axios from 'axios';

const ApiComponent = () => {
  const [name, setName] = useState('');
  const [age, setAge] = useState(null);
  const [error, setError] = useState(null);

  const handleClick = () => {
    axios.get(`https://api.agify.io/?name=${name}`)
     .then(response => {
        setAge(response.data.age);
        setError(null);
      })
     .catch(error => {
        setError(error.message);
        setAge(null);
      });
  };

  return (
    <div>
      <input type="text" value={name} onChange={e => setName(e.target.value)} placeholder="Введите имя" />
      <button onClick={handleClick}>Получить возраст</button>
      {age!== null && <AgeComponent age={age} />}
      {error!== null && <ErrorComponent error={error} />}
    </div>
  );
};

export default ApiComponent;

Шаг 5: Создание компонента для отображения возраста

Создайте новый файл AgeComponent.js в папке src:

import React from 'eact';

const AgeComponent = ({ age }) => {
  return (
    <div style={{ backgroundColor: 'green', color: 'white', padding: '10px' }}>
      Возраст: {age}
    </div>
  );
};

export default AgeComponent;

Шаг 6: Создание компонента для отображения ошибки

Создайте новый файл ErrorComponent.js в папке src:

import React from 'eact';

const ErrorComponent = ({ error }) => {
  return (
    <div style={{ backgroundColor: 'ed', color: 'white', padding: '10px' }}>
      Ошибка: {error}
    </div>
  );
};

export default ErrorComponent;

Шаг 7: Добавление компонента в App.js

Откройте файл App.js и добавьте следующий код:

import React from 'eact';
import ApiComponent from './ApiComponent';

function App() {
  return (
    <div>
      <ApiComponent />
    </div>
  );
}

export default App;

Шаг 8: Запуск приложения

Запустите приложение с помощью команды:

npm start

Откройте браузер и перейдите по адресу http://localhost:3000. Введите имя в поле ввода и нажмите кнопку. Если все прошло успешно, вы увидите возраст, соответствующий введенному имени. Если произошла ошибка, вы увидите сообщение об ошибке.

Если ни один из компонентов не содержит никакой информации, вернитесь к шагу 1 и повторите процесс.
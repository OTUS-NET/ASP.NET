import { useState } from 'react'
import './App.css'
import ClassComponent from './components/ClassComponent'
import FuncComponent from './components/FuncComponent'
import ScalarPropComponent from './components/ScalarPropComponent'
import JsxComponent from './components/JsxComponent'
import TsxComponent from './components/TsxComponent'
import AlertButtonComponent from './components/AlertButtonComponent'
import ChildComponent from './components/ChildComponent'
import ValidationPropsComponent from './components/ValidationPropsComponent'

function App() {
 const consoleLog = (text:string) =>  {
    console.log(text)
 }
  return (
    <>
      <h1 className="read-the-docs">
        Добро пожаловать в React!
      </h1>
      <AlertButtonComponent />
      <JsxComponent name="React" age={26} />
      <TsxComponent name='TypeScrpt' age={30} />
      <ScalarPropComponent scalarProp={11} logHandler={consoleLog}/>
      <ValidationPropsComponent inputValue={false} />
      <ChildComponent>
          <p>Привет, Children ReactNode!</p>
      </ChildComponent>
    </>
  )
}

export default App

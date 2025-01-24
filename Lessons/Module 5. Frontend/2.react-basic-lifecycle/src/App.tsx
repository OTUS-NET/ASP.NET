import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { StateDemo } from './Components/StateDemo';
import { ContextDemo } from './Components/ContextDemo';
import LifecycleDemo from "./Components/LifecycleDemo";
import { HocDemo } from './Components/HocDemo';
import MyPureComponent from './New/PureComponent';
import HocExample from './New/HocExample';

function App() {

  return <>
    <StateDemo />
    {/* <ContextDemo /> */}
    {/* <LifecycleDemo/> */}
    {/*  <HocDemo/> */}
    {/* <HocExample clickCount={0} /> */}
    {/* <MyPureComponent message='PureComponent' /> */}
  </>;
}

export default App;

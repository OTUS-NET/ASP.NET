//react component with a scalar prop using arrow function syntax

import React from 'react';

//Prop optional by adding ? after the prop name
const ScalarPropComponent: React.FC<{ scalarProp?: number, logHandler:(msg:string)=>{ } }> = 
({ scalarProp, logHandler }) => { // Functional component with a scalar prop using arrow function syntax

  const style = {
    color: scalarProp! > 10 ? 'green' : 'red',
    fontSize: '24px'
  };

  return (
    <>
    <h1 style={style}>Скалярный параметр: {scalarProp}</h1>
    <button onClick={()=>{logHandler(`${scalarProp}`)}}>log</button>
  </>
  );
};

export default ScalarPropComponent;




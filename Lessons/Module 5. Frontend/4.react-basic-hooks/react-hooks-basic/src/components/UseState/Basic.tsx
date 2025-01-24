import { useState } from "react";

const UseStateBasic = () => {
    const [value, valueChange] = useState(0);
   
    return (
      <div>
        {value}<br></br>
        <button onClick={() => valueChange(value + 1)}>
          Увеличить значение на 1
        </button>
      </div>
    );
  };

  export default UseStateBasic;
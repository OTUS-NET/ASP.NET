import {createContext, useContext} from "react";

const MyContext = createContext("without provider");
 
const UseContextBasic = () => {
  return (
    <MyContext.Provider value="Hello, i am External">
      <Intermediate />
    </MyContext.Provider>
  );
};
 
const Intermediate = () => {
  return <Internal />;
};
 
const Internal = () => {
  const context = useContext(MyContext);
 
  return `I am Internal component. I have got the message from External: "${context}"`;
};

export default UseContextBasic;
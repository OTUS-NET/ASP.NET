//a component with border ui around child prop
import React from 'react';
const ChildComponent = ({ children }) => {
  return (
    <div className="border">
      {children}
    </div>
  );
};
export default ChildComponent;



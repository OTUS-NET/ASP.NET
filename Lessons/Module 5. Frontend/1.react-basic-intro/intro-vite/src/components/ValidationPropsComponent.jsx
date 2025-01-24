//a component with a prop that use constraint of propTypes = string, number
// and a default value of 10

import React from 'react';
import PropTypes from 'prop-types'; //importing prop-types

const ValidationPropsComponent = ({ inputValue, isTrue }) => {
  return (
    <div>
        <h1>Значение: {inputValue}</h1>
    </div>
  );
};

ValidationPropsComponent.propTypes = {
  inputValue: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  isTrue: PropTypes.bool
};

ValidationPropsComponent.defaultProps = {
  
  isTrue: false
};

export default ValidationPropsComponent;












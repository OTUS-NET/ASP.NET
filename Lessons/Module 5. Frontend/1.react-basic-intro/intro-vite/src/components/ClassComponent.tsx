//simple react class based component

import { Component } from "react"; // Importing React module

class ClassComponent extends Component { // Class component
    //add props
    constructor(props) {
        super(props); // Calling the constructor of the parent class (React.Component)
        this.state = { message: "Hello from Class Component!" }; // Setting initial state
    }
    //add methods
    handleClick() {
        alert(this.state.message); // Alerting the current state message when button is clicked
    }



    render() { // Render method to render the component in the DOM
        return <h1>Привет, ClassComponent!</h1>;
    }
}

export default ClassComponent; // Exporting the class component


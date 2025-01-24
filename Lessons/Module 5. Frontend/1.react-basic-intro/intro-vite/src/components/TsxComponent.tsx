import { FC } from 'react'; // Importing React module for functional component
 
 interface Props {  // Interface for the props of the component
 name: string;
 age: number;
 }
 
 const TsxComponent: FC<Props> = (props) => { // Functional component using arrow functional syntax

 return <h1>Привет, {props.name}!</h1>;
 };
 
 export default TsxComponent;
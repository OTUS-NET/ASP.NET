//a component with a button and alert functionality
export default function AlertButtonComponent() {
    const showAlert = () => {
        alert("Привет, React!");
    };
    return (
        <button onClick={showAlert}>Нажми кнопку</button>
    );
}
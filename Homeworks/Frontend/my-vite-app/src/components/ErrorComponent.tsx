import React from "react";

interface ErrorProps {
    error: string;
}

const ErrorComponent: React.FC<ErrorProps> = ({ error }) => {
    return (
        <div style={{ backgroundColor: 'red', color: 'white', padding: '10px' }}>
            {error}
        </div>
    );
};

export default ErrorComponent;

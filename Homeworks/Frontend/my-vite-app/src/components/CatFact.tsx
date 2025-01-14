import React from "react";

interface CatFactProps {
    fact: string;
}

const CatFact: React.FC<CatFactProps> = ({ fact }) => {
    return (
        <div style={{ backgroundColor: 'green', color: 'white', padding: '10px' }}>
            {fact}
        </div>
    );
};

export default CatFact;

import { useState } from "react";
import './style.css';

interface Props {
    onChange?: () => void;
}

const UseStateCounter = (props: Props) => {
    let [count, setCount] = useState<number>(0);
    let [texter, setTexter] = useState<number>(0);

    const incr = () => {
        setCount(count + 1);
    }

    const incr2 = () => {

        if (props.onChange) {
            props.onChange();
        }
        setTexter((prev: number) => {
            console.log("Было", prev, "Стало", prev + 1);
            return prev + 1;
        });
    }

    return <>
        <button className="fancyButton3" onClick={incr}>
            У меня useState {count}
        </button>
        <button className="fancyButton2" onClick={incr2}>
            У меня useState2 {texter}
        </button>
    </>;
};

export default UseStateCounter;
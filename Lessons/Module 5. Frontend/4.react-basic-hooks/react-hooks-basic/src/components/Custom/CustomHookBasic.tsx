import { useEffect, useState } from "react";
import './style.css';

// Кастомный хук useCooldown - каждые millisecondes меняется объект
const useCooldown = (millisecondes: number = 1000) => {

    const [time, setTime] = useState<number>(0);

    useEffect(() => {
        const id = setInterval(() => {
            setTime(prev => {
                return prev + millisecondes
            });
        }, millisecondes);

        return () => {
            clearInterval(id);
        }
    }, [millisecondes]);

    return time;
}

// Длина итерации в миллисекундах
const millisecondes = 850;

// Количество итераций изменения длины полоски
const portions = 20;

const CustomHookBasic = () => {

    const cooldown = useCooldown(millisecondes);

    const [width, setWidth] = useState<number>(0);

    // Каждый раз когда меняется cooldown
    // Меняем ширину строки
    useEffect(() => {
        console.log(cooldown);
        const length = 100 * (((cooldown / millisecondes) % portions) / portions);
        setWidth(length);
    }, [cooldown]);

    return <>
        <h1>Loading</h1>
        <div
            className="line"
            style={{
                width: width + '%',
            }} />
    </>;
};
export default CustomHookBasic;
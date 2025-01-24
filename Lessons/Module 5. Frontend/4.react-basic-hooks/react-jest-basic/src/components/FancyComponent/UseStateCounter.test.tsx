
import { fireEvent, render } from '@testing-library/react';
import UseStateCounter from "./UseStateCounter";

describe('useStateCounter общий тест', () => {

    test('renders a message', () => {
        // Здесь мы рендерим наш компонент и проверяем что он отрисовался корректно, 
        // jest.fn() - это функция из библиотеки jest которая позволяет создать мок функцию, нужно для тестирования колбеков
        const onChange = jest.fn();

        // getByText - это функция из библиотеки @testing-library/react которая позволяет найти элемент по тексту
        const { getByText } = render(<UseStateCounter onChange={onChange} />)

        // fireEvent.click - это функция из библиотеки @testing-library/react которая позволяет эмулировать клик по элементу
        fireEvent.click(getByText(/У меня useState 0/))
        fireEvent.click(getByText(/У меня useState2/))

        // expect - это функция из библиотеки jest которая позволяет проверить что значение равно ожидаемому
        // toBeInTheDocument - это функция из библиотеки @testing-library/react которая позволяет проверить что элемент с заданным текстом отрисовался в DOM
        expect(getByText('У меня useState 1')).toBeInTheDocument()
        // toHaveBeenCalledTimes - это функция из библиотеки jest которая позволяет проверить сколько раз была вызвана мок функция
        expect(onChange).toHaveBeenCalledTimes(1);
    })
















    // //
    // //
    // //
    // //
    /* 
           let a: any;
        test('Проверяем снапшот', () => {
            act(() => {
                a = create(<UseStateCounter />);
            });
    
            expect(a).toMatchSnapshot();
        });
     */
});

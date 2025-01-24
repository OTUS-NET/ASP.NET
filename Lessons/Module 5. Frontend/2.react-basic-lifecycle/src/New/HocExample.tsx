import React, { Component } from 'react';

// Интерфейс для пропсов, которые будет принимать оборачиваемый компонент
interface WithClickCountProps {
    clickCount: number;
}

// Интерфейс для состояния компонента HOC
interface WithClickCountState {
    count: number;
}


// Создаем HOC, который добавляет счетчик кликов
// HOC, который добавляет счетчик кликов
function withClickCount<T extends object>(WrappedComponent: React.ComponentType<T & WithClickCountProps>) {
    return class extends Component<T, WithClickCountState> {
        constructor(props: T) {
            super(props);
            // Инициализация состояния с начальным значением
            this.state = {
                count: 0,
            };
        }

        // Метод для увеличения счетчика
        incrementCount = () => {
            this.setState((prevState) => ({ count: prevState.count + 1 }));
        };

        render() {
            // Возвращаем обернутый компонент с дополнительными пропсами
            return (
                <WrappedComponent
                    {...this.props} // Передаем все пропсы, которые были переданы в HOC
                    clickCount={this.state.count} // Добавляем новое состояние как пропс
                />
            );
        }
    };
}

// Пример использования HOC  
// Интерфейс для пропсов компонента
interface MyComponentProps {
    clickCount: number; // Получаем clickCount от HOC
}

class HocExample extends Component<MyComponentProps> {
    render() {
        const { clickCount } = this.props; // Используем clickCount из пропсов
        return (
            <div>
                <p>Количество кликов: {clickCount}</p>
            </div>
        );
    }
}

export default HocExample;
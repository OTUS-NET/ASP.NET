import React, { PureComponent } from 'react';

// Интерфейс для пропсов
interface MyComponentProps {
  message: string;
}

// Интерфейс для состояния
interface MyComponentState {
  count: number;
}

class MyPureComponent extends PureComponent<MyComponentProps, MyComponentState> {
  constructor(props: MyComponentProps) {
    super(props);
    this.state = {
      count: 0,
    };
  }

  // Метод для увеличения счётчика
  increment = () => {
    this.setState(prevState => ({
      count: prevState.count + 1,
    }));
  };

  render() {
    const { message } = this.props;
    const { count } = this.state;

    console.log('Рендер компонента');

    return (
      <div>
        <p>{message}</p>
        <p>Счётчик: {count}</p>
        <button onClick={this.increment}>Увеличить</button>
      </div>
    );
  }
}

export default MyPureComponent;
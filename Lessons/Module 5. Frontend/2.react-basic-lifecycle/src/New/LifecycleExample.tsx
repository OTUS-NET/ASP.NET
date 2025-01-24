import React, { Component } from 'react';

// Определяем типы для пропсов и состояния
interface Props {
  initialCount?: number; // Опциональный пропс для начального значения счётчика
}

interface State {
  count?: number;
  hasError?: boolean;
}

class LifecycleExample extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    // Инициализация состояния: начальный count может быть из пропсов, если они переданы
    this.state = {
      count: props.initialCount || 0, // Если пропс не передан, будет 0
      hasError: false, // флаг ошибки, по умолчанию false
    };
    console.log('1. constructor: компонент создается');
  }

  // Этот метод вызывается, когда компонент получает новые пропсы
  static getDerivedStateFromProps(nextProps: Props, nextState: State): State | null {
    console.log('2. getDerivedStateFromProps: обновляются пропсы или состояние');
    // Если новый пропс initialCount отличается от текущего состояния, обновим состояние
    if (nextProps.initialCount !== nextState.count) {
      return {
        count: nextProps.initialCount || nextState.count, // Если пропс передан, обновим count
      };
    }
    return null;
  }

  // Метод для обработки ошибок при рендеринге
  static getDerivedStateFromError(error: Error): State {
    console.log('3. getDerivedStateFromError: обработка ошибки');
    // Если ошибка произошла, установим флаг ошибки в true
    return { hasError: true };
  }

  // Этот метод позволяет предотвратить ненужное обновление компонента
  shouldComponentUpdate(nextProps: Props, nextState: State): boolean {
    console.log('4. shouldComponentUpdate: нужно ли обновить компонент?');
    // Например, можно не обновлять компонент, если count не изменился
    return nextState.count !== this.state.count;
  }

  // Этот метод вызывается перед обновлением DOM
  getSnapshotBeforeUpdate(prevProps: Props, prevState: State): any {
    console.log('5. getSnapshotBeforeUpdate: перед изменением DOM');
    // Мы можем вернуть значение, которое будет передано в componentDidUpdate
    return { prevCount: prevState.count };
  }

  // Этот метод вызывается после того, как изменения были внесены в DOM
  componentDidUpdate(prevProps: Props, prevState: State, snapshot: any) {
    console.log('6. componentDidUpdate: компонент обновился');
    console.log('Предыдущее значение count:', prevState.count);
    if (snapshot) {
      console.log('Снимок предыдущего состояния:', snapshot.prevCount);
    }
  }

  // Этот метод вызывается после того, как компонент был добавлен в DOM
  componentDidMount() {
    console.log('7. componentDidMount: компонент вставлен в DOM');
    // Здесь обычно выполняются запросы к серверу или подписка на события
    setTimeout(() => {
      this.setState({ count: this.state.count! + 1 });
    }, 2000); // Через 2 секунды увеличиваем count
  }

  // Этот метод вызывается перед тем, как компонент будет удален из DOM
  componentWillUnmount() {
    console.log('8. componentWillUnmount: компонент будет удален');
    // Обычно здесь очищаются таймеры, отменяются запросы или отписываются от событий
  }

  // Обработчик для увеличения счётчика
  increment = () => {
    this.setState((prevState) => ({ count: prevState.count! + 1 }));
  };

  // Обработчик для сброса ошибки
  resetErrorBoundary = () => {
    this.setState({ hasError: false });
  };

  render() {
    console.log('9. render: компонент отрисовывается');
    const { count, hasError } = this.state;

    if (hasError) {
      return (
        <div>
          <h1>Что-то пошло не так!</h1>
          <button onClick={this.resetErrorBoundary}>Попробовать снова</button>
        </div>
      );
    }

    return (
      <div>
        <h1>Count: {count}</h1>
        <button onClick={this.increment}>Increment</button>
      </div>
    );
  }
}

export default LifecycleExample;
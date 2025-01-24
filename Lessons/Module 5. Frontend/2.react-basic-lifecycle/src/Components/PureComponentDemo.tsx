import { Component, PureComponent, ReactNode } from "react"; //импортируем React из библиотеки react

interface Math { //создаем интерфейс Math
    a: number;
    b: number;
}

// Нам нужно типизировать пропсы и стейт нашего компонента, так как они должны иметь числа а и б соответственно.
type SummerProps = {
  a: number;
  b: number;
};

interface SummerState {
    data: { a: number; b: number };
}


export class Summer extends PureComponent<SummerProps, SummerState>{ //

    // Вызывается после того, как компонент был рендерен и отрисован на странице. 
    // Подходит для работы с DOM, подписка на события, запуск анимаций.
    componentDidMount(): void {
        console.log("%c SUMMER СМОГ ЗАРЕНДЕРИТЬСЯ",'color:red; background-color: lightblue');
    }

    // Вызывается прямо перед удалением компонента из дерева DOM. Это место для очистки resource'ов компонента, 
    // таких как таймауты, интервалы и подписки на события.
    componentWillUnmount(): void {

        console.log('%c Ариведерчи', 'color:white; background-color: red');
    }


   // Позволяет компонентам решить, нужно ли им обновлять свое дерево компонентов при изменении пропсов или состояния.
   shouldComponentUpdate(
        nextProps: Readonly<Math>,
         nextState: Readonly<SummerState>,
          nextContext: any): boolean {
              console.log('write', nextProps.a + nextProps.b, this.props.a + this.props.b);
              // Здесь мы сравниваем пропсы, чтобы определить нужно ли компоненту перерисовываться.
        return nextProps.a === this.props.a && nextProps.b === this.props.b; 
    }

   /*  shouldComponentUpdate(
        nextProps: Readonly<Math>,
         nextState: Readonly<SummerState>,
          nextContext: any): boolean {

        console.log('write', nextProps.a + nextProps.b, this.props.a + this.props.b);
        return nextProps.a + nextProps.b !== this.props.a + this.props.b;
    } */

    // Вызывается после обновления компонента
    componentDidUpdate(prevProps: Readonly<Math>, prevState: Readonly<SummerState>, snapshot?: any): void {
        console.log('prev',prevProps);
        console.log('current', this.props);
        console.log('SNapshot', snapshot);
        console.log('%c Апдейт', 'color:green; background-color: yellow;font-size:20px')
    }

    // Вызывается до обновления компонента. Возвращает объект с текущими значениями состояния и DOM.
    // Может использоваться для записи данных в localStorage или базу данных перед обновлением компонента
    getSnapshotBeforeUpdate(prevProps: Readonly<Math>, prevState: Readonly<SummerState>) {
        return { "Предыдущий A": prevProps.a, "Предыдущий B": prevProps.b };
    }

    constructor(props: SummerProps) {
        console.log('Сработал конструктор');
        super(props);
        this.state = { data: { a: props.a, b: props.b } };
    }

    render(): ReactNode {
        const s = new Date().toString();
        console.log('РИСУЮ');
        const style = { border: '2px dotted red', padding: '10px' };
        return <div style={style}>
            <span>{this.props.a} + {this.props.b} = {this.props.a + this.props.b}<br />{s}</span>
        </div>;
    }
}


export interface LifecycleDemoState {
    checked: boolean;
    a: number;
    b: number;
    data: { a: number, b: number };

}


interface State { date: string }

class Timer extends Component<{}, State>{


    constructor(props: {}) {
        super(props);
        this.state = { date: new Date() + '' };
        setInterval(() => {
            this.setState({ date: new Date() + '' });
        }, 1000);
    }

    render(): ReactNode {
        return <span>Текущее время {this.state.date}</span>;
    }
}

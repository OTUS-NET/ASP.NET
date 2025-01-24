import { Component, ReactNode } from "react";
import { LifecycleDemoState, Summer } from "./PureComponentDemo";

/// Жизненный цикл компонента
    // 1. constructor
    // 2. componentWillMount
    // 3. render
    // 4. componentDidMount
    // 5. componentWillReceiveProps
    // 6. shouldComponentUpdate
    // 7. componentWillUpdate
    // 8. componentDidUpdate
    // 9. componentWillUnmount

export default class LifecycleDemo extends Component<{}, LifecycleDemoState> { //создаем класс LifecycleDemo, который наследуется от Component с пустыми свойствами и типом состояния LifecycleDemoState

    constructor(props: any) {
        super(props);
        this.state = {
            a: 1,
            b: 1,
            data: { a: 1, b: 1 },
            checked: true
        };
    }
    setA = (e: any) => {
        this.setState({ a: parseInt(e.target.value) });
    };

    setB = (e: any) => {
        this.setState({ b: parseInt(e.target.value) });
    };

    setAB = (e: any) => {
        this.setState({ data: { a: this.state.a, b: this.state.b } });
    };

    getSummer = () => {
        if (this.state.checked) {
            const { a, b } = this.state.data;
            return <Summer a={a} b={b} />;
        }

        return null;
    };
    // Этот метод вызывается после того, как изменения были внесены в DOM
    componentDidUpdate(prevState: LifecycleDemoState, snapshot: LifecycleDemoState) {
        console.log('6. componentDidUpdate: компонент обновился');
        console.log('Предыдущее значение count:', prevState.a);
        if (snapshot) {
        console.log('Снимок предыдущего состояния:', snapshot.a);
        }
    }
    render(): ReactNode {
        const { a, b } = this.state.data;
        const d = new Date() + '';
        return <>


            <label>A: </label><input type="text" onChange={this.setA} />
            <br />

            <label>B: </label><input type="text" onChange={this.setB} />
            <br />


            <button onClick={this.setAB}> Отправить</button>


            <br />


            <button onClick={() => this.setState({ checked: !this.state.checked })}>
                {this.state.checked ? 'Выключить' : 'Включить'}
            </button>

            {/* <br />{this.getSummer()} */}
            {this.getSummer()}
            <br />
           
          
            <Timer /> /
        </>;
    }
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

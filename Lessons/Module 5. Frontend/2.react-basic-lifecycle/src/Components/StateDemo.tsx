import { Component, ReactNode } from "react";
import './StateDemo.css';


interface StateState {//создаем интерфейс StateState 
    number: number;
    otus: string;
}

export class StateDemo extends Component<{}, StateState> {
    
    private _counter:number;
    
    constructor() {
        super({}); //вызываем конструктор родительского класса

        this.state = {
            number: 100,
            otus: 'Privet',
        };
        this._counter = 0;
    }

    incr = () => { //
        this._counter++;
        console.log(this._counter++);
        //this.setState({ number: this.state.number + 1 });

        //this.setState({}); //сайд эффект при обновлении стейта
        //console.log(this.state.number);
    }

    render(): ReactNode {

        console.log('rerender');
        const date = new Date() + '';

        return <div className="state-demo">
            Сегодня {date}
            <br />
            <button onClick={this.incr} >
                'Нажми меня' {this.state.number}
            </button>
            <div>_counter: {this._counter}</div>
        </div>;
    }
}



import { Component, createContext, useState } from "react";
import './ContextDemo.css';


enum Themes { //создаем перечисление Themes
    RedBlue,
    YellowGreen,
    BlackWhite
}

const ThemeObj = { //создаем объект ThemeObj
    [Themes.RedBlue]: 'red-blue',
    [Themes.YellowGreen]: 'green-yellow',
    [Themes.BlackWhite]: 'black-white',
}

const fancyOtusContext = createContext({ theme: Themes.RedBlue, text: '' }); //создаем КОНТЕКСТ fancyOtusContext


interface ColorButtonProps { //создаем интерфейс ColorButtonProps
    onClick: (theme: Themes, text: string) => void;
    text: string;
    theme: Themes;
}


class ColorButton extends Component<ColorButtonProps>{ //создаем класс ColorButton, который наследуется от Component с типом props ColorButtonProps
    style = {
        fontSize: '20px',
        margin: '5px',
        padding: '5px'
    };

    render() {
        return <button
            style={this.style}
            className={ThemeObj[this.props.theme]}
            onClick={() => this.props.onClick(this.props.theme, this.props.text)}>
            {this.props.text}
        </button>;
    }
}

interface ProviderDemoState { //создаем интерфейс ProviderDemoState
    theme: Themes;
    text: string;
    externalText?: string;
}



function Ramka(p: OnClickProps) { //создаем функцию Ramka
    const style = {
        border: '1px solid black',
        margin: '20px',
        padding: '20px',
    }

const [text,setText]=useState<string>('');


    return <div style={style}>
    [{text}]    <Colored onClick={(a) => setText(a)} />

    </div>
}


interface OnClickProps {
    onClick: ((a: string) => void);
}

class Colored extends Component<OnClickProps> {

    foo = (c:string) => {
        this.props.onClick(`ABRAKADABRA '${c}`);
    }



    render() {
        return <fancyOtusContext.Consumer>
            {
                context => {
                    console.log('this is context', context);
                    return <div className={ThemeObj[context.theme]}>
                        Выбран текст {context.text}
                        <button onClick={() => this.foo(context.text)}>ЖМИ</button>
                    </div>

                }
            }
        </fancyOtusContext.Consumer>
    }
}


export class ContextDemo extends Component<{}, ProviderDemoState>{

    constructor(props: {}) {
        super(props);
        this.state = { text: 'Ничего не выбрано', theme: Themes.BlackWhite }
    }

    setTheme = (theme: Themes, text: string) => {
        this.setState({ theme, text });
    }

    getButton = (text: string, theme: Themes) => {
        return <ColorButton text={text} theme={theme} onClick={this.setTheme} />
    }

    setText = (a: string) => {
        console.log(a);
        this.setState({ externalText: a });
    }

    render() {


        const Provider = fancyOtusContext.Provider;

        return <>
        
            {this.getButton("красно-синий", Themes.RedBlue)}
            {this.getButton("желто-зеленый", Themes.YellowGreen)}
            {this.getButton("черно-белый", Themes.BlackWhite)}
            <Provider value={this.state}>
                <Ramka onClick={(a) => this.setText(a)} />
            </Provider>

        </>;
    }
}
import { Component, ComponentType, PropsWithChildren, ReactElement, ReactNode } from "react";


interface HocDemoState {
    isLoading1: boolean;
    isLoading2: boolean;
}


function El(){
    return <span>AAAAA</span>;
}

export class HocDemo extends Component<{}, HocDemoState> {



    constructor(props: {}) {
        super(props);
        this.state = { isLoading1: true, isLoading2: true };
        setInterval(() => {
            this.setState({ isLoading1: false })
        }, 5000);
        setInterval(() => {
            this.setState({ isLoading2: false })
        }, 10000);
    }


    render(): ReactNode {

        const WithLoading1 = withLoading1(OneTexter);
        const WithLoading4 = withLoading2(OneTexter);
        const WithLoading2 = withLoading2(TwoTexter);
        const WithLoading3 = withLoading2(El);
        return <>
            <WithLoading1 isLoading={this.state.isLoading1} fancyText="Привет1" />
            <WithLoading2 isLoading={this.state.isLoading2} fancyText="Привет2" />
            <WithLoading3 isLoading={this.state.isLoading1} />

            {/* <OneTexter text="Другой текст" />
            <TwoTexter  text=" еще" />

            <WithLoadingChildren isLoading={this.state.isLoading1}>
                <OneTexter text="Привет122222" />
            </WithLoadingChildren> */}

        </>;
    }


}




interface TextProps {
    fancyText: string;
}

class OneTexter extends Component<TextProps> {

    render(): ReactNode {

        const style = {
            background: 'lightblue',
            color: 'purple',
            border: '5px dotted red',
            padding: '10px'
        }
        return <div style={style}>
            {this.props.fancyText}
        </div>
    }
}


class TwoTexter extends Component<TextProps> {

    render(): ReactNode {

        const style = {
            background: 'cyan',
            color: 'purple',
            border: '10px solid purple',
            padding: '10px'
        }
        return <div style={style}>
            {this.props.fancyText}
        </div>
    }
}



interface LoadingProps {
    isLoading: boolean;
}

class LoadingScreen extends Component {
    render(): ReactNode {
        return <span style={{ fontSize: '20px' }}>Loading...</span>;
    }
}




export class WithLoadingChildren extends Component<PropsWithChildren & LoadingProps> {

    render() {

        const Children = this.props.children!;
        return this.props.isLoading ? <LoadingScreen /> : this.props.children;
    }

}




const withLoading1 = <P extends Object>(Com: ComponentType<P>) =>
    class LoadingComponent extends Component<LoadingProps & P> {
        render() {
            return this.props.isLoading ? <LoadingScreen /> : <Com {...this.props} />
        }
    }

const withLoading2 = <P extends Object>(Com: ComponentType<P>) =>
    function LoadingComponent(props: LoadingProps & P) {

        return props.isLoading ? <LoadingScreen /> : <Com {...props} />

    }


//на JS
// const withLoading2 = (Com) =>
// class WithLoading extends Component{
//     render() {
//         return this.props.isLoading ? <LoadingScreen /> : <Com {...this.props} />
//     }
// }


// // на JS
// const withLoading2 = (Com) =>
// (props) => {
//     return props.isLoading ? <LoadingScreen /> : <Com {...props} />
// };



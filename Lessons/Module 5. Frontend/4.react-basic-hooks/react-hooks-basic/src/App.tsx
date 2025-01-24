import CustomHookBasic from "./components/Custom/CustomHookBasic";
import GreetingFromLocalStorage from "./components/Custom/UseLocalStorage";
import UseCallbackBasic from "./components/UseCallback/Basic";
import UseCallbackCounterApp from "./components/UseCallback/UseCallbackCounterApp";
import UseContextBasic from "./components/UseContext/Basic";
import ChangeThemeApp from "./components/UseContext/ChangeTheme";
import ChangeThemeExample from "./components/UseContext/ChangeTheme";
import CleanupEffect from "./components/UseEffect/Basic";
import UseEffectBasic from "./components/UseEffect/Basic";
import UserProfileApp from "./components/UseEffect/UserProfileApp";
import UseMemoUserListApp from "./components/UseMemo/Basic";
import UseRefInputApp from "./components/UseRef/Basic";
import UseRefValueApp from "./components/UseRef/UseRefValueApp";
import UseStateBasic from "./components/UseState/Basic";

const App = (props: AppProps) => {
  
  return (
 
  <UseStateBasic />
  //<UseEffectBasic />
  //<UserProfileApp />
  //<UseContextBasic />
  //<ChangeThemeApp />
   //<UseRefInputApp />
   //<UseRefValueApp />
   //<UseCallbackBasic />
   //<UseCallbackCounterApp />
   //<UseMemoUserListApp />
   //<CustomHookBasic />
   //<GreetingFromLocalStorage/>
);


};

export default App;

interface AppProps {
  title: string;
}



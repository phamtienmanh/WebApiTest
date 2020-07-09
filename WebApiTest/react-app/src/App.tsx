import React from 'react';
import './App.css';
import {BrowserRouter as Router, Redirect, Route, Switch} from "react-router-dom";
import Home from "./components/Home";
import Login from "./components/Login";
import Register from "./components/Register";
import AuthService from "./services/AuthService";

const  PrivateRoute: React.FC<{
    component: React.FC;
    path: string;
    exact: boolean;
}> = (props) => {

    let authService = AuthService.getInstance();
    const isAuth = authService.isAuthed();
    return  isAuth ? (<Route  path={props.path}  exact={props.exact} component={props.component} />) :
        (<Redirect  to="/login"  />);
};

function App() {
    return (
        <div className="App">
            <Router>
                <Switch>
                    <PrivateRoute  path="/"  component={Home}  exact  />
                    <Route path="/login" component={Login}/>
                    <Route path="/regis" component={Register}/>
                </Switch>
            </Router>
        </div>
    );
}

export default App;

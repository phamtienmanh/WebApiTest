import React from 'react';
import {useHistory} from 'react-router-dom';
import AuthService from "../services/AuthService";
import {UserModel} from "../models/UserModel";

function Login(props: any) {
    let history = useHistory();
    let authService = AuthService.getInstance();
    const [userModel, setUserModel] = React.useState(new UserModel());
    const [message, setMessage] = React.useState("");
    const login = (userModel: UserModel) => {
        authService.login(userModel).then(mess => {
            if (mess) {
                setMessage(mess);
            } else {
                history.push("/");
            }
        })
    };

    return (
        <div className="">
            <label>Username</label><input value={userModel.userName}
                                          onChange={e => setUserModel({...userModel, userName: e.target.value})}/>
            <label>Password</label><input value={userModel.password}
                                          onChange={e => setUserModel({...userModel, password: e.target.value})}
                                          type="password"/>
            <button onClick={() => login(userModel)}>Login</button>
            <button onClick={() => history.push("/regis")}>Sign up</button>
            <div>{message}</div>
        </div>
    );
}

export default Login;

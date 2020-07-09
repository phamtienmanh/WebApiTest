import {UserModel} from "../models/UserModel";
import {LoggedInUserModel} from "../models/LoggedInUserModel";

export default class AuthService {
    private static instance: AuthService;
    private token = localStorage.getItem('token');

    static getInstance() {
        if (AuthService.instance == null) {
            AuthService.instance = new AuthService();
        }
        return this.instance;
    }

    isAuthed() {
        return this.token !== "" && this.token !== null;
    }

    signUp = async (userModel: UserModel) => {
        let message = "";
        const resp = await fetch(window.location.origin + "/api/auth/registration", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userModel)
        });

        if (resp.status !== 200) {
            const respData = await resp.json();
            for (const key of Object.keys(respData)) {
                message += (`${key}: ${respData[key]} `);
            }
        }
        return message;
    };

    login = async (userModel: UserModel) => {
        let message = "";
        const resp = await fetch(window.location.origin + "/api/auth/login", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userModel)
        });
        if (resp.status === 200) {
            const respData: LoggedInUserModel = await resp.json();
            this.token = respData.token;
            localStorage.setItem('token', this.token);
        } else {
            const respData = await resp.json();
            for (const key of Object.keys(respData)) {
                message += (`${key}: ${respData[key]} `);
            }
        }
        return message;
    };
}

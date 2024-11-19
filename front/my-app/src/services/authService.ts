import Cookies from "js-cookie";
import apiClient from "./apiClient";
import { LoginInput, RegisterInput } from "../models/authModels";

export interface LoginResponse {
    access_token: string;
    refreshToken: string;
    user: {
        id: number;
        firstName: string;
        lastName: string;
        username: string;
        email: string;
        password: string;
        phoneNumber: string;
    };
}

export const login = async (data: LoginInput): Promise<LoginResponse> => {
    try {
        const response = await apiClient.post(`/users/authorize?username=${data.username}&password=${data.password}`);
        const { access_token, refreshToken, username } = response.data;

        Cookies.set('token', access_token.result);
        Cookies.set('refreshToken', refreshToken);

        const authToken = `Bearer ${access_token.result}`;

        const userResponse = await apiClient.post("/users/userdata", {}, {
            headers: {Authorization: authToken}
        });

        const user = userResponse.data;

        Cookies.set("user", JSON.stringify(user));

        return {access_token: access_token.result, refreshToken, user};
    } 
    catch (error) 
    {
        console.error("Login error:", error);
        throw new Error("Failed to login. Please check your credentials.");
    }
};

export const register = async (data: RegisterInput): Promise<void> => {
    try {
        await apiClient.post(`users/register`, data);
    } catch (error) {
        console.error("Ошибка при регистрации:", error);
        throw new Error("Не удалось зарегистрироваться. Попробуйте позже.");
    }
};

export const logout = () => {
    Cookies.remove('token');
    Cookies.remove('refreshToken');
    Cookies.remove('user');
};

export const isAuthenticated = () => {
    return !!Cookies.get('token') && !!Cookies.get('user');
};

export const getCurrentUser = async () => {
    try {
        const userj = Cookies.get("user");
        const token = Cookies.get("token");
        const refreshToken = Cookies.get("refreshToken");

        if (userj)
        {
            const user = JSON.parse(userj);

            return {
                token,
                refreshToken,
                user
            }
        }

        throw new Error("Could not retrieve user data.");

    } 
    catch (error) 
    {
        console.error("Error retrieving user data:", error);
        throw new Error("Could not retrieve user data.");
    }
};

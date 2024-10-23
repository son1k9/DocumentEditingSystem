import Cookies from "js-cookie";
import axios from "axios";
import { LoginInput, RegisterInput } from "../models/authModels";

const API_URL = "http://api";

export interface LoginResponse {
    token: string;
    user: {
        id: number;
        name: string;
        email: string;
        role: string;
    };
}

export const login = async (data: LoginInput): Promise<LoginResponse> => {
    try {
        /*
        const response = await axios.post(`${API_URL}/login`, data);
        const { token, user } = response.data;
        Cookies.set('token', token, { expires: 30 });
        return { token, user };
        */
        const user = {
            id: 1,
            name: "Тестовый пользователь",
            email: "testuser@example.com",
            role: "admin",
        };
        const token = "test-token";
        Cookies.set('token', token, { expires: 30 });
        return { token, user };
    } catch (error) {
        console.error("Ошибка при логине:", error);
        throw new Error("Не удалось выполнить вход. Проверьте данные.");
    }
};


export const register = async (data: RegisterInput): Promise<void> => {
    try {
        await axios.post(`${API_URL}/register`, data);
    } catch (error) {
        console.error("Ошибка при регистрации:", error);
        throw new Error("Не удалось зарегистрироваться. Попробуйте позже.");
    }
};

export const logout = () => {
    Cookies.remove('token');
};

export const isAuthenticated = () => {
    return !!Cookies.get('token');
};

export const getCurrentUser = async (): Promise<{ id: number, name: string, email: string, role: string }> => {
    try {
        /*
        const token = Cookies.get('token');
        if (!token) {
            throw new Error('Нет токена');
        }
        const response = await axios.get(`${API_URL}/me`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
        */
        return {
            id: 1,
            name: "Тестовый пользователь",
            email: "test@example.com",
            role: "admin",
        };
    } catch (error) {
        console.error("Ошибка при получении текущего пользователя:", error);
        throw new Error("Не удалось загрузить данные пользователя.");
    }
};

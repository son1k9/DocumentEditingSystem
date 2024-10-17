// src/services/authService.ts
import Cookies from "js-cookie";
import axios from "axios";
import { LoginInput, RegisterInput } from "../models/authModels";

const API_URL = "http://api"; // Убедитесь, что здесь указан правильный URL

export interface LoginResponse {
    token: string;
}

export const login = async (data: LoginInput): Promise<LoginResponse> => {
    const response = await axios.post(`${API_URL}/login`, data);
    const { token } = response.data;
    Cookies.set('token', token, { expires: 30 });
    return response.data;
};

export const register = async (data: RegisterInput): Promise<void> => {
    await axios.post(`${API_URL}/register`, data);
};

export const logout = () => {
    Cookies.remove('token');
};

export const isAuthenticated = () => {
    return !!Cookies.get('token');
};

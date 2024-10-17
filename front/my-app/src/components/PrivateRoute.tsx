import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext"; // Импортируем хук контекста

const PrivateRoute: React.FC = () => {
    const { isAuthenticated } = useAuth(); // Используем контекст для получения состояния аутентификации
    return isAuthenticated ? <Outlet /> : <Navigate to="/login" />;
}

export default PrivateRoute;
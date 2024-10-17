import React, { createContext, useContext, useState, useEffect } from 'react';
import { login as loginService, register as registerService, logout as logoutService, isAuthenticated as checkAuthenticated } from '../services/authService'; // Импортируем сервисы
import { LoginInput, RegisterInput } from '../models/authModels'; // Импортируем интерфейсы

interface AuthContextProps {
  isAuthenticated: boolean;
  login: (data: LoginInput) => Promise<void>;
  logout: () => void;
  register: (data: RegisterInput) => Promise<void>;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(checkAuthenticated());

  useEffect(() => {
    // Проверка аутентификации при загрузке
    setIsAuthenticated(checkAuthenticated());
  }, []);

  const login = async (data: LoginInput) => {
    await loginService(data);
    setIsAuthenticated(true);
  };

  const logout = () => {
    logoutService();
    setIsAuthenticated(false);
  };

  const register = async (data: RegisterInput) => {
    await registerService(data);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
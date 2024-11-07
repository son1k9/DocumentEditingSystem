import React, { createContext, useContext, useState, useEffect } from 'react';
import { login as loginService, register as registerService, logout as logoutService, isAuthenticated as checkAuthenticated, getCurrentUser } from '../services/authService';
import { LoginInput, RegisterInput } from '../models/authModels'; 

interface User {
  id: number;
  name: string;
  email: string;
  role: string;
}

interface AuthContextProps {
  isAuthenticated: boolean;
  user: User | null;
  error: string | null;
  login: (data: LoginInput) => Promise<void>;
  logout: () => void;
  register: (data: RegisterInput) => Promise<void>;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(checkAuthenticated());
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const initAuth = async () => {
      if (checkAuthenticated()) {
        try {
          const currentUser = await getCurrentUser();
          setUser(currentUser);
          setIsAuthenticated(true);
        } catch (error: any) {
          setError(error.message);
          setIsAuthenticated(false);
          setUser(null);
        }
      }
    };
    initAuth();
  }, []);

  const login = async (data: LoginInput) => {
    try {
      const { user } = await loginService(data);
      setUser(user);
      setIsAuthenticated(true);
      setError(null);
    } catch (error: any) {
      setError(error.message);
    }
  };

  const logout = () => {
    logoutService();
    setIsAuthenticated(false);
    setUser(null);
    setError(null);
  };

  const register = async (data: RegisterInput) => {
    try {
      await registerService(data);
      setError(null);
    } catch (error: any) {
      setError(error.message);
    }
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, error, login, logout, register }}>
      {children}
      {error && <p className="error-message">{error}</p>}
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

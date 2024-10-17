// src/pages/Login.tsx
import React from 'react';
import { useForm } from 'react-hook-form';
import { useAuth } from '../context/AuthContext'; // Импортируем хук контекста
import { useNavigate } from 'react-router-dom';
import { LoginInput } from '../models/authModels'; // Импортируем интерфейс

const Login: React.FC = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<LoginInput>();
  const { login } = useAuth(); // Используем контекст для управления состоянием аутентификации
  const navigate = useNavigate();

  const onSubmit = async (data: LoginInput) => {
    try {
      await login(data); // Теперь используем метод login из контекста
      navigate('/dashboard'); // Перенаправляем на личный кабинет
    } catch (error: any) {
      alert(error.response?.data?.message || 'Ошибка при входе');
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="w-full max-w-md p-8 bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center mb-6">Вход</h1>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="mb-4">
            <label htmlFor="username" className="block mb-2 text-sm font-medium text-gray-700">
              Имя пользователя
            </label>
            <input
              type="text"
              id="username"
              {...register('username', { required: 'Это поле обязательно для заполнения' })}
              className={`w-full p-2 border ${errors.username ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.username && <p className="text-red-500 text-sm">{errors.username.message}</p>}
          </div>

          <div className="mb-4">
            <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-700">
              Пароль
            </label>
            <input
              type="password"
              id="password"
              {...register('password', { required: 'Это поле обязательно для заполнения' })}
              className={`w-full p-2 border ${errors.password ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.password && <p className="text-red-500 text-sm">{errors.password.message}</p>}
          </div>

          <button
            type="submit"
            className="w-full p-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          >
            Войти
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;

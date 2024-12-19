import React from 'react';
import { useForm } from 'react-hook-form';
import { useAuth } from '../context/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { RegisterInput } from '../models/authModels';

const Register: React.FC = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterInput>();
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (data: RegisterInput) => {
    try {
      await registerUser(data);
      navigate('/login');
    } catch (error: any) {
      alert(error.response?.data?.message || 'Ошибка при регистрации');
    }
  };

  return (
    <div className="flex items-center justify-center bg-gray-100">
      <div className="w-full max-w-md p-8 bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center mb-6">Регистрация</h1>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="mb-4">
            <label htmlFor="firstname" className="block mb-2 text-sm font-medium text-gray-700">
              Имя
            </label>
            <input
              type="text"
              id="firstname"
              {...register('firstname', { required: 'Это поле обязательно для заполнения' })}
              className={`w-full p-2 border ${errors.firstname ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.firstname && <p className="text-red-500 text-sm">{errors.firstname.message}</p>}
          </div>

          <div className="mb-4">
            <label htmlFor="lastname" className="block mb-2 text-sm font-medium text-gray-700">
              Фамилия
            </label>
            <input
              type="text"
              id="lastname"
              {...register('lastname', { required: 'Это поле обязательно для заполнения' })}
              className={`w-full p-2 border ${errors.lastname ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.lastname && <p className="text-red-500 text-sm">{errors.lastname.message}</p>}
          </div>

          <div className="mb-4">
            <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-700">
              Электронная почта
            </label>
            <input
              type="email"
              id="email"
              {...register('email', { required: 'Это поле обязательно для заполнения', pattern: { value: /^[^@]+@[^@]+\.[^@]+$/, message: 'Введите корректный email' } })}
              className={`w-full p-2 border ${errors.email ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.email && <p className="text-red-500 text-sm">{errors.email.message}</p>}
          </div>

          <div className="mb-4">
            <label htmlFor="phoneNumber" className="block mb-2 text-sm font-medium text-gray-700">
              Номер телефона
            </label>
            <input
              type="text"
              id="phoneNumber"
              {...register('phoneNumber', { required: 'Это поле обязательно для заполнения' })}
              className={`w-full p-2 border ${errors.phoneNumber ? 'border-red-500' : 'border-gray-300'} rounded-md focus:outline-none focus:ring focus:ring-blue-300`}
            />
            {errors.phoneNumber && <p className="text-red-500 text-sm">{errors.phoneNumber.message}</p>}
          </div>

		  <div className="mb-4">
            <label htmlFor="username" className="block mb-2 text-sm font-medium text-gray-700">
              Логин
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
            Зарегистрироваться
          </button>
        </form>

        <p className="mt-4 text-sm text-center text-gray-600">
          Уже есть аккаунт? <Link to="/login" className="text-blue-500 hover:underline">Войдите</Link>
        </p>
      </div>
    </div>
  );
};


export default Register;

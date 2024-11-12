import React from 'react';
import { Link } from 'react-router-dom';

const Home: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center bg-gray-100 p-4">
      <h1 className="text-4xl font-bold mb-4 text-blue-600">Добро пожаловать!</h1>
      <p className="text-lg text-center mb-6">
        Это приложение для совместного редактирования документов.
        <br />
        Начните с авторизации или переходите в личный кабинет.
      </p>
      <div className="flex space-x-4">
        <button className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 transition duration-200">
          <Link to="/login">Авторизация</Link>
        </button>
        <button className="bg-gray-300 text-gray-800 py-2 px-4 rounded hover:bg-gray-400 transition duration-200">
          <Link to="/register">Регистрация</Link>
        </button>
        <button className="bg-green-500 text-white py-2 px-4 rounded hover:bg-green-600 transition duration-200">
          <Link to="/account">Личный кабинет</Link>
        </button>
      </div>
    </div>
  );
};

export default Home;

import React from 'react';

const Home: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-4xl font-bold mb-4">Добро пожаловать!</h1>
      <p className="text-lg text-center">
        Это приложение для совместного редактирования документов.
        <br />
        Начните с авторизации или переходите в личный кабинет.
      </p>
    </div>
  );
};

export default Home;
import React from 'react';

const About: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-4xl font-bold mb-4">О нас</h1>
      <p className="text-lg text-center max-w-2xl mx-auto">
        Мы создаем приложение для совместного редактирования документов с использованием технологий
        <strong> React</strong>, <strong>TypeScript</strong>, и <strong>SignalR</strong>.
        <br />
        Наша цель - упростить процесс совместной работы и сделать его более эффективным.
      </p>
    </div>
  );
};

export default About;
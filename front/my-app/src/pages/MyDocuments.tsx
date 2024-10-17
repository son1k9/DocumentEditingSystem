import React from 'react';

const MyDocuments: React.FC = () => {
  return (
    <div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">

      <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Редактирование документа</h1>
        <div className="border border-gray-300 p-4 flex-1 overflow-auto">
          <textarea
            className="w-full h-full border-0 focus:outline-none resize-none"
            placeholder="Введите текст вашего документа..."
          />
        </div>
      </div>

      <div className="w-64 bg-gray-100 p-4 border-l border-gray-300 shadow-md flex flex-col">
        <h2 className="text-xl font-bold mb-4">Мои документы</h2>
        <ul className="space-y-2 flex-grow overflow-auto">
          <li className="p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded">
            Документ 1
          </li>
          <li className="p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded">
            Документ 2
          </li>
          <li className="p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded">
            Документ 3
          </li>
          <li className="p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded">
            Документ 4
          </li>
        </ul>
        <button className="mt-4 w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600">
          Создать новый документ
        </button>
      </div>
    </div>
  );
};

export default MyDocuments;

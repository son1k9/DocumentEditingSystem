import React from "react";
import { User } from "../models/User";

interface DashboardInfoProps{
    user: User;
}

const DashboardInfo: React.FC<DashboardInfoProps> = ({ user }) => {
    return (
      <div className="p-4">
        <h2 className="text-2xl font-bold mb-4">Личные данные</h2>
        <p><strong>Имя:</strong> {user.name}</p>
        <p><strong>Email:</strong> {user.email}</p>
        <p><strong>Роль:</strong> {user.role}</p>

        <button className="mt-4 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
          Редактировать
        </button>
      </div>
    );
  };

export default DashboardInfo;
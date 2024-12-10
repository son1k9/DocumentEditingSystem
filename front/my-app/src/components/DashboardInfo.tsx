import React from "react";
import { User } from "../models/User";

interface DashboardInfoProps{
    user: User;
}

const DashboardInfo: React.FC<DashboardInfoProps> = ({ user }) => {
    return (
      <div className="p-4">
        <h2 className="text-2xl font-bold mb-4">Личные данные</h2>
        <p><strong>Фамилия:</strong> {user.user.lastName}</p>
        <p><strong>Имя:</strong> {user.user.firstName}</p>
        <p><strong>Имя пользователя:</strong> {user.user.username}</p>
        <p><strong>Email:</strong> {user.user.email}</p>
        <p><strong>Номер телефона:</strong> {user.user.phoneNumber}</p>
      </div>
    );
  };

export default DashboardInfo;
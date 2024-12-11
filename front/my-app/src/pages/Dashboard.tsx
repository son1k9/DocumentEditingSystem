import React from "react";
import DashboardInfo from "../components/DashboardInfo";
import { useAuth } from "../context/AuthContext";

const Dashboard: React.FC = () => {
  const { user } = useAuth();

  if (!user) {
    return <div>Загрузка данных пользователя...</div>;
  }

  return (
    <div className="p-4 flex gap-4 h-full">
      <section className="flex-1 bg-white p-6 shadow-md rounded">
        <DashboardInfo user={user} />
      </section>
    </div>
  );
};

export default Dashboard;

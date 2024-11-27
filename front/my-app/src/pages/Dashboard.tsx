import React, { useEffect, useState } from "react";
import { Link, Route, Routes, Navigate } from "react-router-dom";
import DashboardInfo from "../components/DashboardInfo";
import DashboardDocuments from "../components/DashboardDocuments";
import { useAuth } from "../context/AuthContext";
import { getDocumentsForUser } from "../services/documentService";
import { Document } from "../models/Document";

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [documents, setDocuments] = useState<Document[]>([]);

  useEffect(() => {
    const fetchDocuments = async () => {
      try{
        if (user?.user.id){
          const fetchDocuments = await getDocumentsForUser();
          setDocuments(fetchDocuments);
        }
      }
      catch (error)
      {
        console.error('Ошибка загрузки документов', error);
      }
    }

    fetchDocuments();

  }, [user]);

  if (!user) {
    return <div>Загрузка данных пользователя...</div>;
  }

  return (
    <div className="p-4 flex gap-4 h-full">

      <aside className="w-1/4 bg-white p-4 shadow-md rounded">
        <h1 className="text-3xl font-bold mb-6">Личный кабинет</h1>

        <nav>
          <ul>
            <li className="mb-4">
              <Link to="/account/dashboard" className="text-blue-500 hover:underline">
                Личные данные
              </Link>
            </li>
            <li>
              <Link to="/account/documents" className="text-blue-500 hover:underline">
                Мои документы
              </Link>
            </li>
          </ul>
        </nav>
      </aside>

      <section className="flex-1 bg-white p-6 shadow-md rounded">
        <Routes>
          <Route path="/dashboard" element={<DashboardInfo user={user} />} />
          <Route path="/documents" element={<DashboardDocuments documents={documents} />} />
          <Route path="/" element={<Navigate to="/account/dashboard" replace />} />
        </Routes>
      </section>
    </div>
  );
};

export default Dashboard;

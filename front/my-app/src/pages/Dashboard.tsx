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
      <section className="flex-1 bg-white p-6 shadow-md rounded">
        <DashboardInfo user={user} />
      </section>
    </div>
  );
};

export default Dashboard;

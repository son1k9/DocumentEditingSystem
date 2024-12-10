import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getDocumentById, DocumentContent } from '../services/documentService';

const DocumentPageView: React.FC = () => {
  const { slug } = useParams();
  const { user } = useAuth();
  const [document, setDocument] = useState<DocumentContent>();

  useEffect(() => {
    const fetchDocument = async () => {
      if (!slug) return;

      const id = parseInt(slug);
      if (isNaN(id)) return;

      const document = await getDocumentById(id);
      setDocument(document);
    };
    fetchDocument();
  })

  if (!user) return null;
  if (!document) return <div>Загрузка документа...</div>;

  return (
    <div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">
      <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Просмотр документа {document.documentName}</h1>
        <div className="border border-gray-300 p-4 flex-1 overflow-auto">
          <textarea
            readOnly={true}
            className="w-full h-full border-0 focus:outline-none resize-none overflow:hidden"
            placeholder="Введите текст вашего документа..."
            value={document.text}
          />
        </div>
      </div>
    </div>
  );
}

export default DocumentPageView;
import React from 'react';
import { Document } from '../models/Document';

interface DashboardDocumentsProps {
  documents: Document[];
}

const DashboardDocuments: React.FC<DashboardDocumentsProps> = ({ documents }) => {
    return (
      <div className="p-4">
        <h2 className="text-2xl font-bold mb-4">Мои документы</h2>
        <ul>
          {documents.map((doc) => (
            <li key={doc.id} className="mb-4">
              <h3 className="text-xl font-semibold">{doc.documentName}</h3>

              <button className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                Редактировать
              </button>
            </li>
          ))}
        </ul>
      </div>
    );
  };
export default DashboardDocuments;
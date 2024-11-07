import React, { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { Document } from '../models/Document';
import { useAuth } from '../context/AuthContext';
import { getDocumentsForUser } from '../services/documentService';
import diff_match_patch from 'diff-match-patch';

const dmp = new diff_match_patch();

const MyDocuments: React.FC = () => {
  const { user } = useAuth();
  const [documents, setDocuments] = useState<Document[]>([]);
  const [activeDocumentId, setActiveDocumentId] = useState<number | null>(null);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [documentContent, setDocumentContent] = useState('');
  const [lastSentContent, setLastSentContent] = useState<string>(''); 
  const typingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    const fetchDocuments = async () => {
      if (user) {
        try {
          const userDocuments = await getDocumentsForUser(user.id);
          setDocuments(userDocuments);
        } catch (error) {
          console.error('Error fetching documents:', error);
        }
      }
    };

    fetchDocuments();
  }, [user]);

  useEffect(() => {
    const connectToHub = async (documentId: number) => {
      if (connection) {
        await connection.stop();
      }

      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(`/documentHub/${documentId}`)
        .withAutomaticReconnect()
        .build();

      newConnection.on('ReceiveDocumentUpdate', (patchText: string) => {
        setDocumentContent((localContent) => {
          const patches = dmp.patch_fromText(patchText);
          const [updatedContent] = dmp.patch_apply(patches, localContent);
          return updatedContent;
        });
      });

      await newConnection.start();
      setConnection(newConnection);
    };

    if (activeDocumentId !== null) {
      const selectedDocument = documents.find(doc => doc.id === activeDocumentId);
      setDocumentContent(selectedDocument?.content || '');
      setLastSentContent(selectedDocument?.content || '');
      connectToHub(activeDocumentId);
    }

    return () => {
      connection?.stop();
    };
  }, [activeDocumentId, documents]);

  const handleDocumentSelect = (documentId: number) => {
    setActiveDocumentId(documentId);
  };

  const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const newContent = e.target.value;
    setDocumentContent(newContent);

    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    typingTimeoutRef.current = setTimeout(() => {
      sendChanges(newContent);
    }, 3000);
  };

  const sendChanges = async (newContent: string) => {
    if (newContent !== lastSentContent) {
      const diff = dmp.diff_main(lastSentContent, newContent);
      if (diff.length) {
        const patches = dmp.patch_make(lastSentContent, diff);
        const patchText = dmp.patch_toText(patches);

        if (connection) {
          try {
            await connection.invoke('SendDocumentUpdate', activeDocumentId, patchText);
            setLastSentContent(newContent);
          } catch (err) {
            console.error('Error sending document update:', err);
          }
        }
      }
    }
  };

  const activeDocument = documents.find(doc => doc.id === activeDocumentId);

  return (
    <div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">
      <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Редактирование документа</h1>
        {activeDocument && (
          <div className="mb-4 text-gray-500 text-sm">
            <p>Создан: {new Date(activeDocument.createdAt).toLocaleString()}</p>
            <p>Последнее обновление: {new Date(activeDocument.updatedAt).toLocaleString()}</p>
          </div>
        )}
        <div className="border border-gray-300 p-4 flex-1 overflow-auto">
          <textarea
            className="w-full h-full border-0 focus:outline-none resize-none"
            placeholder="Введите текст вашего документа..."
            value={documentContent}
            onChange={handleContentChange}
          />
        </div>
      </div>

      <div className="w-64 bg-gray-100 p-4 border-l border-gray-300 shadow-md flex flex-col">
        <h2 className="text-xl font-bold mb-4">Мои документы</h2>
        <ul className="space-y-2 flex-grow overflow-auto">
          {documents.map(doc => (
            <li
              key={doc.id}
              className={`p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded ${activeDocumentId === doc.id ? 'bg-gray-200' : ''}`}
              onClick={() => handleDocumentSelect(doc.id)}
            >
              {doc.title}
              <p className="text-xs text-gray-500">Обновлено: {new Date(doc.updatedAt).toLocaleString()}</p>
            </li>
          ))}
        </ul>
        <button className="mt-4 w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600">
          Создать новый документ
        </button>
      </div>
    </div>
  );
};

export default MyDocuments;

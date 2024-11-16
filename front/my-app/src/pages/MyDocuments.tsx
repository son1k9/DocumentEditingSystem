import React, { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { Document } from '../models/Document';
import { useAuth } from '../context/AuthContext';
import { Operation, OperationType } from '../utils/OperationService';

const MyDocuments: React.FC = () => {
  const { user } = useAuth();
  const [documents, setDocuments] = useState<Document[]>([]);
  const [activeDocumentId, setActiveDocumentId] = useState<string | null>(null);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [documentContent, setDocumentContent] = useState('');
  const [currentVersion, setCurrentVersion] = useState<number>(0); // Текущая версия
  const typingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [operationsQueue, setOperationsQueue] = useState<Operation[]>([]);
  const activeDocumentIdRef = useRef<string | null>(null);

  const mockDocuments = [
    {
      id: 'aefaefs',
      title: 'Тестовый документ',
      content: '',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ];

  useEffect(() => {
    setDocuments(mockDocuments);
  }, []);

  useEffect(() => {
    const initializeConnection = async () => {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5019/hubs/documents')
        .withAutomaticReconnect()
        .build();

      newConnection.on('JoinedDocument', (joinedDocumentId) => {
        console.log(`Joined document ${joinedDocumentId} group successfully`);
      });

      newConnection.on('ReceivedOperation', (operation: Operation, version: number) => {
        console.log('Received operation:', operation, 'Version:', version);
        handleReceivedOperation(operation, version);
      });

      newConnection.on('ReceivedAcknowledge', (nextVersion: number) => {
        console.log('Acknowledged version:', nextVersion);
        setCurrentVersion(nextVersion);
      });

      try {
        await newConnection.start();
        console.log('Connected to SignalR hub');
        setConnection(newConnection);
      } catch (err) {
        console.error('Error connecting to SignalR hub:', err);
      }
    };

    initializeConnection();

    return () => {
      connection?.stop();
    };
  }, []);

  useEffect(() => {
    activeDocumentIdRef.current = activeDocumentId;

    if (connection && activeDocumentId) {
      const joinDocument = async () => {
        try {
          await connection.invoke('JoinDocument', activeDocumentId);
        } catch (err) {
          console.error('Error joining document group:', err);
        }
      };

      joinDocument();
    }
  }, [activeDocumentId, connection]);

  const handleDocumentSelect = (documentId: string) => {
    const selectedDocument = documents.find((doc) => doc.id === documentId);
    setActiveDocumentId(documentId);
    setDocumentContent(selectedDocument?.content || '');
    setCurrentVersion(0); // Сброс версии при переключении документа
    console.log('Selected document:', documentId, selectedDocument);
  };

  const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const newContent = e.target.value;
    setDocumentContent(newContent);
  
    const operations = detectOperations(documentContent, newContent);
  
    setOperationsQueue((prevOps) => [...prevOps, ...operations]);
  };

  useEffect(() => {
    // Этот эффект будет срабатывать каждый раз, когда изменяется очередь операций
    if (operationsQueue.length > 0) {
      sendChanges(operationsQueue);
    }
  }, [operationsQueue]);

  const detectOperations = (oldContent: string, newContent: string) => {
    const operations: Operation[] = [];
    const insertedText = newContent.substring(oldContent.length);
    if (insertedText) {
      operations.push(Operation.createInsertOp(oldContent.length, insertedText, currentVersion, user?.user.id || 0));
    }

    const deletedText = oldContent.substring(newContent.length);
    if (deletedText) {
      operations.push(Operation.createDeleteOp(newContent.length, deletedText, currentVersion, user?.user.id || 0));
    }

    return operations;
  };

  const sendChanges = async (operations: Operation[]) => {
    if (connection && operations.length > 0 && activeDocumentIdRef.current) {
      try {
        const currentQueue = [...operations];
        setOperationsQueue([]); // Очистка очереди после отправки
  
        for (const operation of currentQueue) {
          const operationWithVersion = { ...operation, version: currentVersion };
          console.log('Sending operation with version:', operationWithVersion);
          await connection.invoke('SendOperation', operationWithVersion);
        }
      } catch (err) {
        console.error('Error sending changes:', err);
      }
    }
  };

  const handleReceivedOperation = (operation: Operation, version: number) => {
    /*if (version !== currentVersion) {
      console.warn('Version mismatch:', { localVersion: currentVersion, receivedVersion: version });
      synchronizeWithServer();
    } else {*/
      setDocumentContent((prevContent) => applyOperationToContent(prevContent, operation));
      setCurrentVersion(version);
    //}
  };

  const applyOperationToContent = (content: string, operation: Operation): string => {
    switch (operation.type) {
      case OperationType.Insert:
        return content.slice(0, operation.pos) + operation.text + content.slice(operation.pos);
      case OperationType.Delete:
        return content.slice(0, operation.pos) + content.slice(operation.pos + operation.text.length);
      default:
        console.error('Unknown operation type:', operation.type);
        return content;
    }
  };

  return (
    <div className="flex-1 flex overflow-hidden min-h-[calc(100vh-120px)]">
      <div className="flex-1 bg-white p-6 shadow-md flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Редактирование документа</h1>
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
          {documents.map((doc) => (
            <li
              key={doc.id}
              className={`p-2 bg-white hover:bg-gray-200 cursor-pointer border rounded ${
                activeDocumentId === doc.id ? 'bg-gray-200' : ''
              }`}
              onClick={() => handleDocumentSelect(doc.id)}
            >
              {doc.title}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default MyDocuments;

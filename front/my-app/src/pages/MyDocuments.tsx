import React, { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { Document } from '../models/Document';
import { useAuth } from '../context/AuthContext';
import { Operation, OperationType } from '../utils/OperationService';
import diff_match_patch from 'diff-match-patch';
import { getDocumentsForUser } from '../services/documentService';
import useModal from '../hooks/useModal';
import AddDocumentForm from '../forms/AddDocumentForm';
import UpdateDocumentForm from '../forms/UpdateDocumentForm';
import DeleteDocumentForm from '../forms/DeleteDocumentForm';


const dmp = new diff_match_patch();

const MyDocuments: React.FC = () => {
  const { user } = useAuth();
  const [documents, setDocuments] = useState<Document[]>([]);
  const [activeDocumentId, setActiveDocumentId] = useState<string | null>(null);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [documentContent, setDocumentContent] = useState('');
  const [currentVersion, setCurrentVersion] = useState<number>(0);
  const [operationsQueue, setOperationsQueue] = useState<Operation[]>([]);
  const activeDocumentIdRef = useRef<string | null>(null);
  const { isOpen, openModal, closeModal } = useModal();
  const [formType, setFormType] = useState<'add' | 'update' | 'delete' | null>(null);
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const openDocumentModal = (document: Document | null, type: 'add' | 'update' | 'delete') => {
    setSelectedDocument(document);
    setFormType(type);
    openModal();
  };

  const refreshDocuments = () => {
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
  };

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

  useEffect(() => {
    const initializeConnection = async () => {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5019/hubs/documents',
          {accessTokenFactory: () => user?.token}
        )
        .withAutomaticReconnect()
        .build();

      newConnection.on('JoinedDocument', (operations : Operation[], version : number, joinedDocumentId) => {

        for (const operation of operations){
          setDocumentContent((prevContent) => applyOperationToContent(prevContent, operation));
          setCurrentVersion(version);
        }

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
    const selectedDocument = documents.find((doc) => doc.id.toString() === documentId);
    setActiveDocumentId(documentId);
    setDocumentContent(selectedDocument?.text || '');
    setCurrentVersion(0);
    console.log('Selected document:', documentId, selectedDocument);
  };

  const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const newContent = e.target.value;
    setDocumentContent(newContent);
  
    const operations = detectOperations(documentContent, newContent);
  
    setOperationsQueue((prevOps) => [...prevOps, ...operations]);
  };

  useEffect(() => {
    if (operationsQueue.length > 0) {
      sendChanges(operationsQueue);
    }
  }, [operationsQueue]);

  const detectOperations = (oldContent: string, newContent: string) => {
    const operations: Operation[] = [];
  
    const diffs = dmp.diff_main(oldContent, newContent);
    
    dmp.diff_cleanupSemantic(diffs);
  
    let currentPos = 0;
  
    diffs.forEach(([operation, text]) => {
      switch (operation) {
        case 0:
          currentPos += text.length;
          break;
  
        case 1:
          operations.push(Operation.createInsertOp(currentPos, text, user?.user.id || 0));
          currentPos += text.length;
          break;
  
        case -1:
          operations.push(Operation.createDeleteOp(currentPos, text, user?.user.id || 0));
          break;
  
        default:
          console.error('Unknown operation type');
      }
    });
  
    return operations;
  };

  const sendChanges = async (operations: Operation[]) => {
    if (connection && operations.length > 0 && activeDocumentIdRef.current) {
      try {
        const currentQueue = [...operations];
        setOperationsQueue([]);
  
        for (const operation of currentQueue) {
          console.log('Sending operation with version:', operation);
          await connection.invoke('SendOperation', operation, currentVersion);
        }
      } catch (err) {
        console.error('Error sending changes:', err);
      }
    }
  };

  const handleReceivedOperation = (operation: Operation, version: number) => {

    const oldOp = new Operation(operation.Type, operation.Pos, operation.Text, operation.UserID);

    for (let i = 0; i < operationsQueue.length; i++){
      operation = operation.transform(operationsQueue[i], true);
      operationsQueue[i] = operationsQueue[i].transform(oldOp);
    }

    setDocumentContent((prevContent) => applyOperationToContent(prevContent, operation));
    setCurrentVersion(version);
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
                activeDocumentId === doc?.id?.toString() ? 'bg-gray-200' : ''
              }`}
            >
              <div className="flex justify-between items-center">
                <span onClick={() => handleDocumentSelect(doc.id.toString())}>
                  {doc.documentName}
                </span>
                  <button
                    onClick={() =>
                      openDocumentModal(doc, user?.user.id === doc.ownerId ? 'update' : 'delete')
                    }
                    className="ml-2 text-gray-700 hover:text-black transition duration-200"
                  >
                    ⚙️
                  </button>
              </div>
            </li>
          ))}
        </ul>
        <button
          className="btn mt-4 hover:bg-slate-500 hover:text-white transition-all duration-300"
          onClick={() => openDocumentModal(null, 'add')}
        >
          Добавить документ
        </button>
      </div>

      {isOpen && formType === 'add' && (
        <div className="modal-overlay">
          <div className="modal-content">
            <AddDocumentForm closeModal={closeModal} refreshDocuments={refreshDocuments} />
          </div>
        </div>
      )}

      {isOpen && formType === 'update' && selectedDocument && (
        <div className="modal-overlay">
          <div className="modal-content">
            <UpdateDocumentForm
              documentId={selectedDocument.id}
              //currentEditors={selectedDocument.editors.map((e) => e.username)}
              closeModal={closeModal}
              refreshDocuments={refreshDocuments}
            />
          </div>
        </div>
      )}

      {isOpen && formType === 'delete' && selectedDocument && (
        <div className="modal-overlay">
          <div className="modal-content">
            <DeleteDocumentForm
              documentId={selectedDocument.id}
              closeModal={closeModal}
              refreshDocuments={refreshDocuments}
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default MyDocuments;

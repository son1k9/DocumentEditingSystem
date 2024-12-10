import React, { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getDocumentsForUser } from "../services/documentService";
import { Document } from "../models/Document";
import DocumentCard from "../components/DocumentCard";
import useModal from '../hooks/useModal';
import AddDocumentForm from '../forms/AddDocumentForm';
import UpdateDocumentForm from '../forms/UpdateDocumentForm';
import DeleteDocumentForm from '../forms/DeleteDocumentForm';

const DocumentsPage: React.FC = () => {
  const { user } = useAuth();
  const [documents, setDocuments] = useState<Document[]>([]);
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
  const [formType, setFormType] = useState<'add' | 'update' | 'delete' | null>(null);
  const { isOpen, openModal, closeModal } = useModal();

  const refreshDocuments = () => {
    const fetchDocuments = async () => {
      try{
          const fetchDocuments = await getDocumentsForUser();
          setDocuments(fetchDocuments);
      }
      catch (error)
      {
        console.error('Ошибка загрузки документов', error);
      }
    }

    fetchDocuments();
  };

  useEffect(() => {
    refreshDocuments();
  });

  if (!documents) {
    return <div>Загрузка документов...</div>;
  }

  const openDocumentModal = (document: Document | null, type: 'add' | 'update' | 'delete') => {
    setSelectedDocument(document);
    setFormType(type);
    openModal();
  };


  return (
    <div style={styles.container}>

      <h1 style={styles.header}>Документы</h1>

      <button 
        className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
        onClick={() => openDocumentModal(null, 'add')}>
        Добавить
      </button>

      <div style={styles.cardContainer}>
        {documents.map((doc) => (
          <DocumentCard
            key={doc.id}
            document={doc}
            onSettingsClick={() => openDocumentModal(doc, user?.user.id === doc.ownerId ? 'update' : 'delete')} />
        ))}
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
              editors={selectedDocument.editors.map((editor) => editor.toString())}
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
}

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    display: 'flex',
    flexDirection: 'column',
    margin: 0,
    padding: 10,
    backgroundColor: '#f4f4f4',
    fontFamily: 'Arial, sans-serif',
    alignSelf: 'start',
    alignItems: 'center'
  },
  header: {
    textAlign: 'center',
    padding: '20px',
    fontSize: '1.5rem',
    width: '100%',
    fontWeight: 'bold'
  },
  cardContainer: {
    marginTop: 10,
    display: 'flex',
    flex: '1 0 21%',
    flexWrap: 'wrap',
    flexDirection: 'row',
    justifyContent: 'center',
    gap: '20px',
  }
};


export default DocumentsPage;;
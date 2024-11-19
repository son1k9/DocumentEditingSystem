import React from 'react';
import { useForm } from 'react-hook-form';
import { updateDocument, deleteDocument } from '../services/documentService';

interface UpdateDocumentFormProps {
  documentId: number;
//  currentEditors: string[];
  closeModal: () => void;
  refreshDocuments: () => void;
}

const UpdateDocumentForm: React.FC<UpdateDocumentFormProps> = ({
  documentId,
//  currentEditors,
  closeModal,
  refreshDocuments,
}) => {
  const { register, handleSubmit, reset } = useForm<{ editors: string[] }>();

  const onSubmit = async (data: { editors: string[] }) => {
    try {
      await updateDocument(documentId);
      refreshDocuments();
      closeModal();
    } catch (error) {
      console.error('Error updating editors:', error);
    }
  };

  const handleDelete = async () => {
    try {
      await deleteDocument(documentId);
      refreshDocuments();
      closeModal();
    } catch (error) {
      console.error('Error deleting document:', error);
    }
  };

  return (
    <div className="p-4 bg-white rounded shadow-lg">
      <h2 className="text-xl font-bold mb-4">Управление документом</h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="mb-4">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Редакторы (Usernames)
          </label>
        </div>
        <div className="flex justify-between">
          <button type="submit" className="btn-primary">
            Обновить редакторов
          </button>
          <button type="button" onClick={handleDelete} className="btn-danger">
            Удалить документ
          </button>
        </div>
      </form>
    </div>
  );
};

export default UpdateDocumentForm;

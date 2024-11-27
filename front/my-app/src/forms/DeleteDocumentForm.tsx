import React from 'react';
import { deleteFromEditor} from '../services/documentService';

interface DeleteDocumentFormProps {
  documentId: number;
  closeModal: () => void;
  refreshDocuments: () => void;
}

const DeleteDocumentForm: React.FC<DeleteDocumentFormProps> = ({
  documentId,
  closeModal,
  refreshDocuments,
}) => {
  const handleDelete = async () => {
    try {
      await deleteFromEditor(documentId);
      refreshDocuments();
      closeModal();
    } catch (error) {
      console.error('Error deleting document for user:', error);
    }
  };

  return (
    <div className="p-4 bg-white rounded shadow-lg">
      <h2 className="text-xl font-bold mb-4">Удаление документа</h2>
      <p>Вы уверены, что хотите удалить этот документ из вашего списка?</p>
      <div className="flex justify-end mt-4">
        <button
          onClick={closeModal}
          className="btn-secondary mr-2"
        >
          Отмена
        </button>
        <button
          onClick={handleDelete}
          className="btn-danger"
        >
          Удалить
        </button>
      </div>
    </div>
  );
};

export default DeleteDocumentForm;

import React from 'react';
import { useForm } from 'react-hook-form';
import { updateDocumentEditors, deleteDocument } from '../services/documentService';

interface UpdateDocumentFormProps {
  documentId: number;
  editors: string[];
  closeModal: () => void;
  refreshDocuments: () => void;
}

const UpdateDocumentForm: React.FC<UpdateDocumentFormProps> = ({
  documentId,
  editors,
  closeModal,
  refreshDocuments,
}) => {
  const { register, handleSubmit, reset } = useForm<{ editors: string }>({
    defaultValues: { editors: editors.join(',') },
  });

  const onSubmit = async (data: { editors: string }) => {
    try {
      const editorArray = data.editors.split(',').map((e) => e.trim());
      await updateDocumentEditors(documentId, editorArray);
      refreshDocuments();
      closeModal();
      reset();
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
      <label>Редакторы (Username через запятую):</label>
      <input className='w-[90%]'
        {...register('editors')}
        placeholder="Введите ID редакторов через запятую"
        defaultValue={editors.join(',')}
      />
      <br></br>
      <button type="submit">Обновить</button>
      <br></br>
      <button type="button" onClick={closeModal}>
        Отмена
      </button>
      <br></br>
      <button type="button" onClick={handleDelete}>
        Удалить документ
      </button>
    </form>
    </div>
  );
};

export default UpdateDocumentForm;

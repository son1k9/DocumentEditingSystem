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
    <div className="p-4 bg-white rounded">
      <h2 className="text-xl font-bold mb-4">Управление документом</h2>
      <form onSubmit={handleSubmit(onSubmit)}>
      <label>Редакторы (Имена пользователей через запятую):</label>
      <input className='w-[90%] w-full p-2 border rounded-md focus:outline-none focus:ring focus:ring-blue-300`}'
        {...register('editors')}
        placeholder="Введите редакторов через запятую"
        defaultValue={editors.join(',')}
      />
      <br></br>
      <br></br>
      <button type="submit" className="bg-green-500 text-white py-2 px-4 rounded hover:bg-green-600">Обновить</button>
      <br></br>
      <button type="button" onClick={closeModal} className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
        Отмена
      </button>
      <br></br>
      <button type="button" onClick={handleDelete} className="mt-2 px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600">
        Удалить документ
      </button>
    </form>
    </div>
  );
};

export default UpdateDocumentForm;

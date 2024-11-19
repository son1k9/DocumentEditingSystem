import apiClient from './apiClient';
import { Document } from '../models/Document';

export const getDocumentsForUser = async (): Promise<Document[]> => {
    try {
      const response = await apiClient.get(`/Documents/GetAvailableDocuments`);
      return response.data;
    } catch (error) {
      console.error('Ошибка при получении документов:', error);
      throw new Error('Не удалось загрузить документы');
    }
  };

export const addDocument = async (formData: FormData): Promise<void> => {
  try{
    const response = await apiClient.post('/documents/LoadDocument', formData,{
      headers:{
        "Content-Type": 'multipart/form-data',
      },
    });

    return response.data;
  }
  catch(error)
  {
    console.error('Ошибка при добавлении документа:', error);
    throw new Error('Не удалось добавить документ');
  }
};

export const updateDocument = async (documentId: number): Promise<void> =>{
  throw new Error();
}

export const deleteDocument = async (documentId: number): Promise<void> =>{
  try {
    const response = await apiClient.post(`/Documents/DeleteDocument?documentId=${documentId}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении документов:', error);
    throw new Error('Не удалось загрузить документы');
  }
}

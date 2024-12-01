import apiClient from './apiClient';
import { Document } from '../models/Document';

interface DocumentContent{
  id : number,
  documentName: string,
  text : string,
  version: number,
  ownerId: number,
  editors: string[]
}

export const getDocumentsForUser = async (): Promise<Document[]> => {
    try {
      const response = await apiClient.get(`/Documents/GetAvailableDocuments`);
      return response.data;
    } catch (error) {
      console.error('Ошибка при получении документов:', error);
      throw new Error('Не удалось загрузить документы');
    }
  };

export const getDocumentById = async (id: number): Promise<DocumentContent> => {
    try {
      const response = await apiClient.get(`/Documents/${id}`);
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

export const updateDocumentEditors = async (documentId: number, editors: string[]): Promise<void> =>{
  try {
    const params = new URLSearchParams();
    editors.forEach((editor) => params.append('editors', editor));

    const response = await apiClient.patch(`/Documents/updateEditors/${documentId}?${params.toString()}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при обновлении документа:', error);
    throw new Error('Ошибка при обновлении документа');
  }
}

export const deleteFromEditor = async (documentId: number): Promise<void> =>{
  try {
    const response = await apiClient.patch(`/Documents/deleteFromEditor/${documentId}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при обновлении документа:', error);
    throw new Error('Ошибка при обновлении документа');
  }
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

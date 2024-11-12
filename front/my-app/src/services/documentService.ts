import apiClient from './apiClient';
import { Document } from '../models/Document';

export const getDocumentsForUser = async (userId: number): Promise<Document[]> => {
    try {
      const response = await apiClient.get(`/api/documents`, {
        params: { userId },
      });
      return response.data;
    } catch (error) {
      console.error('Ошибка при получении документов:', error);
      throw new Error('Не удалось загрузить документы');
    }
  };
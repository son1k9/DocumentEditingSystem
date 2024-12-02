import axios from 'axios';
import Cookies from 'js-cookie';
import { useAuth } from '../context/AuthContext';

const apiClient = axios.create({
  baseURL: 'http://localhost:5019/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

async function refreshToken() {
  try {
    const refreshToken = Cookies.get('refresh_token');

    if (!refreshToken) {
      throw new Error('Refresh token отсутствует');
    }

    const userJson = Cookies.get('user');

    if (!userJson){
      throw new Error('User отсутствует');
    }

    const user = JSON.parse(userJson);
    
    const response = await axios.post('http://localhost:5019/api/users/GetAccessToken', {
      refreshToken: refreshToken,
      username: user.user.username
    });

    const newAccessToken = response.data.access_token;
    Cookies.set('token', newAccessToken);
    return newAccessToken;
  } catch (error) {
    console.error('Ошибка при обновлении токена:', error);
    throw error;
  }
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (
      error.response &&
      error.response.status === 401 &&
      !originalRequest._retry &&
      Cookies.get('refreshToken')
    ) {
      originalRequest._retry = true;

      try {
        const newAccessToken = await refreshToken();
        originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        console.error('Не удалось обновить токен:', refreshError);
        const { logout } = useAuth();
        logout();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);


apiClient.interceptors.request.use(
  (config) => {
    const token = Cookies.get('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default apiClient;
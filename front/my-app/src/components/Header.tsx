import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Header: React.FC = () => {
  const { isAuthenticated, logout } = useAuth(); // Используем контекст аутентификации

  return (
    <header className="bg-blue-500 text-white p-4">
      <nav className="flex justify-between items-center">
        <ul className="flex space-x-4">
          <li>
            <Link to="/" className="hover:text-gray-200">
              Главная
            </Link>
          </li>
          <li>
            <Link to="/documents" className="hover:text-gray-200">
              Мои документы
            </Link>
          </li>
        </ul>
        
        <ul className="flex space-x-4">
          {isAuthenticated ? (
            <>
              <li>
                <Link to="/account" className="hover:text-gray-200">
                  Личный кабинет
                </Link>
              </li>
              <li>
                <button onClick={logout} className="hover:text-gray-200">
                  Выйти
                </button>
              </li>
            </>
          ) : (
            <li>
              <Link to="/login" className="hover:text-gray-200">
                Логин
              </Link>
            </li>
          )}
        </ul>
      </nav>
    </header>
  );
};

export default Header;

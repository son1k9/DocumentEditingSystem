import React from "react";
import { Link } from "react-router-dom";

const Header: React.FC = () => {
    return (
        <header className="bg-blue-500 text-white p-4">
            <nav>
                <ul className="flex space-x-4">
                    <li>
                        <Link to="/" className="hover:text-gray-200">
                            Главная
                        </Link>
                    </li>
                    <li>
                        <Link to="/about" className="hover:text-gray-200">
                            О нас
                        </Link>
                    </li>
                    <li>
                        <Link to="/login" className="hover:text-gray-200">
                            Логин
                        </Link>
                    </li>
                    <li>
                        <Link to="/register" className="hover:text-gray-200">
                            Регистрация
                        </Link>
                    </li>
                    <li>
                        <Link to="/dashboard" className="hover:text-gray-200">
                            Личный кабинет
                        </Link>
                    </li>
                </ul>
            </nav>
        </header>
    )
}

export default Header;
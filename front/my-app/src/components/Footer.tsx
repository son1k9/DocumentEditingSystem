import React from "react";

const Footer: React.FC = () => {
    return (
        <footer className="bg-gray-800 text-white p-4">
            <p className="text-center">© {new Date().getFullYear()} Все права защищены.</p>
        </footer>
    );
};

export default Footer;
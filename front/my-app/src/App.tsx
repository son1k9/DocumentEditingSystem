import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import About from './pages/About';
import Login from './pages/Login';
import Register from './pages/Register';
import PrivateRoute from './components/PrivateRoute';
import Dashboard from './pages/Dashboard'; // Не забудьте импортировать Dashboard
import { AuthProvider } from './context/AuthContext'; // Импортируйте AuthProvider

import './styles/global.css';

const App: React.FC = () => {
  return (
    <AuthProvider> {/* Оборачиваем приложение в AuthProvider */}
      <Router>
        <div className="container mx-auto px-4">
          <Header />
          <main>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/about" element={<About />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              
              {/* Приватный маршрут для Dashboard */}
              <Route element={<PrivateRoute />}>
                <Route path="/dashboard" element={<Dashboard />} />
              </Route>
            </Routes>
          </main>
          <Footer />
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;

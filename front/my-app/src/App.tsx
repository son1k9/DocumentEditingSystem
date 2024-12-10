import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import PrivateRoute from './components/PrivateRoute';
import Dashboard from './pages/Dashboard';
import { AuthProvider } from './context/AuthContext';

import './styles/global.css';
import DocumentsPage from './pages/DocumentsPage';
import DocumentPage from './pages/DocumentPage';
import DocumentPageView from './pages/DocumentPageView';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="flex flex-col min-h-screen">
          <Header />
          <main className="flex-grow bg-gray-100 flex items-center justify-center">
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route element={<PrivateRoute />}>
              <Route path='/documents' element={<DocumentsPage />} />
              <Route path='/documents/:slug' element={<DocumentPage />} />
              <Route path='/documents/:slug/view' element={<DocumentPageView />} />
              <Route path="/account/*" element={<Dashboard />} />
              </Route>
            </Routes>
          </main>
          {/* Footer */}
          <Footer />
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;

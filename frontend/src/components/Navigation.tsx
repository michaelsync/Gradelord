import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const Navigation: React.FC = () => {
  const { user, logout } = useAuth();
  const location = useLocation();

  const isActive = (path: string) => location.pathname === path;

  const handleLogout = () => {
    logout();
  };

  return (
    <nav className="navigation">
      <div className="nav-container">
        <Link to="/dashboard" className="nav-brand">
          Teach Portal
        </Link>
        
        <ul className="nav-links">
          <li>
            <Link 
              to="/dashboard" 
              className={`nav-link ${isActive('/dashboard') ? 'active' : ''}`}
            >
              Dashboard
            </Link>
          </li>
          <li>
            <Link 
              to="/students" 
              className={`nav-link ${isActive('/students') ? 'active' : ''}`}
            >
              My Students
            </Link>
          </li>
          <li>
            <Link 
              to="/teachers" 
              className={`nav-link ${isActive('/teachers') ? 'active' : ''}`}
            >
              All Teachers
            </Link>
          </li>
        </ul>

        <div className="nav-user">
          <span>Welcome, {user?.firstName} {user?.lastName}</span>
          <button onClick={handleLogout} className="logout-btn">
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
}; 
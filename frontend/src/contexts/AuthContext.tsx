import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { AuthContextType, Teacher, LoginRequest, RegisterRequest } from '../types';
import { apiService } from '../services/api';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<Teacher | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for existing authentication on mount
    const storedToken = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');

    if (storedToken && storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);
        setToken(storedToken);
        setUser(parsedUser);
      } catch (error) {
        // Clear invalid stored data
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (loginData: LoginRequest): Promise<void> => {
    setIsLoading(true);
    try {
      const response = await apiService.login(loginData);
      const { token: newToken, teacher } = response;

      localStorage.setItem('token', newToken);
      localStorage.setItem('user', JSON.stringify(teacher));
      
      setToken(newToken);
      setUser(teacher);
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (registerData: RegisterRequest): Promise<void> => {
    setIsLoading(true);
    try {
      const response = await apiService.register(registerData);
      const { token: newToken, teacher } = response;

      localStorage.setItem('token', newToken);
      localStorage.setItem('user', JSON.stringify(teacher));
      
      setToken(newToken);
      setUser(teacher);
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = (): void => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
  };

  const value: AuthContextType = {
    user,
    token,
    login,
    register,
    logout,
    isLoading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}; 
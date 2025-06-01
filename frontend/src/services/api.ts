import axios, { AxiosInstance, AxiosResponse } from 'axios';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  Teacher,
  Student,
  CreateStudentRequest,
} from '../types';

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Clear invalid token
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Authentication endpoints
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/login', data);
    return response.data;
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/register', data);
    return response.data;
  }

  async validateToken(token: string): Promise<{ valid: boolean }> {
    const response = await this.api.post('/auth/validate-token', token);
    return response.data;
  }

  // Teacher endpoints
  async getAllTeachers(): Promise<Teacher[]> {
    const response: AxiosResponse<Teacher[]> = await this.api.get('/teachers');
    return response.data;
  }

  async getTeacher(id: string): Promise<Teacher> {
    const response: AxiosResponse<Teacher> = await this.api.get(`/teachers/${id}`);
    return response.data;
  }

  async getCurrentTeacher(): Promise<Teacher> {
    const response: AxiosResponse<Teacher> = await this.api.get('/teachers/me');
    return response.data;
  }

  // Student endpoints
  async getMyStudents(): Promise<Student[]> {
    const response: AxiosResponse<Student[]> = await this.api.get('/students');
    return response.data;
  }

  async getStudent(id: string): Promise<Student> {
    const response: AxiosResponse<Student> = await this.api.get(`/students/${id}`);
    return response.data;
  }

  async createStudent(data: CreateStudentRequest): Promise<Student> {
    const response: AxiosResponse<Student> = await this.api.post('/students', data);
    return response.data;
  }

  async updateStudent(id: string, data: CreateStudentRequest): Promise<Student> {
    const response: AxiosResponse<Student> = await this.api.put(`/students/${id}`, data);
    return response.data;
  }

  async deleteStudent(id: string): Promise<void> {
    await this.api.delete(`/students/${id}`);
  }
}

export const apiService = new ApiService(); 
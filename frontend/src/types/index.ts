// Authentication types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expires: string;
  teacher: Teacher;
}

export interface Teacher {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  studentCount: number;
  createdAt: string;
}

// Student types
export interface CreateStudentRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface Student {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  teacherId: string;
  teacherName: string;
  createdAt: string;
}

// API Response types
export interface ApiError {
  message: string;
}

export interface ValidationError {
  [key: string]: string[];
}

// Context types
export interface AuthContextType {
  user: Teacher | null;
  token: string | null;
  login: (loginData: LoginRequest) => Promise<void>;
  register: (registerData: RegisterRequest) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

export interface StudentsContextType {
  students: Student[];
  fetchStudents: () => Promise<void>;
  addStudent: (student: CreateStudentRequest) => Promise<void>;
  updateStudent: (id: string, student: CreateStudentRequest) => Promise<void>;
  deleteStudent: (id: string) => Promise<void>;
  isLoading: boolean;
} 
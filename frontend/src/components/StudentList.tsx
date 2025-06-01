import React, { useState, useEffect } from 'react';
import { apiService } from '../services/api';
import { Student, CreateStudentRequest } from '../types';

interface StudentModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (student: CreateStudentRequest) => Promise<void>;
  student?: Student | null;
  isSubmitting: boolean;
}

const StudentModal: React.FC<StudentModalProps> = ({ 
  isOpen, 
  onClose, 
  onSubmit, 
  student, 
  isSubmitting 
}) => {
  const [formData, setFormData] = useState<CreateStudentRequest>({
    firstName: '',
    lastName: '',
    email: '',
  });
  const [errors, setErrors] = useState<Partial<CreateStudentRequest>>({});

  useEffect(() => {
    if (student) {
      setFormData({
        firstName: student.firstName,
        lastName: student.lastName,
        email: student.email,
      });
    } else {
      setFormData({
        firstName: '',
        lastName: '',
        email: '',
      });
    }
    setErrors({});
  }, [student, isOpen]);

  const validateForm = (): boolean => {
    const newErrors: Partial<CreateStudentRequest> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = 'First name is required';
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = 'Last name is required';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    await onSubmit(formData);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    if (errors[name as keyof CreateStudentRequest]) {
      setErrors(prev => ({
        ...prev,
        [name]: undefined
      }));
    }
  };

  if (!isOpen) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0,0,0,0.5)',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      zIndex: 1000
    }}>
      <div className="form-container" style={{ margin: 0, maxWidth: '500px' }}>
        <h2 className="form-title">
          {student ? 'Edit Student' : 'Add New Student'}
        </h2>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="firstName" className="form-label">
              First Name
            </label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              className="form-input"
              placeholder="Enter first name"
            />
            {errors.firstName && (
              <div className="form-error">{errors.firstName}</div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="lastName" className="form-label">
              Last Name
            </label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              className="form-input"
              placeholder="Enter last name"
            />
            {errors.lastName && (
              <div className="form-error">{errors.lastName}</div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="email" className="form-label">
              Email
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="form-input"
              placeholder="Enter email"
            />
            {errors.email && (
              <div className="form-error">{errors.email}</div>
            )}
          </div>

          <div className="action-buttons">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Saving...' : (student ? 'Update Student' : 'Add Student')}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="btn btn-secondary"
              disabled={isSubmitting}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export const StudentList: React.FC = () => {
  const [students, setStudents] = useState<Student[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingStudent, setEditingStudent] = useState<Student | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchStudents = async () => {
    try {
      setIsLoading(true);
      const data = await apiService.getMyStudents();
      setStudents(data);
    } catch (error: any) {
      setError('Failed to load students');
      console.error('Students fetch error:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  const handleAddStudent = () => {
    setEditingStudent(null);
    setIsModalOpen(true);
  };

  const handleEditStudent = (student: Student) => {
    setEditingStudent(student);
    setIsModalOpen(true);
  };

  const handleDeleteStudent = async (studentId: string) => {
    if (!window.confirm('Are you sure you want to delete this student?')) {
      return;
    }

    try {
      await apiService.deleteStudent(studentId);
      setStudents(prev => prev.filter(s => s.id !== studentId));
    } catch (error: any) {
      setError('Failed to delete student');
      console.error('Delete student error:', error);
    }
  };

  const handleSubmitStudent = async (studentData: CreateStudentRequest) => {
    try {
      setIsSubmitting(true);
      
      if (editingStudent) {
        const updatedStudent = await apiService.updateStudent(editingStudent.id, studentData);
        setStudents(prev => prev.map(s => s.id === editingStudent.id ? updatedStudent : s));
      } else {
        const newStudent = await apiService.createStudent(studentData);
        setStudents(prev => [...prev, newStudent]);
      }
      
      setIsModalOpen(false);
      setEditingStudent(null);
    } catch (error: any) {
      setError(editingStudent ? 'Failed to update student' : 'Failed to add student');
      console.error('Submit student error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingStudent(null);
  };

  if (isLoading) {
    return <div className="loading">Loading students...</div>;
  }

  return (
    <div>
      <div className="dashboard-header">
        <h1>My Students</h1>
        <p>Manage your students and their information</p>
      </div>

      {error && (
        <div className="form-error" style={{ marginBottom: '2rem', textAlign: 'center' }}>
          {error}
        </div>
      )}

      <div className="add-button">
        <button onClick={handleAddStudent} className="btn btn-primary">
          Add New Student
        </button>
      </div>

      {students.length === 0 ? (
        <div className="card">
          <p>No students found. Add your first student to get started!</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Added</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.fullName}</td>
                  <td>{student.email}</td>
                  <td>{new Date(student.createdAt).toLocaleDateString()}</td>
                  <td>
                    <div className="action-buttons">
                      <button
                        onClick={() => handleEditStudent(student)}
                        className="btn btn-secondary btn-sm"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteStudent(student.id)}
                        className="btn btn-danger btn-sm"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="dashboard-stats" style={{ marginTop: '2rem' }}>
        <div className="stat-card">
          <div className="stat-number">{students.length}</div>
          <div className="stat-label">Total Students</div>
        </div>
      </div>

      <StudentModal
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        onSubmit={handleSubmitStudent}
        student={editingStudent}
        isSubmitting={isSubmitting}
      />
    </div>
  );
}; 
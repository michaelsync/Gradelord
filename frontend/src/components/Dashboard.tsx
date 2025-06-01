import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { apiService } from '../services/api';
import { Student, Teacher } from '../types';

export const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [students, setStudents] = useState<Student[]>([]);
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        const [studentsData, teachersData] = await Promise.all([
          apiService.getMyStudents(),
          apiService.getAllTeachers()
        ]);
        setStudents(studentsData);
        setTeachers(teachersData);
      } catch (error: any) {
        setError('Failed to load dashboard data');
        console.error('Dashboard data fetch error:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  if (isLoading) {
    return <div className="loading">Loading dashboard...</div>;
  }

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Welcome back, {user?.firstName}!</h1>
        <p>Here's an overview of your teaching portal</p>
      </div>

      {error && (
        <div className="form-error" style={{ marginBottom: '2rem', textAlign: 'center' }}>
          {error}
        </div>
      )}

      <div className="dashboard-stats">
        <div className="stat-card">
          <div className="stat-number">{students.length}</div>
          <div className="stat-label">My Students</div>
        </div>
        
        <div className="stat-card">
          <div className="stat-number">{teachers.length}</div>
          <div className="stat-label">Total Teachers</div>
        </div>
        
        <div className="stat-card">
          <div className="stat-number">
            {teachers.reduce((total, teacher) => total + teacher.studentCount, 0)}
          </div>
          <div className="stat-label">Total Students</div>
        </div>
      </div>

      <div className="card-grid">
        <div className="card">
          <h3 className="card-title">My Students</h3>
          <p>Manage your students, add new ones, and track their information.</p>
          <Link to="/students" className="btn btn-primary">
            View My Students
          </Link>
        </div>

        <div className="card">
          <h3 className="card-title">All Teachers</h3>
          <p>Browse all teachers in the system and see their student counts.</p>
          <Link to="/teachers" className="btn btn-secondary">
            View All Teachers
          </Link>
        </div>

        <div className="card">
          <h3 className="card-title">Quick Actions</h3>
          <p>Quickly add a new student to your class.</p>
          <Link to="/students" className="btn btn-primary">
            Add New Student
          </Link>
        </div>
      </div>

      {students.length > 0 && (
        <div className="card">
          <h3 className="card-title">Recent Students</h3>
          <div className="table-container">
            <table className="table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Added</th>
                </tr>
              </thead>
              <tbody>
                {students.slice(0, 5).map((student) => (
                  <tr key={student.id}>
                    <td>{student.fullName}</td>
                    <td>{student.email}</td>
                    <td>{new Date(student.createdAt).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}; 
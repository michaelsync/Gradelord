import React, { useState, useEffect } from 'react';
import { apiService } from '../services/api';
import { Teacher } from '../types';

export const TeacherList: React.FC = () => {
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchTeachers = async () => {
      try {
        setIsLoading(true);
        const data = await apiService.getAllTeachers();
        setTeachers(data);
      } catch (error: any) {
        setError('Failed to load teachers');
        console.error('Teachers fetch error:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchTeachers();
  }, []);

  if (isLoading) {
    return <div className="loading">Loading teachers...</div>;
  }

  return (
    <div>
      <div className="dashboard-header">
        <h1>All Teachers</h1>
        <p>Browse all teachers in the system and their student counts</p>
      </div>

      {error && (
        <div className="form-error" style={{ marginBottom: '2rem', textAlign: 'center' }}>
          {error}
        </div>
      )}

      {teachers.length === 0 ? (
        <div className="card">
          <p>No teachers found.</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Username</th>
                <th>Email</th>
                <th>Student Count</th>
                <th>Joined</th>
              </tr>
            </thead>
            <tbody>
              {teachers.map((teacher) => (
                <tr key={teacher.id}>
                  <td>{teacher.fullName}</td>
                  <td>{teacher.username}</td>
                  <td>{teacher.email}</td>
                  <td>
                    <span className="stat-number" style={{ fontSize: '1.2rem' }}>
                      {teacher.studentCount}
                    </span>
                  </td>
                  <td>{new Date(teacher.createdAt).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="dashboard-stats" style={{ marginTop: '2rem' }}>
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
        
        <div className="stat-card">
          <div className="stat-number">
            {teachers.length > 0 
              ? Math.round(teachers.reduce((total, teacher) => total + teacher.studentCount, 0) / teachers.length)
              : 0
            }
          </div>
          <div className="stat-label">Avg Students per Teacher</div>
        </div>
      </div>
    </div>
  );
}; 
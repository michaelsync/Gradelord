# Teach Portal - Full-Stack Student Management System

A modern full-stack web application for teachers to manage students and view other teachers in the system. Built with .NET 8, React 18, PostgreSQL, and Docker.

## 🚀 Features

### Authentication & Authorization

- **User Registration**: Teachers can sign up with username, email, first name, last name, and password
- **Secure Login**: JWT-based authentication with token validation
- **Protected Routes**: Frontend route protection based on authentication status

### Student Management

- **CRUD Operations**: Create, read, update, and delete students
- **Teacher Association**: Students are automatically associated with the logged-in teacher
- **Data Validation**: Comprehensive form validation on both frontend and backend
- **Modal Interface**: User-friendly modal forms for adding/editing students

### Teacher Directory

- **View All Teachers**: Browse all registered teachers in the system
- **Student Counts**: See how many students each teacher has
- **Statistics**: Dashboard with aggregated statistics

### Dashboard

- **Overview Statistics**: Quick stats showing student counts and teacher information
- **Recent Students**: Display recently added students
- **Quick Actions**: Easy navigation to key features

## 🏗️ Architecture

DRY, SOLID, DDD

### Backend (.NET 8)

- **Clean Architecture**: Organized into Domain, Application, Infrastructure, and API layers
- **Entity Framework Core**: Code-first approach with PostgreSQL
- **JWT Authentication**: Secure token-based authentication
- **Repository Pattern**: Abstracted data access layer
- **Dependency Injection**: Built-in .NET DI container
- **Swagger Documentation**: Auto-generated API documentation

### Frontend (React 18)

- **TypeScript**: Full type safety throughout the application
- **React Router**: Client-side routing with protected routes
- **Context API**: State management for authentication
- **Axios**: HTTP client with interceptors for API calls
- **Responsive Design**: Mobile-friendly CSS with modern styling
- **Form Validation**: Client-side validation with error handling

### Database (PostgreSQL)

- **Relational Design**: Proper foreign key relationships
- **Entity Relationships**: One-to-many relationship between Teachers and Students
- **Audit Fields**: Created/updated timestamps on all entities
- **Unique Constraints**: Username and email uniqueness enforcement

### DevOps (Docker)

- **Multi-container Setup**: Separate containers for database, backend, and frontend
- **Health Checks**: Database health monitoring
- **Volume Persistence**: Data persistence for PostgreSQL
- **Network Isolation**: Custom Docker network for service communication

## 📋 Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for local development)
- PostgreSQL (for local development)

## 🚀 Quick Start with Docker

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd Gradelord
   ```

2. **Start all services**

   ```bash
   docker-compose up --build -d
   ```

3. **Access the application**

   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger Documentation: http://localhost:5000

4. **Create your first teacher account**
   - Navigate to http://localhost:3000
   - Click "Register here" to create a new account
   - Fill in your details and start managing students!

## 🛠️ Local Development Setup

### Backend Setup

1. **Navigate to backend directory**

   ```bash
   cd backend
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)

   - Edit `src/TeachPortal.API/appsettings.json`
   - Update the `ConnectionStrings:DefaultConnection`

4. **Run the application**
   ```bash
   dotnet run --project src/TeachPortal.API
   ```

### Frontend Setup

1. **Navigate to frontend directory**

   ```bash
   cd frontend
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm start
   ```

### Database Setup

1. **Install PostgreSQL** locally or use Docker:

   ```bash
   docker run --name teach-portal-db -e POSTGRES_DB=TeachPortalDB -e POSTGRES_USER=teachadmin -e POSTGRES_PASSWORD=Teacher123! -p 5432:5432 -d postgres:16-alpine
   ```

2. **Database will be created automatically** when the backend starts

## 📁 Project Structure

```
Gradelord/
├── backend/
│   ├── src/
│   │   ├── TeachPortal.Domain/          # Domain entities and interfaces
│   │   ├── TeachPortal.Application/     # Business logic and DTOs
│   │   ├── TeachPortal.Infrastructure/  # Data access and repositories
│   │   └── TeachPortal.API/            # Web API controllers and configuration
│   ├── Dockerfile
│   └── TeachPortal.sln
├── frontend/
│   ├── src/
│   │   ├── components/                  # React components
│   │   ├── contexts/                    # React contexts
│   │   ├── services/                    # API services
│   │   ├── types/                       # TypeScript type definitions
│   │   ├── App.tsx                      # Main App component
│   │   └── index.tsx                    # Entry point
│   ├── public/                          # Static assets
│   ├── Dockerfile
│   └── package.json
├── docker-compose.yml                   # Multi-container orchestration
└── README.md
```

## 🔧 Configuration

### Environment Variables

#### Backend

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `JWT__SecretKey`: Secret key for JWT token signing
- `JWT__Issuer`: JWT token issuer
- `JWT__Audience`: JWT token audience
- `JWT__ExpiryInMinutes`: Token expiration time

#### Frontend

- `REACT_APP_API_URL`: Backend API base URL
- `REACT_APP_ENVIRONMENT`: Environment name

### Database Configuration

The application uses PostgreSQL with the following default settings:

- **Database**: TeachPortalDB
- **Username**: teachadmin
- **Password**: Teacher123!
- **Port**: 5432

## 🔐 Security Features

- **Password Hashing**: BCrypt for secure password storage
- **JWT Tokens**: Stateless authentication with configurable expiration
- **CORS Configuration**: Properly configured for frontend-backend communication
- **Input Validation**: Comprehensive validation on both client and server
- **SQL Injection Protection**: Entity Framework parameterized queries
- **Authorization**: Route-level authorization with JWT validation

## 📊 API Endpoints

### Authentication

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/validate-token` - Token validation

### Teachers

- `GET /api/teachers` - Get all teachers
- `GET /api/teachers/{id}` - Get teacher by ID
- `GET /api/teachers/me` - Get current teacher

### Students

- `GET /api/students` - Get current teacher's students
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

## 🧪 Testing

### Backend Testing

```bash
cd backend
dotnet test
```

### Frontend Testing

```bash
cd frontend
npm test
```

## 🚀 Deployment

### Production Docker Build

```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Manual Deployment

1. Build backend: `dotnet publish -c Release`
2. Build frontend: `npm run build`
3. Deploy to your hosting platform

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License.

## 🆘 Troubleshooting

### Common Issues

1. **Database Connection Issues**

   - Ensure PostgreSQL is running
   - Check connection string in appsettings.json
   - Verify database credentials

2. **Frontend API Connection Issues**

   - Check REACT_APP_API_URL environment variable
   - Ensure backend is running on correct port
   - Verify CORS configuration

3. **JWT Token Issues**
   - Check JWT secret key configuration
   - Verify token expiration settings
   - Clear browser localStorage if needed

### Docker Issues

1. **Port Conflicts**

   - Check if ports 3000, 5000, or 5432 are already in use
   - Modify docker-compose.yml port mappings if needed

2. **Build Failures**
   - Run `docker-compose down` and `docker-compose up --build`
   - Check Docker logs: `docker-compose logs [service-name]`

## 📞 Support

For support and questions, please open an issue in the repository or contact the development team.

---

**Built with ❤️ using .NET 8, React 18, PostgreSQL, and Docker**

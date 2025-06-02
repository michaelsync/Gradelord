# Teach Portal - Full-Stack Student Management System

A modern full-stack web application for teachers to manage students and view other teachers in the system. Built with .NET 8, React 18, PostgreSQL, and Docker.

# How to run the application

## 📋 Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for local development)
- PostgreSQL (for local development)
- Windows WSL - Enable long path

> [!NOTE]
> I tested this application on Windows 11 with WSL2. I have the WSL and Docker integration enabled. I also enabled the [long file path support](https://learn.microsoft.com/en-us/answers/questions/1805411/how-to-enable-long-file-path-names-in-windows-11) . The WSL2 has .NET 8 SKD, Nodejs 24.1 (asdf).
> I used to use the Visual Studio Enterprise editon or Pro edition at work but I don't have the personal license anymore. I still have the community editon but I use the vscode or the cursor most of the times now.

## 🚀 Quick Start with Docker

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd Gradelord
   ```

2. **Start all services**

   ```bash
   docker-compose up -d
   ```

> [!TIP]
> Docker Compose will use existing images if they're already built.It only builds images if they don't exist yet.If we need to rebuild due to the code change, we can use `docker-compose build frontend --no-cache` or `docker-compose build backend --no-cache` or `docker-compose up -d --build --force-recreate`

3. **Access the application**

   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger Documentation: http://localhost:5000

4. **Create your first teacher account**
   - Navigate to http://localhost:3000
   - Click "Register here" to create a new account
   - Fill in your details and start managing students!

> [!TIP]
> If you are not using the docker, please check this guide https://github.com/michaelsync/Gradelord/tree/main/docs#%EF%B8%8F-local-development-setup for more instructions.

> [!IMPORTANT]
> The configuration settings can be found here https://github.com/michaelsync/Gradelord/blob/main/docs/README.md#-configuration.

## :battery: Design Patterns

- **Clean Architecture**: The code structure in the backend are created based on the [Clearn Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). The code are organized into domain, interface, infra/db layer. I also use the Entities, Repositories, Services as a part of DDD (Domain Driven Design). When we are adding the features, we can introduce using the Microservice architecture.
- **DRY/SOLID**: The code are written with DRY (Don't repeat yourself) pricipal and SOLID pricipal in mind. The AuthService, TeacherService are the example of Single Responsibility Principle. The IRepository is an example of Open/Closed Principle. They can be extended but can't be modified the existing behivor. The TeacherRepo and StudentRepo are the example of Liskov Substitution Principle where they are the subclass of IRepo but they can be replacable without breaking the program. I also have IAuthService and etc. for Interface Segregation Principle. All interfaces and concrete classes are registered to ASP.NET DI using Dependency Inversion Principle. The example of DRY are BaseEntity class and Repository<T> class that I reuse the logic for saving the audit field. Another example in the Frontend is that I reuse the Auth logic.
- Password Hashing: BCrypt for secure password storage
- JWT Tokens: Stateless authentication with configurable expiration
- SQL Injection Protection: Entity Framework parameterized queries

Please check more information on the [Architecture](https://github.com/michaelsync/Gradelord/blob/main/docs/README.md#%EF%B8%8F-architecture) section and [Project Structure](https://github.com/michaelsync/Gradelord/blob/main/docs/README.md#-project-structure) section.

### Assumptions

- No student or other users such as admins can login
- The teacher can't create the students for other teachers.
- The teacher can see other teachers + the count of students that they created. (not details)
- I assumed that it will be hosted on Docker/kubnerties.

### Enhancement

- Use a proper Azure B2C or AWS Cognito for the user management
- More unit tests, integration tests
- Deploymnet pipeline, PR validation
- MFA, DDos attack prevention
- Loggging, monitoring (e.g. heathcheck, page visit/traffic, errors, cost and etc.), analytics (e.g. user behivor tracking, device tracking and etc.), alerts (e.g. sms, email)
- Load Test, Integration tests, Pen tests
- CICD, code security scanning, code coverage, blue green deployment or canary
- auto-scaling
- HA - high availability
  - "five-nines" availability (e.g. keep the system running even some components are failing. )
  - self-healing (e.g. Fault Tolerance, auto recovery or restart when crash)
  - Disaster recovery (e.g. multiple regions)

Related:

- https://github.com/michaelsync/Gradelord/blob/main/docs/README.md

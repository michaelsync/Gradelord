## How to run

## Design patterns
 * DI and IoC
 * Repository

### Tech Stack
 * Backend: .NET 8
 * Frontend: ReactJS
 * Security: password hashing, session management, and JWT token for authentication

## Requirements
 - Users: teachers (no student or admin)
 - Sign up username, email, firstname, lastname and password.
 - Log in with username and password.
 - create students by providing the student’s firstname, lastname, email for themself (not for other teacter)
 - view the list of students they've created.
 - can view a list of all other teachers in the system along with the number of students they have created.
- Validation, pagnitation, cache

## Assumptions
 - No student or other users such as admins can login
 - The teacher can't create the students for other teachers.
 - The teacher can see other teachers + the count of students that they created. (not details)

## Enchancements

### Features
 - can enable the student login or admin users
 - multi-tenants - E.g. there is a backoffice system that allows the admin to create different schoools.
 - add other features like class work, attendence/leaves, submit homework, community and etc.
 - help page or AI chat for support
### Technical
 - MFA, DDos attack prevention
 - Loggging, monitoring (e.g. heathcheck, page visit/traffic, errors, cost and etc.), analytics (e.g. user behivor tracking, device tracking and etc.), alerts (e.g. sms, email)
 - Load Test, Integration tests, Pen tests
 - CICD, code security scanning, code coverage, blue green deployment or canary
 - auto-scaling
 - HA - high availability
   - "five-nines" availability (e.g. keep the system running even some components are failing. )
   - self-healing (e.g. Fault Tolerance, auto recovery or restart when crash)
   - Disaster recovery (e.g. multiple regions)

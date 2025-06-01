import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { vi } from 'vitest'
import { Login } from '../Login'
import { AuthProvider } from '../../contexts/AuthContext'

// Mock the API service
vi.mock('../../services/api', () => ({
  apiService: {
    login: vi.fn(),
  },
}))

// Mock react-router-dom
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

const renderLogin = () => {
  return render(
    <BrowserRouter>
      <AuthProvider>
        <Login />
      </AuthProvider>
    </BrowserRouter>
  )
}

describe('Login Component', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  test('renders login form', () => {
    renderLogin()
    
    expect(screen.getByText('Login to Teach Portal')).toBeInTheDocument()
    expect(screen.getByLabelText(/username/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument()
  })

  test('shows validation errors for empty fields', async () => {
    renderLogin()
    
    const loginButton = screen.getByRole('button', { name: /login/i })
    fireEvent.click(loginButton)

    await waitFor(() => {
      expect(screen.getByText('Username is required')).toBeInTheDocument()
      expect(screen.getByText('Password is required')).toBeInTheDocument()
    })
  })

  test('updates input values when typing', () => {
    renderLogin()
    
    const usernameInput = screen.getByLabelText(/username/i) as HTMLInputElement
    const passwordInput = screen.getByLabelText(/password/i) as HTMLInputElement

    fireEvent.change(usernameInput, { target: { value: 'testuser' } })
    fireEvent.change(passwordInput, { target: { value: 'password123' } })

    expect(usernameInput.value).toBe('testuser')
    expect(passwordInput.value).toBe('password123')
  })

  test('shows link to register page', () => {
    renderLogin()
    
    const registerLink = screen.getByText(/don't have an account/i).closest('p')
    expect(registerLink).toBeInTheDocument()
    expect(screen.getByText('Register here')).toBeInTheDocument()
  })
}) 
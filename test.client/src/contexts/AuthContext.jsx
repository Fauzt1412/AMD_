import { createContext, useContext, useState, useEffect } from 'react'

const AuthContext = createContext()

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

// Simple hash function for demo purposes (in real app, use proper backend hashing)
const simpleHash = (str) => {
  let hash = 0
  for (let i = 0; i < str.length; i++) {
    const char = str.charCodeAt(i)
    hash = ((hash << 5) - hash) + char
    hash = hash & hash // Convert to 32bit integer
  }
  return hash.toString()
}

export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  // Load user from localStorage on mount
  useEffect(() => {
    const savedUser = localStorage.getItem('currentUser')
    if (savedUser) {
      setCurrentUser(JSON.parse(savedUser))
    }
    setIsLoading(false)
  }, [])

  // Save user to localStorage when currentUser changes
  useEffect(() => {
    if (currentUser) {
      localStorage.setItem('currentUser', JSON.stringify(currentUser))
    } else {
      localStorage.removeItem('currentUser')
    }
  }, [currentUser])

  const signup = (username, password) => {
    // Get existing users
    const users = JSON.parse(localStorage.getItem('users') || '{}')
    
    // Check if username already exists
    if (users[username]) {
      throw new Error('Username already exists')
    }

    // Validate input
    if (!username || !password) {
      throw new Error('Username and password are required')
    }

    if (username.length < 3) {
      throw new Error('Username must be at least 3 characters long')
    }

    if (password.length < 6) {
      throw new Error('Password must be at least 6 characters long')
    }

    // Create new user
    const newUser = {
      username,
      passwordHash: simpleHash(password),
      createdAt: new Date().toISOString()
    }

    // Save user
    users[username] = newUser
    localStorage.setItem('users', JSON.stringify(users))

    // Initialize empty URL list for user
    const userUrls = JSON.parse(localStorage.getItem('userUrls') || '{}')
    userUrls[username] = []
    localStorage.setItem('userUrls', JSON.stringify(userUrls))

    // Set as current user
    setCurrentUser({ username, createdAt: newUser.createdAt })
    
    return newUser
  }

  const login = (username, password) => {
    // Get existing users
    const users = JSON.parse(localStorage.getItem('users') || '{}')
    
    // Check if user exists
    if (!users[username]) {
      throw new Error('User not found')
    }

    // Check password
    if (users[username].passwordHash !== simpleHash(password)) {
      throw new Error('Invalid password')
    }

    // Set as current user
    setCurrentUser({ 
      username, 
      createdAt: users[username].createdAt 
    })
    
    return users[username]
  }

  const logout = () => {
    setCurrentUser(null)
  }

  const value = {
    currentUser,
    signup,
    login,
    logout,
    isLoading
  }

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}
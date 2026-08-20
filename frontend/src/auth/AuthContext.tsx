import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { authApi } from "../api/endpoints";
import { getStoredToken, setStoredToken } from "../api/client";

interface AuthState {
  isAuthenticated: boolean;
  isLoading: boolean;
  email: string | null;
  userId: string | null;
  isAdmin: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [email, setEmail] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [isAdmin, setIsAdmin] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = getStoredToken();
    if (!token) {
      setIsLoading(false);
      return;
    }
    authApi
      .me()
      .then((info) => {
        setEmail(info.email);
        setUserId(info.id);
        setIsAdmin(info.isAdmin);
      })
      .catch(() => setStoredToken(null))
      .finally(() => setIsLoading(false));
  }, []);

  async function login(loginEmail: string, password: string) {
    const result = await authApi.login(loginEmail, password);
    setStoredToken(result.accessToken);
    const info = await authApi.me();
    setEmail(info.email);
    setUserId(info.id);
    setIsAdmin(info.isAdmin);
  }

  function logout() {
    setStoredToken(null);
    setEmail(null);
    setUserId(null);
    setIsAdmin(false);
    authApi.logout().catch(() => {
      // Token zaten silindiği için backend çağrısı başarısız olsa da sorun değil.
    });
  }

  return (
    <AuthContext.Provider
      value={{ isAuthenticated: email !== null, isLoading, email, userId, isAdmin, login, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth, AuthProvider içinde kullanılmalı");
  return ctx;
}

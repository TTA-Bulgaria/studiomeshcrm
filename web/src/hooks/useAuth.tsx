'use client';

import { createContext, useContext, useEffect, useState } from 'react';
import { usePathname } from 'next/navigation';
import { useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { LoadingOverlay } from '@/components/ui/FailsafeProvider';

const PUBLIC_PATHS = ['/login', '/signup', '/register', '/forgot-password', '/verify-email'];
const PUBLIC_PREFIXES = ['/reset-password', '/portal'];

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
  tenantId?: string;
  tenantSlug?: string;
  isOnboardingCompleted?: boolean;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string, redirectTo?: string) => Promise<void>;
  register: (email: string, password: string, fullName: string, agencyName: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const pathname = usePathname();
  const queryClient = useQueryClient();

  const isPublicPath =
    PUBLIC_PATHS.includes(pathname) ||
    PUBLIC_PREFIXES.some((prefix) => pathname.startsWith(prefix));

  useEffect(() => {
    async function restoreSession() {
      try {
        const data = await api.get<User>('/api/auth/me');
        setUser(data);
      } catch {
        localStorage.removeItem('access_token');
      } finally {
        setLoading(false);
      }
    }
    restoreSession();
  }, []);

  const login = async (email: string, password: string, redirectTo?: string) => {
    const data = await api.post<any>('/api/auth/login', { email, password });

    if (data.accessToken) {
      localStorage.setItem('access_token', data.accessToken);
    }

    setUser(data);

    if (data.isOnboardingCompleted === false) {
      window.location.href = '/onboarding';
      return;
    }

    if (data.tenantSlug) {
      // Validate before using slug to build a URL — rejects any server-supplied value
      // that could form a malicious subdomain (e.g. injected via compromised API response).
      if (!/^[a-z0-9-]+$/.test(data.tenantSlug)) {
        throw new Error('Invalid tenant slug received from server');
      }
      const { hostname, protocol, port } = window.location;
      if (hostname !== 'localhost') {
        const parts = hostname.split('.');
        const isBaseDomain = parts.length === 2 || (parts.length === 3 && parts[0] === 'app');
        if (isBaseDomain) {
          const baseDomain = parts.length === 3 ? parts.slice(1).join('.') : hostname;
          const portSuffix = port ? `:${port}` : '';
          window.location.href = `${protocol}//${data.tenantSlug}.${baseDomain}${portSuffix}/dashboard`;
          return;
        }
      }
    }

    window.location.href = redirectTo ?? '/dashboard';
  };

  const register = async (email: string, password: string, fullName: string, agencyName: string) => {
    await api.post<any>('/api/auth/register', { email, password, fullName, agencyName });
    // Registration succeeds but email must be verified before login.
    // The caller (register page) handles the success state.
  };

  const logout = async () => {
    try {
      await api.post('/api/auth/logout', {});
    } catch {
      // Still clear local state
    } finally {
      localStorage.removeItem('access_token');
      setUser(null);
      queryClient.clear();
      window.location.href = '/login';
    }
  };

  return (
    <AuthContext.Provider value={{ user, loading, isAuthenticated: !!user, login, register, logout }}>
      {loading && !isPublicPath ? <LoadingOverlay /> : children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

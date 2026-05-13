'use client';

import { useState, useEffect, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { api, isApiError } from '@/lib/api';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/LayoutPrimitives';
import { Eye, EyeOff, Mail } from 'lucide-react';
import Link from 'next/link';
import { useTranslation } from 'react-i18next';

/**
 * Validate a redirect destination from the URL query string.
 * Only allows internal relative paths that are not auth pages.
 * Returns undefined for anything that could cause an open redirect or loop.
 */
function getSafeRedirect(raw: string | null): string | undefined {
  if (!raw) return undefined;
  // Reject absolute URLs and protocol-relative paths (//evil.com).
  if (!raw.startsWith('/') || raw.startsWith('//')) return undefined;
  // Reject auth pages — redirecting back to them would create a circular bounce.
  if (['/login', '/register', '/signup'].some((p) => raw.startsWith(p))) return undefined;
  return raw;
}

function LoginPageContent() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [emailNotVerified, setEmailNotVerified] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);
  const [resendSent, setResendSent] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  // Tracks whether a login() call is in flight. Used to prevent the
  // already-authenticated redirect effect from firing after login() sets user —
  // login() owns its own navigation; this component should not race with it.
  const [isLoggingIn, setIsLoggingIn] = useState(false);
  const { login, user, loading } = useAuth();
  const { t } = useTranslation('auth');
  const router = useRouter();
  const searchParams = useSearchParams();
  const redirectTo = getSafeRedirect(searchParams.get('redirect'));

  // Redirect already-authenticated users away from the login page.
  // This handles the case where restoreSession() finds an existing session on
  // mount (e.g. localStorage token with no cookie). It must NOT fire when
  // login() sets user, because login() handles its own navigation — including
  // the validated redirect destination. isLoggingIn guards that separation.
  useEffect(() => {
    if (!loading && user && !isLoggingIn) {
      router.replace('/dashboard');
    }
  }, [user, loading, router, isLoggingIn]);

  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError('');
    setEmailNotVerified(false);
    setResendSent(false);
    setIsSubmitting(true);
    setIsLoggingIn(true);
    try {
      await login(email, password, redirectTo);
    } catch (err) {
      setIsLoggingIn(false);
      if (isApiError(err) && err.status === 401 && err.message.toLowerCase().includes('verify')) {
        setEmailNotVerified(true);
      } else if (err instanceof Error && err.message.includes('429')) {
        setError(t('login.errors.rateLimit'));
      } else {
        setError(t('login.errors.invalid'));
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleResendVerification = async () => {
    setResendLoading(true);
    try {
      await api.post('/api/auth/resend-verification', { email });
      setResendSent(true);
    } catch {
      // silently ignore — user sees no change, can try again
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <div className="flex min-h-[calc(100vh-64px)] items-center justify-center p-4 bg-background">
      <Card className="w-full max-w-sm">
        <CardHeader className="text-center">
          <div className="mx-auto mb-4 h-12 w-12 rounded-xl bg-primary flex items-center justify-center">
            <span className="text-white font-bold text-xl">A</span>
          </div>
          <CardTitle>{t('login.title')}</CardTitle>
          <p className="text-sm text-muted-foreground mt-2">{t('login.subtitle')}</p>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleLogin} className="space-y-4">
            <Input
              label={t('login.email')}
              type="email"
              placeholder="name@company.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="email"
              required
            />

            {/* Password field with show/hide toggle */}
            <div className="space-y-1.5">
              <label htmlFor="password-input" className="text-sm font-medium leading-none">
                {t('login.password')} <span className="text-destructive" aria-hidden="true">*</span>
              </label>
              <div className="relative">
                <input
                  id="password-input"
                  type={showPassword ? 'text' : 'password'}
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  autoComplete="current-password"
                  required
                  className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 pr-10 text-sm
                    placeholder:text-muted-foreground
                    focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2
                    disabled:cursor-not-allowed disabled:opacity-50"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground
                    focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring rounded"
                  aria-label={showPassword ? t('login.hidePassword') : t('login.showPassword')}
                >
                  {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                </button>
              </div>
            </div>

            {/* Forgot password link */}
            <div className="flex justify-end -mt-1">
              <Link
                href="/forgot-password"
                className="text-xs text-muted-foreground hover:text-primary transition-colors"
              >
                {t('login.forgotPassword')}
              </Link>
            </div>

            {emailNotVerified && (
              <div role="alert" className="p-4 rounded-lg bg-amber-50 border border-amber-200 space-y-2">
                <div className="flex items-start gap-2">
                  <Mail className="h-4 w-4 text-amber-600 mt-0.5 shrink-0" />
                  <p className="text-sm text-amber-800 font-medium">Please verify your email address</p>
                </div>
                <p className="text-xs text-amber-700 pl-6">Check your inbox for the verification link we sent when you registered.</p>
                {resendSent ? (
                  <p className="text-xs text-emerald-700 pl-6 font-medium">Verification email resent — check your inbox.</p>
                ) : (
                  <button
                    type="button"
                    onClick={handleResendVerification}
                    disabled={resendLoading}
                    className="text-xs text-amber-800 underline pl-6 hover:text-amber-900 disabled:opacity-50"
                  >
                    {resendLoading ? 'Sending…' : 'Resend verification email'}
                  </button>
                )}
              </div>
            )}

            {error && (
              <div
                role="alert"
                className="p-3 text-sm rounded-md bg-destructive/10 text-destructive border border-destructive/20"
              >
                {error}
              </div>
            )}

            <Button
              type="submit"
              className="w-full"
              isLoading={isSubmitting}
              aria-busy={isSubmitting}
              aria-disabled={isSubmitting}
            >
              {isSubmitting ? t('login.submitting') : t('login.submit')}
            </Button>
          </form>

          <div className="mt-6 text-center text-sm text-muted-foreground">
            {t('login.noAccount')}{' '}
            <Link href="/register" className="font-semibold text-primary hover:underline">
              {t('login.createLink')}
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default function LoginPage() {
  return (
    <Suspense>
      <LoginPageContent />
    </Suspense>
  );
}

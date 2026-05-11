'use client';

import { useEffect, useState, useCallback, Suspense } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import { Button } from '@/components/ui/Button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/LayoutPrimitives';
import { CheckCircle2, AlertCircle } from 'lucide-react';

interface AdAccount {
  id: string;
  name: string;
  isActive: boolean;
}

interface AccountsResponse {
  projectId: string;
  accounts: AdAccount[];
}

function ConnectPageContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const session = searchParams.get('session');
  const errorParam = searchParams.get('error');

  const [data, setData] = useState<AccountsResponse | null>(null);
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(true);
  const [linking, setLinking] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchAccounts = useCallback(async () => {
    if (!session) {
      setError('No session found. Please reconnect from your project.');
      setLoading(false);
      return;
    }
    try {
      const result = await api.get<AccountsResponse>(`/api/facebook/oauth/accounts?session=${session}`);
      setData(result);
      // Pre-select active accounts
      setSelected(new Set(result.accounts.filter(a => a.isActive).map(a => a.id)));
    } catch {
      setError('Session expired or invalid. Please reconnect from your project.');
    } finally {
      setLoading(false);
    }
  }, [session]);

  useEffect(() => {
    if (errorParam) {
      const messages: Record<string, string> = {
        facebook_denied: 'Facebook access was denied.',
        invalid_request: 'Invalid OAuth request.',
        session_expired: 'Session expired. Please try again.',
        token_exchange_failed: 'Failed to connect to Facebook. Please try again.',
      };
      setError(messages[errorParam] ?? 'An error occurred connecting to Facebook.');
      setLoading(false);
      return;
    }
    fetchAccounts();
  }, [errorParam, fetchAccounts]);

  const toggle = (id: string) => {
    setSelected(prev => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  const handleLink = async () => {
    if (!session || selected.size === 0) return;
    setLinking(true);
    try {
      await api.post('/api/facebook/oauth/link', {
        session,
        selectedAccountIds: Array.from(selected),
      });
      router.push(`/projects/${data?.projectId}?tab=ads`);
    } catch {
      setError('Failed to link accounts. Please try again.');
      setLinking(false);
    }
  };

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-brand-600 border-t-transparent rounded-full animate-spin mx-auto mb-4" />
          <p className="text-slate-500 text-sm">Loading your Facebook ad accounts...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center p-4">
        <Card className="w-full max-w-md">
          <CardContent className="py-8 text-center">
            <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-4" />
            <h2 className="text-lg font-semibold text-slate-900 mb-2">Connection failed</h2>
            <p className="text-slate-500 text-sm mb-6">{error}</p>
            <Button onClick={() => router.back()} variant="outline">Go back</Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (!data || data.accounts.length === 0) {
    return (
      <div className="flex min-h-screen items-center justify-center p-4">
        <Card className="w-full max-w-md">
          <CardContent className="py-8 text-center">
            <p className="text-slate-500 text-sm mb-6">No ad accounts found on this Facebook account.</p>
            <Button onClick={() => router.back()} variant="outline">Go back</Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4 bg-slate-50">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <div className="flex items-center gap-3 mb-1">
            <div className="w-9 h-9 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold text-sm">f</div>
            <CardTitle>Select Facebook Ad Accounts</CardTitle>
          </div>
          <p className="text-slate-500 text-sm">
            Choose which ad accounts to link to this project. You can change this later.
          </p>
        </CardHeader>
        <CardContent>
          <div className="space-y-2 mb-6">
            {data.accounts.map(account => (
              <button
                key={account.id}
                type="button"
                onClick={() => toggle(account.id)}
                className={`w-full flex items-center justify-between p-4 rounded-xl border text-left transition-all duration-150 ${
                  selected.has(account.id)
                    ? 'border-brand-300 bg-brand-50'
                    : 'border-slate-200 bg-white hover:border-slate-300'
                }`}
              >
                <div>
                  <p className="text-sm font-semibold text-slate-900">{account.name}</p>
                  <p className="text-xs text-slate-400 mt-0.5">{account.id}</p>
                </div>
                <div className={`w-5 h-5 rounded-full border-2 flex items-center justify-center transition-colors ${
                  selected.has(account.id) ? 'border-brand-600 bg-brand-600' : 'border-slate-300'
                }`}>
                  {selected.has(account.id) && <CheckCircle2 className="w-3.5 h-3.5 text-white" />}
                </div>
              </button>
            ))}
          </div>

          <div className="flex gap-3">
            <Button
              variant="outline"
              className="flex-1"
              onClick={() => router.push(`/projects/${data.projectId}?tab=ads`)}
            >
              Cancel
            </Button>
            <Button
              className="flex-1"
              onClick={handleLink}
              isLoading={linking}
              disabled={selected.size === 0}
            >
              Link {selected.size} account{selected.size !== 1 ? 's' : ''}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default function ConnectPage() {
  return (
    <Suspense>
      <ConnectPageContent />
    </Suspense>
  );
}

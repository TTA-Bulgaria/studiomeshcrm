'use client';

import { useRouter } from 'next/navigation';
import { Container, Section } from '@/components/ui/LayoutPrimitives';
import { Button } from '@/components/ui/Button';
import { CheckCircle2, ArrowRight, BarChart2, Zap, Link2 } from 'lucide-react';

export default function IntegrationsPage() {
  const router = useRouter();

  return (
    <Container>
      <Section>
        <h1 className="text-2xl font-bold tracking-tight">Integrations</h1>
        <p className="text-slate-500 text-sm mt-1">
          Connect your ad platforms to track spend, CPL, and ROAS per project.
        </p>
      </Section>

      <Section>
        {/* Meta Ads — available */}
        <div className="rounded-2xl border border-slate-200 bg-white overflow-hidden mb-4">
          <div className="p-6 flex flex-col sm:flex-row gap-5">
            <div className="w-12 h-12 rounded-xl bg-blue-600 flex items-center justify-center text-white font-bold text-xl shrink-0">
              f
            </div>
            <div className="flex-1 min-w-0">
              <div className="flex flex-wrap items-center gap-2 mb-1.5">
                <h2 className="text-base font-semibold text-slate-900">Meta Ads (Facebook / Instagram)</h2>
                <span className="inline-flex items-center gap-1 text-xs font-medium px-2 py-0.5 bg-emerald-50 text-emerald-700 border border-emerald-200 rounded-full">
                  <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 inline-block" />
                  Available
                </span>
              </div>
              <p className="text-sm text-slate-500 mb-4">
                Connect your Facebook ad accounts to pull real spend, impressions, clicks, and
                conversions into each project. ROI and ROAS are calculated automatically against
                your signed contracts.
              </p>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-1.5">
                {[
                  'Per-project ad account linking',
                  'Daily automatic sync',
                  'Real-time CPL and ROAS analytics',
                  'Token expiry detection with reconnect prompt',
                ].map(f => (
                  <div key={f} className="flex items-center gap-2 text-xs text-slate-600">
                    <CheckCircle2 className="w-3.5 h-3.5 text-emerald-500 shrink-0" />
                    {f}
                  </div>
                ))}
              </div>
            </div>
          </div>
          <div className="border-t border-slate-100 bg-slate-50 px-6 py-3 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
            <p className="text-xs text-slate-500 flex items-center gap-1.5">
              <Link2 className="w-3.5 h-3.5 shrink-0" />
              Connect from inside a project — open any project and go to the <strong className="font-semibold text-slate-700">Ads</strong> tab.
            </p>
            <Button
              size="sm"
              className="flex items-center gap-1.5 shrink-0"
              onClick={() => router.push('/projects')}
            >
              Go to Projects <ArrowRight className="w-3.5 h-3.5" />
            </Button>
          </div>
        </div>

        {/* Coming soon platforms */}
        <div className="grid gap-4 sm:grid-cols-2">
          {[
            {
              icon: <BarChart2 className="w-5 h-5 text-red-500" />,
              iconBg: 'bg-red-50',
              name: 'Google Ads',
              description: 'Track spend and conversions from Google Search and Display campaigns.',
            },
            {
              icon: <Zap className="w-5 h-5 text-slate-700" />,
              iconBg: 'bg-slate-100',
              name: 'TikTok Ads',
              description: 'Automated performance reporting for TikTok campaigns.',
            },
          ].map(p => (
            <div key={p.name} className="rounded-2xl border border-slate-200 bg-white p-5 flex items-start gap-4 opacity-70">
              <div className={`w-10 h-10 rounded-xl ${p.iconBg} flex items-center justify-center shrink-0`}>
                {p.icon}
              </div>
              <div>
                <div className="flex items-center gap-2 mb-1">
                  <h3 className="text-sm font-semibold text-slate-800">{p.name}</h3>
                  <span className="text-xs px-2 py-0.5 bg-slate-100 text-slate-400 rounded-full border border-slate-200">
                    Coming soon
                  </span>
                </div>
                <p className="text-xs text-slate-400">{p.description}</p>
              </div>
            </div>
          ))}
        </div>
      </Section>
    </Container>
  );
}

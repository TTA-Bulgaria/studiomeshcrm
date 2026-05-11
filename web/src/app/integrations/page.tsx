'use client';

import { useRouter } from 'next/navigation';
import { Container, Section, Card, CardContent } from '@/components/ui/LayoutPrimitives';
import { Button } from '@/components/ui/Button';
import { CheckCircle2, ArrowRight, BarChart2, Zap } from 'lucide-react';

export default function IntegrationsPage() {
  const router = useRouter();

  return (
    <Container>
      <Section className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Integrations</h1>
          <p className="text-slate-500 text-sm mt-1">
            Connect your ad platforms to track spend, CPL, and ROAS per project.
          </p>
        </div>
      </Section>

      {/* Meta Ads */}
      <Section>
        <Card>
          <CardContent className="p-6">
            <div className="flex flex-col md:flex-row md:items-center gap-6">
              <div className="w-12 h-12 rounded-xl bg-blue-600 flex items-center justify-center text-white font-bold text-xl shrink-0">
                f
              </div>
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <h2 className="text-lg font-semibold">Meta Ads (Facebook / Instagram)</h2>
                  <span className="text-xs font-semibold px-2 py-0.5 bg-green-100 text-green-700 rounded-full">
                    Available
                  </span>
                </div>
                <p className="text-sm text-slate-500">
                  Connect your Facebook ad accounts to pull real spend, impressions, clicks, and
                  conversions into each project. ROI and ROAS are calculated automatically against
                  your signed contracts.
                </p>
                <ul className="mt-3 space-y-1">
                  {[
                    'Per-project ad account linking',
                    'Daily automatic sync',
                    'Real-time CPL and ROAS analytics',
                    'Token expiry detection with reconnect prompt',
                  ].map(f => (
                    <li key={f} className="flex items-center gap-2 text-xs text-slate-600">
                      <CheckCircle2 className="w-3.5 h-3.5 text-green-500 shrink-0" />
                      {f}
                    </li>
                  ))}
                </ul>
              </div>
              <div className="shrink-0">
                <p className="text-xs text-slate-400 mb-3 max-w-[200px] text-center">
                  Connect from inside a project — open any project and go to the <strong>Ads</strong> tab.
                </p>
                <Button
                  className="w-full flex items-center gap-2"
                  onClick={() => router.push('/projects')}
                >
                  Go to Projects <ArrowRight className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      </Section>

      {/* Coming soon */}
      <Section>
        <div className="grid gap-4 md:grid-cols-2">
          {[
            {
              icon: <BarChart2 className="w-5 h-5 text-red-500" />,
              name: 'Google Ads',
              description: 'Track spend and conversions from Google Search and Display campaigns.',
              color: 'bg-red-50 border-red-100',
            },
            {
              icon: <Zap className="w-5 h-5 text-slate-800" />,
              name: 'TikTok Ads',
              description: 'Automated performance reporting for TikTok campaigns.',
              color: 'bg-slate-50 border-slate-100',
            },
          ].map(p => (
            <Card key={p.name} className={`border ${p.color}`}>
              <CardContent className="p-5 flex items-start gap-4">
                <div className="w-9 h-9 rounded-lg bg-white border flex items-center justify-center shrink-0">
                  {p.icon}
                </div>
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="text-sm font-semibold">{p.name}</h3>
                    <span className="text-xs px-2 py-0.5 bg-white border rounded-full text-slate-400">
                      Coming soon
                    </span>
                  </div>
                  <p className="text-xs text-slate-500">{p.description}</p>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </Section>
    </Container>
  );
}

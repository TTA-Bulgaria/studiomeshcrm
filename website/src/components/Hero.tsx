import { ArrowRight, Star } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

export default function Hero() {
  return (
    <section className="relative overflow-hidden bg-white pt-32 pb-20">
      {/* Subtle dot grid */}
      <div className="absolute inset-0 bg-[radial-gradient(#e2e8f0_1px,transparent_1px)] bg-[size:32px_32px] opacity-60 pointer-events-none" />
      {/* Soft radial glow */}
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[900px] h-[500px] bg-brand-100/60 rounded-full blur-[120px] pointer-events-none" />

      <div className="relative max-w-7xl mx-auto px-6 lg:px-8 text-center">
        {/* Badge */}
        <div className="inline-flex items-center gap-2 bg-brand-50 border border-brand-200 rounded-full px-4 py-1.5 mb-8">
          <span className="w-2 h-2 bg-brand-500 rounded-full animate-pulse" />
          <span className="text-brand-700 text-sm font-medium">Built for digital agencies</span>
        </div>

        {/* Headline */}
        <h1 className="text-5xl md:text-7xl font-extrabold text-slate-900 leading-[1.05] tracking-tight mb-6 text-balance">
          The Agency OS<br />
          <span className="gradient-text">Built for Growth</span>
        </h1>

        {/* Subheadline */}
        <p className="text-lg md:text-xl text-slate-500 max-w-2xl mx-auto mb-10 leading-relaxed">
          Studio Mesh unifies your leads, projects, contracts, invoices, and ad performance
          into one clean workspace — so your team spends less time switching tools and more
          time delivering results.
        </p>

        {/* CTAs */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4 mb-12">
          <a
            href={`${APP_URL}/register`}
            className="group flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-semibold px-7 py-3.5 rounded-xl transition-all duration-200 shadow-lg shadow-brand-600/25 hover:shadow-brand-700/30"
          >
            Start your free trial
            <ArrowRight className="w-4 h-4 group-hover:translate-x-0.5 transition-transform" />
          </a>
          <a
            href={`${APP_URL}/login`}
            className="flex items-center gap-2 text-slate-600 hover:text-slate-900 font-medium px-7 py-3.5 rounded-xl border border-slate-200 hover:border-slate-300 hover:bg-slate-50 transition-all duration-200"
          >
            Sign in to your account
          </a>
        </div>

        {/* Social proof */}
        <div className="flex flex-col items-center gap-2 mb-16">
          <div className="flex items-center gap-1">
            {[...Array(5)].map((_, i) => (
              <Star key={i} className="w-4 h-4 text-amber-400 fill-amber-400" />
            ))}
            <span className="text-slate-500 text-sm ml-2 font-medium">4.9 / 5 from early adopters</span>
          </div>
          <p className="text-slate-400 text-sm">Trusted by growing agencies across Europe &amp; Africa</p>
        </div>

        {/* App mockup */}
        <div className="relative max-w-5xl mx-auto">
          {/* Fade bottom */}
          <div className="absolute bottom-0 left-0 right-0 h-32 bg-gradient-to-t from-white to-transparent z-10 pointer-events-none rounded-b-2xl" />
          <div className="bg-white border border-slate-200 rounded-2xl overflow-hidden shadow-2xl shadow-slate-200/80">
            {/* Browser bar */}
            <div className="flex items-center gap-2 px-4 py-3 bg-slate-50 border-b border-slate-200">
              <div className="flex gap-1.5">
                <div className="w-3 h-3 rounded-full bg-red-400/80" />
                <div className="w-3 h-3 rounded-full bg-amber-400/80" />
                <div className="w-3 h-3 rounded-full bg-emerald-400/80" />
              </div>
              <div className="flex-1 mx-4 bg-white border border-slate-200 rounded-md px-3 py-1 text-xs text-slate-400">
                app.studiomeshcrm.com/dashboard
              </div>
            </div>
            {/* Dashboard content */}
            <div className="p-6 grid grid-cols-4 gap-4 bg-slate-50/50">
              {[
                { label: 'Active Projects', value: '12', accent: 'bg-brand-100 text-brand-700', dot: 'bg-brand-500' },
                { label: 'Open Leads', value: '34', accent: 'bg-emerald-100 text-emerald-700', dot: 'bg-emerald-500' },
                { label: 'Monthly Revenue', value: '$48.2K', accent: 'bg-violet-100 text-violet-700', dot: 'bg-violet-500' },
                { label: 'Pending Invoices', value: '7', accent: 'bg-orange-100 text-orange-700', dot: 'bg-orange-500' },
              ].map(({ label, value, accent, dot }) => (
                <div key={label} className="bg-white rounded-xl p-4 border border-slate-100 shadow-sm">
                  <div className={`inline-flex items-center gap-1.5 text-xs font-medium px-2 py-1 rounded-md mb-3 ${accent}`}>
                    <div className={`w-1.5 h-1.5 rounded-full ${dot}`} />
                    {label}
                  </div>
                  <div className="text-2xl font-bold text-slate-900">{value}</div>
                </div>
              ))}
              {/* Revenue chart */}
              <div className="col-span-3 bg-white rounded-xl p-4 border border-slate-100 shadow-sm h-36">
                <div className="text-xs text-slate-500 mb-3 font-medium">Revenue Trend — Last 12 months</div>
                <div className="flex items-end gap-1 h-20">
                  {[40, 55, 45, 70, 55, 80, 65, 90, 60, 85, 75, 100].map((h, i) => (
                    <div
                      key={i}
                      className="flex-1 rounded-sm transition-all"
                      style={{ height: `${h}%`, background: i === 11 ? '#7c3aed' : '#ede9fe' }}
                    />
                  ))}
                </div>
              </div>
              {/* Pipeline */}
              <div className="bg-white rounded-xl p-4 border border-slate-100 shadow-sm h-36">
                <div className="text-xs text-slate-500 mb-3 font-medium">Pipeline</div>
                <div className="space-y-2.5">
                  {[
                    { label: 'Qualified', w: '70%', color: 'bg-brand-500' },
                    { label: 'Proposal', w: '45%', color: 'bg-violet-400' },
                    { label: 'Closed', w: '30%', color: 'bg-emerald-500' },
                  ].map(({ label, w, color }) => (
                    <div key={label}>
                      <div className="flex justify-between text-xs text-slate-400 mb-1">
                        <span>{label}</span>
                        <span>{w}</span>
                      </div>
                      <div className="h-1.5 bg-slate-100 rounded-full overflow-hidden">
                        <div className={`h-full ${color} rounded-full`} style={{ width: w }} />
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

import { ArrowRight, Play } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

export default function Hero() {
  return (
    <section className="relative min-h-screen flex items-center overflow-hidden bg-gray-950">
      {/* Background grid + glow */}
      <div className="absolute inset-0 bg-[linear-gradient(to_right,#ffffff08_1px,transparent_1px),linear-gradient(to_bottom,#ffffff08_1px,transparent_1px)] bg-[size:48px_48px]" />
      <div className="absolute top-1/4 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] bg-brand-600/20 rounded-full blur-[120px] pointer-events-none" />

      <div className="relative max-w-7xl mx-auto px-6 lg:px-8 pt-24 pb-20 text-center">
        {/* Badge */}
        <div className="inline-flex items-center gap-2 bg-brand-600/10 border border-brand-600/20 rounded-full px-4 py-1.5 mb-8">
          <span className="w-2 h-2 bg-brand-400 rounded-full animate-pulse" />
          <span className="text-brand-300 text-sm font-medium">Built for digital agencies</span>
        </div>

        {/* Headline */}
        <h1 className="text-5xl md:text-7xl font-extrabold text-white leading-[1.05] tracking-tight mb-6">
          The Agency OS<br />
          <span className="gradient-text">Built for Growth</span>
        </h1>

        {/* Subheadline */}
        <p className="text-lg md:text-xl text-gray-400 max-w-2xl mx-auto mb-10 leading-relaxed">
          Studio Mesh unifies your leads, projects, contracts, invoices, and ad performance
          into one clean workspace — so your team spends less time switching tools and more
          time delivering results.
        </p>

        {/* CTAs */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4 mb-16">
          <a
            href={`${APP_URL}/register`}
            className="group flex items-center gap-2 bg-brand-600 hover:bg-brand-500 text-white font-semibold px-7 py-3.5 rounded-xl transition-all duration-200 shadow-lg shadow-brand-600/30 hover:shadow-brand-500/40"
          >
            Start your free trial
            <ArrowRight className="w-4 h-4 group-hover:translate-x-0.5 transition-transform" />
          </a>
          <a
            href={`${APP_URL}/login`}
            className="flex items-center gap-2 text-gray-300 hover:text-white font-medium px-7 py-3.5 rounded-xl border border-gray-700 hover:border-gray-500 transition-all duration-200"
          >
            <Play className="w-4 h-4" />
            Sign in
          </a>
        </div>

        {/* Social proof */}
        <p className="text-gray-500 text-sm mb-3">Trusted by growing agencies across Europe & Africa</p>
        <div className="flex items-center justify-center gap-1">
          {[...Array(5)].map((_, i) => (
            <svg key={i} className="w-4 h-4 text-yellow-400 fill-current" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
          ))}
          <span className="text-gray-400 text-sm ml-2">4.9 / 5 from early adopters</span>
        </div>

        {/* App screenshot mockup */}
        <div className="mt-16 relative max-w-5xl mx-auto">
          <div className="absolute inset-0 bg-gradient-to-t from-gray-950 via-transparent to-transparent z-10 pointer-events-none rounded-2xl" />
          <div className="bg-gray-900 border border-gray-800 rounded-2xl overflow-hidden shadow-2xl shadow-black/50">
            {/* Fake browser bar */}
            <div className="flex items-center gap-2 px-4 py-3 bg-gray-800/50 border-b border-gray-700/50">
              <div className="flex gap-1.5">
                <div className="w-3 h-3 rounded-full bg-red-500/70" />
                <div className="w-3 h-3 rounded-full bg-yellow-500/70" />
                <div className="w-3 h-3 rounded-full bg-green-500/70" />
              </div>
              <div className="flex-1 mx-4 bg-gray-700/50 rounded-md px-3 py-1 text-xs text-gray-400">
                app.studiomeshcrm.com/dashboard
              </div>
            </div>
            {/* Dashboard mockup */}
            <div className="p-6 grid grid-cols-4 gap-4">
              {[
                { label: 'Active Projects', value: '12', color: 'from-brand-600 to-brand-800' },
                { label: 'Open Leads', value: '34', color: 'from-emerald-600 to-emerald-800' },
                { label: 'Monthly Revenue', value: '$48.2K', color: 'from-violet-600 to-violet-800' },
                { label: 'Pending Invoices', value: '7', color: 'from-orange-600 to-orange-800' },
              ].map(({ label, value, color }) => (
                <div key={label} className="bg-gray-800 rounded-xl p-4">
                  <div className={`w-8 h-8 rounded-lg bg-gradient-to-br ${color} mb-3`} />
                  <div className="text-2xl font-bold text-white">{value}</div>
                  <div className="text-xs text-gray-400 mt-1">{label}</div>
                </div>
              ))}
              <div className="col-span-3 bg-gray-800 rounded-xl p-4 h-32">
                <div className="text-xs text-gray-400 mb-3 font-medium">Revenue Trend</div>
                <div className="flex items-end gap-1 h-16">
                  {[40, 65, 45, 80, 55, 90, 70, 95, 60, 85, 75, 100].map((h, i) => (
                    <div key={i} className="flex-1 bg-brand-600/40 rounded-sm" style={{ height: `${h}%` }} />
                  ))}
                </div>
              </div>
              <div className="bg-gray-800 rounded-xl p-4 h-32">
                <div className="text-xs text-gray-400 mb-3 font-medium">Pipeline</div>
                <div className="space-y-2">
                  {[{ label: 'Qualified', w: '70%' }, { label: 'Proposal', w: '45%' }, { label: 'Closed', w: '30%' }].map(({ label, w }) => (
                    <div key={label}>
                      <div className="text-xs text-gray-500 mb-1">{label}</div>
                      <div className="h-1.5 bg-gray-700 rounded-full overflow-hidden">
                        <div className="h-full bg-brand-500 rounded-full" style={{ width: w }} />
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

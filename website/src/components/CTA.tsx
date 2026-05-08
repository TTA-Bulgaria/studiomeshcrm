import { ArrowRight } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

export default function CTA() {
  return (
    <section className="py-24 bg-slate-900">
      <div className="max-w-4xl mx-auto px-6 lg:px-8 text-center">
        <h2 className="text-4xl md:text-5xl font-extrabold text-white tracking-tight mb-5 text-balance">
          Ready to run your agency<br />
          <span className="gradient-text">without the chaos?</span>
        </h2>
        <p className="text-slate-400 text-lg mb-10 max-w-xl mx-auto">
          Join agencies already using Studio Mesh to close more deals, deliver better projects, and grow faster.
        </p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <a
            href={`${APP_URL}/register`}
            className="group flex items-center gap-2 bg-white hover:bg-slate-50 text-slate-900 font-semibold px-8 py-4 rounded-xl transition-all duration-200 text-base shadow-lg"
          >
            Start your free 14-day trial
            <ArrowRight className="w-4 h-4 group-hover:translate-x-0.5 transition-transform" />
          </a>
          <a
            href="mailto:hello@studiomeshcrm.com"
            className="text-slate-400 hover:text-white font-medium px-8 py-4 rounded-xl border border-slate-700 hover:border-slate-500 transition-all duration-200 text-base"
          >
            Talk to us first
          </a>
        </div>

        <p className="text-slate-600 text-sm mt-8">No credit card required · Cancel anytime · Setup in minutes</p>
      </div>
    </section>
  )
}

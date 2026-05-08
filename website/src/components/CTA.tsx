import { ArrowRight } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

export default function CTA() {
  return (
    <section className="py-24 bg-white">
      <div className="max-w-4xl mx-auto px-6 lg:px-8 text-center">
        <div className="relative rounded-3xl overflow-hidden bg-gray-950 px-8 py-16 md:px-16">
          {/* Background glow */}
          <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[400px] bg-brand-600/20 rounded-full blur-[100px] pointer-events-none" />
          <div className="absolute inset-0 bg-[linear-gradient(to_right,#ffffff05_1px,transparent_1px),linear-gradient(to_bottom,#ffffff05_1px,transparent_1px)] bg-[size:32px_32px]" />

          <div className="relative">
            <h2 className="text-4xl md:text-5xl font-extrabold text-white tracking-tight mb-4">
              Ready to run your agency<br />
              <span className="gradient-text">without the chaos?</span>
            </h2>
            <p className="text-gray-400 text-lg mb-10 max-w-xl mx-auto">
              Join agencies already using Studio Mesh to close more deals, deliver better projects, and grow faster.
            </p>

            <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
              <a
                href={`${APP_URL}/register`}
                className="group flex items-center gap-2 bg-brand-600 hover:bg-brand-500 text-white font-semibold px-8 py-4 rounded-xl transition-all duration-200 shadow-lg shadow-brand-600/30 hover:shadow-brand-500/40 text-base"
              >
                Start your free 14-day trial
                <ArrowRight className="w-4 h-4 group-hover:translate-x-0.5 transition-transform" />
              </a>
              <a
                href="mailto:hello@studiomeshcrm.com"
                className="text-gray-300 hover:text-white font-medium px-8 py-4 rounded-xl border border-gray-700 hover:border-gray-500 transition-all duration-200 text-base"
              >
                Talk to us first
              </a>
            </div>

            <p className="text-gray-500 text-sm mt-6">No credit card required · Cancel anytime · Setup in minutes</p>
          </div>
        </div>
      </div>
    </section>
  )
}

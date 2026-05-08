import { Check } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

const plans = [
  {
    name: 'Starter',
    price: '$49',
    period: '/mo',
    description: 'Perfect for solo operators and small agencies getting started.',
    features: [
      'Up to 3 team members',
      'Lead & client management',
      'Project workspace (Kanban)',
      'Proposals & e-signatures',
      'Basic invoicing',
      'Email support',
    ],
    cta: 'Start free trial',
    highlighted: false,
  },
  {
    name: 'Growth',
    price: '$99',
    period: '/mo',
    description: 'Everything you need to run and scale a growing agency.',
    features: [
      'Up to 10 team members',
      'Everything in Starter',
      'Smart automation',
      'Contracts with e-signature',
      'Ad performance dashboard',
      'Facebook & Google Ads integration',
      'Priority support',
    ],
    cta: 'Start free trial',
    highlighted: true,
    badge: 'Most Popular',
  },
  {
    name: 'Agency',
    price: '$199',
    period: '/mo',
    description: 'For established agencies managing multiple clients at scale.',
    features: [
      'Unlimited team members',
      'Everything in Growth',
      'Custom branding & white-label',
      'Advanced reporting',
      'API access',
      'Dedicated account manager',
      'SLA-backed support',
    ],
    cta: 'Talk to sales',
    highlighted: false,
  },
]

export default function Pricing() {
  return (
    <section id="pricing" className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-slate-50 border border-slate-200 rounded-full px-4 py-1.5 mb-4">
            <span className="text-slate-600 text-sm font-medium">Simple pricing</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-slate-900 tracking-tight mb-4 text-balance">
            Plans that grow<br />
            <span className="gradient-text">with your agency</span>
          </h2>
          <p className="text-slate-500 text-lg max-w-xl mx-auto">
            Start free for 14 days. No credit card required. Cancel anytime.
          </p>
        </div>

        <div className="grid md:grid-cols-3 gap-6 items-stretch">
          {plans.map(({ name, price, period, description, features, cta, highlighted, badge }) => (
            <div
              key={name}
              className={`relative rounded-2xl p-8 flex flex-col transition-all duration-200 ${
                highlighted
                  ? 'bg-brand-600 shadow-xl shadow-brand-600/20 scale-[1.02]'
                  : 'bg-white border border-slate-200 shadow-sm hover:shadow-md hover:-translate-y-0.5'
              }`}
            >
              {badge && (
                <div className="absolute -top-3.5 left-1/2 -translate-x-1/2">
                  <span className="bg-slate-900 text-white text-xs font-bold px-3 py-1 rounded-full">
                    {badge}
                  </span>
                </div>
              )}

              <div className="mb-6">
                <h3 className={`text-base font-bold mb-1 ${highlighted ? 'text-white' : 'text-slate-900'}`}>{name}</h3>
                <p className={`text-sm mb-4 ${highlighted ? 'text-brand-200' : 'text-slate-500'}`}>{description}</p>
                <div className="flex items-baseline gap-1">
                  <span className={`text-4xl font-extrabold ${highlighted ? 'text-white' : 'text-slate-900'}`}>{price}</span>
                  <span className={`text-sm ${highlighted ? 'text-brand-200' : 'text-slate-400'}`}>{period}</span>
                </div>
              </div>

              <ul className="space-y-3 mb-8 flex-1">
                {features.map((f) => (
                  <li key={f} className="flex items-start gap-3">
                    <Check className={`w-4 h-4 mt-0.5 shrink-0 ${highlighted ? 'text-white' : 'text-brand-600'}`} />
                    <span className={`text-sm ${highlighted ? 'text-brand-50' : 'text-slate-600'}`}>{f}</span>
                  </li>
                ))}
              </ul>

              <a
                href={`${APP_URL}/register`}
                className={`block text-center font-semibold py-3 px-6 rounded-xl transition-all duration-200 text-sm ${
                  highlighted
                    ? 'bg-white text-brand-700 hover:bg-brand-50'
                    : 'bg-slate-900 text-white hover:bg-slate-800'
                }`}
              >
                {cta}
              </a>
            </div>
          ))}
        </div>

        <p className="text-center text-slate-400 text-sm mt-10">
          All plans include a 14-day free trial. No credit card required.
        </p>
      </div>
    </section>
  )
}

const steps = [
  {
    number: '01',
    title: 'Capture your leads',
    description: 'Leads arrive from Facebook Lead Ads, web forms, or manual entry. They\'re scored, assigned, and ready to pursue — instantly visible in your pipeline.',
  },
  {
    number: '02',
    title: 'Send proposals & close deals',
    description: 'Build and send branded proposals in minutes. Clients review, sign, and accept — triggering automatic project and contract creation.',
  },
  {
    number: '03',
    title: 'Deliver & get paid',
    description: 'Your team executes inside the project workspace. Invoices generate automatically, payments are tracked, and nothing falls through the cracks.',
  },
]

export default function HowItWorks() {
  return (
    <section className="py-24 bg-gray-950">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-white/5 border border-white/10 rounded-full px-4 py-1.5 mb-4">
            <span className="text-gray-300 text-sm font-medium">Simple by design</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-white tracking-tight mb-4">
            From lead to payment<br />
            <span className="gradient-text">in three steps</span>
          </h2>
          <p className="text-gray-400 text-lg max-w-xl mx-auto">
            Studio Mesh is designed around how agencies actually work — not how software vendors think they work.
          </p>
        </div>

        <div className="grid md:grid-cols-3 gap-8 relative">
          {/* Connector line */}
          <div className="hidden md:block absolute top-10 left-1/3 right-1/3 h-px bg-gradient-to-r from-brand-600/0 via-brand-600/50 to-brand-600/0" />

          {steps.map(({ number, title, description }) => (
            <div key={number} className="relative">
              <div className="flex flex-col items-center text-center md:items-start md:text-left">
                <div className="w-16 h-16 rounded-2xl bg-brand-600/10 border border-brand-600/20 flex items-center justify-center mb-6">
                  <span className="text-2xl font-black text-brand-400">{number}</span>
                </div>
                <h3 className="text-xl font-bold text-white mb-3">{title}</h3>
                <p className="text-gray-400 text-sm leading-relaxed">{description}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

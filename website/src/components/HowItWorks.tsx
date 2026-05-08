const steps = [
  {
    number: '01',
    title: 'Capture your leads',
    description: 'Leads arrive from Facebook Lead Ads, web forms, or manual entry. They\'re scored, assigned, and ready to pursue — instantly visible in your pipeline.',
    preview: (
      <div className="bg-white rounded-xl border border-slate-200 p-4 shadow-sm">
        <div className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-3">Lead Pipeline</div>
        {[
          { name: 'Atlas Creative Co.', stage: 'New', color: 'bg-blue-100 text-blue-700' },
          { name: 'Bloom Media', stage: 'Contacted', color: 'bg-amber-100 text-amber-700' },
          { name: 'Nova Digital', stage: 'Qualified', color: 'bg-emerald-100 text-emerald-700' },
        ].map(({ name, stage, color }) => (
          <div key={name} className="flex items-center justify-between py-2 border-b border-slate-50 last:border-0">
            <div className="flex items-center gap-2">
              <div className="w-6 h-6 rounded-full bg-brand-100 text-brand-700 text-xs font-bold flex items-center justify-center">
                {name[0]}
              </div>
              <span className="text-xs text-slate-700 font-medium">{name}</span>
            </div>
            <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${color}`}>{stage}</span>
          </div>
        ))}
      </div>
    ),
  },
  {
    number: '02',
    title: 'Send proposals & close deals',
    description: 'Build and send branded proposals in minutes. Clients review, sign, and accept — triggering automatic project and contract creation.',
    preview: (
      <div className="bg-white rounded-xl border border-slate-200 p-4 shadow-sm">
        <div className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-3">Proposal #PRO-042</div>
        <div className="text-sm font-bold text-slate-900 mb-1">Social Media Management</div>
        <div className="text-xs text-slate-500 mb-3">Bloom Media · Sent 2 hours ago</div>
        <div className="space-y-1.5 mb-4">
          {[
            { item: 'Monthly content strategy', price: '$800' },
            { item: 'Ad management (Meta + Google)', price: '$600' },
            { item: 'Monthly reporting', price: '$200' },
          ].map(({ item, price }) => (
            <div key={item} className="flex justify-between text-xs">
              <span className="text-slate-600">{item}</span>
              <span className="font-semibold text-slate-900">{price}</span>
            </div>
          ))}
        </div>
        <div className="flex items-center justify-between border-t border-slate-100 pt-2">
          <span className="text-xs font-bold text-slate-900">Total / mo</span>
          <span className="text-sm font-extrabold text-brand-600">$1,600</span>
        </div>
        <div className="mt-3 bg-emerald-50 border border-emerald-200 rounded-lg px-3 py-1.5 text-xs text-emerald-700 font-semibold text-center">
          ✓ Signed — 14 min ago
        </div>
      </div>
    ),
  },
  {
    number: '03',
    title: 'Deliver & get paid',
    description: 'Your team executes inside the project workspace. Invoices generate automatically, payments are tracked, and nothing falls through the cracks.',
    preview: (
      <div className="bg-white rounded-xl border border-slate-200 p-4 shadow-sm">
        <div className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-3">Project Progress</div>
        <div className="text-sm font-bold text-slate-900 mb-3">Bloom Media Onboarding</div>
        <div className="space-y-2 mb-4">
          {[
            { task: 'Brand audit', done: true },
            { task: 'Content calendar', done: true },
            { task: 'Ad creative review', done: false },
            { task: 'Go-live', done: false },
          ].map(({ task, done }) => (
            <div key={task} className="flex items-center gap-2">
              <div className={`w-4 h-4 rounded-full flex items-center justify-center text-xs ${done ? 'bg-emerald-500 text-white' : 'border-2 border-slate-200'}`}>
                {done && '✓'}
              </div>
              <span className={`text-xs ${done ? 'text-slate-400 line-through' : 'text-slate-700'}`}>{task}</span>
            </div>
          ))}
        </div>
        <div className="bg-brand-50 border border-brand-100 rounded-lg px-3 py-2 text-xs text-brand-700">
          <span className="font-semibold">Invoice #INV-089</span> auto-generated · $1,600 · Due in 7 days
        </div>
      </div>
    ),
  },
]

export default function HowItWorks() {
  return (
    <section id="how-it-works" className="py-24 bg-slate-50">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-white border border-slate-200 rounded-full px-4 py-1.5 mb-4">
            <span className="text-slate-600 text-sm font-medium">Simple by design</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-slate-900 tracking-tight mb-4 text-balance">
            From lead to payment<br />
            <span className="gradient-text">in three steps</span>
          </h2>
          <p className="text-slate-500 text-lg max-w-xl mx-auto">
            Studio Mesh is designed around how agencies actually work — not how software vendors think they work.
          </p>
        </div>

        <div className="space-y-20">
          {steps.map(({ number, title, description, preview }, idx) => (
            <div
              key={number}
              className={`flex flex-col md:flex-row items-center gap-12 ${idx % 2 === 1 ? 'md:flex-row-reverse' : ''}`}
            >
              {/* Text */}
              <div className="flex-1">
                <div className="text-8xl font-black text-slate-100 leading-none mb-4 select-none">{number}</div>
                <h3 className="text-2xl font-bold text-slate-900 mb-3">{title}</h3>
                <p className="text-slate-500 leading-relaxed">{description}</p>
              </div>
              {/* Preview */}
              <div className="flex-1 w-full max-w-md">
                {preview}
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

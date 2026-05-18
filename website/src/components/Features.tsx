import { Users, FolderKanban, FileText, Receipt, BarChart3, Zap } from 'lucide-react'

const features = [
  {
    icon: Users,
    title: 'Lead & Client Management',
    description: 'Capture leads from Facebook, web forms, or manually. Track every conversation, score prospects, and convert to clients in one click.',
    accent: 'bg-blue-50 text-blue-600 border-blue-100',
    large: true,
    preview: (
      <div className="mt-4 space-y-2">
        {[
          { name: 'Atlas Creative Co.', value: '$4,200', stage: 'Qualified', color: 'bg-emerald-100 text-emerald-700' },
          { name: 'Bloom Media', value: '$1,600', stage: 'Proposal', color: 'bg-amber-100 text-amber-700' },
          { name: 'Nova Digital', value: '$3,000', stage: 'New', color: 'bg-blue-100 text-blue-700' },
        ].map(({ name, value, stage, color }) => (
          <div key={name} className="flex items-center justify-between bg-white rounded-lg px-3 py-2 border border-slate-100 text-xs">
            <span className="font-medium text-slate-800">{name}</span>
            <div className="flex items-center gap-2">
              <span className="text-slate-500">{value}</span>
              <span className={`px-2 py-0.5 rounded-full font-medium ${color}`}>{stage}</span>
            </div>
          </div>
        ))}
      </div>
    ),
  },
  {
    icon: FolderKanban,
    title: 'Project Workspace',
    description: 'Kanban boards, task assignment, and team collaboration — from kickoff to delivery.',
    accent: 'bg-brand-50 text-brand-600 border-brand-100',
  },
  {
    icon: FileText,
    title: 'Proposals & Contracts',
    description: 'Send branded proposals with e-signature. Get notified the moment a client views or signs.',
    accent: 'bg-emerald-50 text-emerald-600 border-emerald-100',
  },
  {
    icon: Receipt,
    title: 'Invoicing & Billing',
    description: 'Generate invoices from contracts automatically. Track payments and revenue in real time.',
    accent: 'bg-orange-50 text-orange-600 border-orange-100',
  },
  {
    icon: BarChart3,
    title: 'Ad Performance Dashboard',
    description: 'Connect Facebook & Google Ads — see ROAS and CPL alongside your financials.',
    accent: 'bg-pink-50 text-pink-600 border-pink-100',
  },
  {
    icon: Zap,
    title: 'Smart Automation',
    description: 'Proposal accepted? Project, contract, and kickoff tasks are created automatically.',
    accent: 'bg-violet-50 text-violet-600 border-violet-100',
  },
]

export default function Features() {
  const [large, ...rest] = features

  return (
    <section id="features" className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-brand-50 border border-brand-100 rounded-full px-4 py-1.5 mb-4">
            <span className="text-brand-600 text-sm font-medium">Everything in one place</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-slate-900 tracking-tight mb-4 text-balance">
            Your entire agency.<br />
            <span className="gradient-text">One platform.</span>
          </h2>
          <p className="text-slate-500 text-lg max-w-xl mx-auto">
            Stop juggling 6 different tools. Studio Mesh brings together every workflow your agency needs to operate and scale.
          </p>
        </div>

        {/* Bento grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          {/* Large card */}
          <div className={`md:row-span-2 rounded-2xl p-7 border bg-slate-50 border-slate-100 flex flex-col`}>
            <div className={`w-11 h-11 rounded-xl flex items-center justify-center mb-4 border ${large.accent}`}>
              <large.icon className="w-5 h-5" />
            </div>
            <h3 className="font-bold text-slate-900 text-lg mb-2">{large.title}</h3>
            <p className="text-slate-500 text-sm leading-relaxed">{large.description}</p>
            {large.preview}
          </div>

          {/* Remaining 5 cards */}
          {rest.map(({ icon: Icon, title, description, accent }) => (
            <div
              key={title}
              className="rounded-2xl p-7 border border-slate-100 bg-white hover:border-slate-200 hover:shadow-sm transition-all duration-200"
            >
              <div className={`w-11 h-11 rounded-xl flex items-center justify-center mb-4 border ${accent}`}>
                <Icon className="w-5 h-5" />
              </div>
              <h3 className="font-bold text-slate-900 text-base mb-2">{title}</h3>
              <p className="text-slate-500 text-sm leading-relaxed">{description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

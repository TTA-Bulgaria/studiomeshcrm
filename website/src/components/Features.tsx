import { Users, FolderKanban, FileText, Receipt, BarChart3, Zap } from 'lucide-react'

const features = [
  {
    icon: Users,
    title: 'Lead & Client Management',
    description: 'Capture leads from Facebook, web forms, or manually. Track every conversation, score prospects, and convert them to clients in one click.',
    color: 'bg-blue-500/10 text-blue-400',
  },
  {
    icon: FolderKanban,
    title: 'Project Workspace',
    description: 'Kanban boards, task assignment, time tracking, and team collaboration — everything your project needs from kickoff to delivery.',
    color: 'bg-brand-500/10 text-brand-400',
  },
  {
    icon: FileText,
    title: 'Proposals & Contracts',
    description: 'Send branded proposals and legally binding contracts with e-signature. Get notified the moment a client views or signs.',
    color: 'bg-emerald-500/10 text-emerald-400',
  },
  {
    icon: Receipt,
    title: 'Invoicing & Billing',
    description: 'Generate invoices automatically from contracts. Track payment status, send overdue reminders, and monitor revenue in real time.',
    color: 'bg-orange-500/10 text-orange-400',
  },
  {
    icon: BarChart3,
    title: 'Ad Performance Dashboard',
    description: 'Connect Facebook and Google Ads to see spend, ROAS, and CPL alongside your project financials — no more switching between platforms.',
    color: 'bg-pink-500/10 text-pink-400',
  },
  {
    icon: Zap,
    title: 'Smart Automation',
    description: 'When a proposal is accepted, the system automatically creates the project, contract, and kickoff tasks. Zero manual setup.',
    color: 'bg-yellow-500/10 text-yellow-400',
  },
]

export default function Features() {
  return (
    <section id="features" className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        {/* Header */}
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-brand-50 border border-brand-100 rounded-full px-4 py-1.5 mb-4">
            <span className="text-brand-600 text-sm font-medium">Everything in one place</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-gray-900 tracking-tight mb-4">
            Your entire agency.<br />
            <span className="gradient-text">One platform.</span>
          </h2>
          <p className="text-gray-500 text-lg max-w-xl mx-auto">
            Stop juggling 6 different tools. Studio Mesh brings together every workflow your agency needs to operate and scale.
          </p>
        </div>

        {/* Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {features.map(({ icon: Icon, title, description, color }) => (
            <div
              key={title}
              className="group p-7 rounded-2xl border border-gray-100 hover:border-brand-100 bg-white hover:bg-brand-50/30 transition-all duration-200 hover:shadow-sm"
            >
              <div className={`w-11 h-11 rounded-xl flex items-center justify-center mb-5 ${color}`}>
                <Icon className="w-5 h-5" />
              </div>
              <h3 className="font-bold text-gray-900 text-lg mb-2">{title}</h3>
              <p className="text-gray-500 text-sm leading-relaxed">{description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

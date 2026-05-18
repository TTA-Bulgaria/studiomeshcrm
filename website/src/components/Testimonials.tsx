const testimonials = [
  {
    quote: "We used to run our agency on 5 different tools. Studio Mesh replaced all of them. Our team onboarded in a day.",
    author: "Miroslav P.",
    role: "Founder, Digital Agency — Bulgaria",
    initials: "MP",
    color: "bg-brand-600",
  },
  {
    quote: "The automation alone saves us 3–4 hours per new client. Proposal accepted → project created instantly. It's brilliant.",
    author: "Adaeze O.",
    role: "Operations Lead, Performance Agency — Nigeria",
    initials: "AO",
    color: "bg-emerald-600",
  },
  {
    quote: "Finally a CRM that understands agency workflows. The contract portal and e-signature feature is seamless.",
    author: "Stefan V.",
    role: "CEO, Growth Studio — Sofia",
    initials: "SV",
    color: "bg-violet-600",
  },
]

const Star = () => (
  <svg className="w-4 h-4 text-amber-400 fill-amber-400" viewBox="0 0 20 20">
    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
  </svg>
)

export default function Testimonials() {
  return (
    <section className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 bg-amber-50 border border-amber-100 rounded-full px-4 py-1.5 mb-4">
            <div className="flex gap-0.5">
              {[...Array(5)].map((_, i) => <Star key={i} />)}
            </div>
            <span className="text-amber-700 text-sm font-medium">4.9 / 5 rating</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-extrabold text-slate-900 tracking-tight mb-4 text-balance">
            Agencies love<br />
            <span className="gradient-text">Studio Mesh</span>
          </h2>
          <p className="text-slate-500 text-lg">Here's what early adopters are saying.</p>
        </div>

        <div className="grid md:grid-cols-3 gap-6">
          {testimonials.map(({ quote, author, role, initials, color }) => (
            <div
              key={author}
              className="bg-white rounded-2xl p-8 border border-slate-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200"
            >
              <div className="flex gap-1 mb-5">
                {[...Array(5)].map((_, i) => <Star key={i} />)}
              </div>
              <p className="text-slate-700 text-sm leading-relaxed mb-6">"{quote}"</p>
              <div className="flex items-center gap-3">
                <div className={`w-9 h-9 rounded-full ${color} flex items-center justify-center text-white text-xs font-bold shrink-0`}>
                  {initials}
                </div>
                <div>
                  <div className="text-sm font-semibold text-slate-900">{author}</div>
                  <div className="text-xs text-slate-500">{role}</div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

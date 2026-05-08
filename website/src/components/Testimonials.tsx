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

export default function Testimonials() {
  return (
    <section className="py-24 bg-gray-50">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="text-center mb-16">
          <h2 className="text-4xl md:text-5xl font-extrabold text-gray-900 tracking-tight mb-4">
            Agencies love<br />
            <span className="gradient-text">Studio Mesh</span>
          </h2>
          <p className="text-gray-500 text-lg">Here's what early adopters are saying.</p>
        </div>

        <div className="grid md:grid-cols-3 gap-6">
          {testimonials.map(({ quote, author, role, initials, color }) => (
            <div key={author} className="bg-white rounded-2xl p-8 border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
              {/* Stars */}
              <div className="flex gap-1 mb-5">
                {[...Array(5)].map((_, i) => (
                  <svg key={i} className="w-4 h-4 text-yellow-400 fill-current" viewBox="0 0 20 20">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                ))}
              </div>
              <p className="text-gray-700 text-sm leading-relaxed mb-6">"{quote}"</p>
              <div className="flex items-center gap-3">
                <div className={`w-9 h-9 rounded-full ${color} flex items-center justify-center text-white text-xs font-bold`}>
                  {initials}
                </div>
                <div>
                  <div className="text-sm font-semibold text-gray-900">{author}</div>
                  <div className="text-xs text-gray-500">{role}</div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

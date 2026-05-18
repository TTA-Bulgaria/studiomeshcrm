import Pricing from '@/components/Pricing'
import CTA from '@/components/CTA'
import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Pricing — Studio Mesh CRM',
  description: 'Simple, transparent pricing for agencies of all sizes. Start free for 14 days.',
}

export default function PricingPage() {
  return (
    <main>
      <div className="pt-20 bg-gray-950">
        <div className="max-w-7xl mx-auto px-6 lg:px-8 py-16 text-center">
          <h1 className="text-5xl md:text-6xl font-extrabold text-white tracking-tight mb-4">
            Simple, honest<br />
            <span className="gradient-text">pricing</span>
          </h1>
          <p className="text-gray-400 text-xl max-w-xl mx-auto">
            No hidden fees. No surprises. Just the tools your agency needs to grow.
          </p>
        </div>
      </div>
      <Pricing />
      <CTA />
    </main>
  )
}

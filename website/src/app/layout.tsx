import type { Metadata } from 'next'
import './globals.css'
import Navbar from '@/components/Navbar'
import Footer from '@/components/Footer'

export const metadata: Metadata = {
  title: 'Studio Mesh CRM — The Agency OS Built for Growth',
  description: 'Studio Mesh CRM is the all-in-one platform for digital agencies. Manage leads, projects, contracts, invoices, and ad performance in one place.',
  openGraph: {
    title: 'Studio Mesh CRM',
    description: 'The Agency OS Built for Growth',
    url: 'https://studiomeshcrm.com',
    siteName: 'Studio Mesh CRM',
    type: 'website',
  },
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Navbar />
        {children}
        <Footer />
      </body>
    </html>
  )
}

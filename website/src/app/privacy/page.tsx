import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Privacy Policy — Studio Mesh CRM',
  description: 'How Studio Mesh CRM collects, uses, and protects your data.',
}

export default function PrivacyPage() {
  return (
    <main className="pt-24 pb-16 bg-white">
      <div className="max-w-3xl mx-auto px-6 lg:px-8">
        <h1 className="text-4xl font-extrabold text-gray-900 mb-2">Privacy Policy</h1>
        <p className="text-gray-400 text-sm mb-10">Last updated: May 2025</p>

        <div className="prose prose-gray max-w-none">
          <p className="text-gray-600 leading-relaxed mb-6">
            Studio Mesh CRM ("we", "our", or "us") is committed to protecting your privacy. This Privacy Policy explains how we collect, use, disclose, and safeguard your information when you use our platform.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">1. Information We Collect</h2>
          <p className="text-gray-600 leading-relaxed mb-4">
            We collect information you provide directly to us, such as when you create an account, use our services, or contact us for support. This includes:
          </p>
          <ul className="list-disc pl-6 text-gray-600 space-y-2 mb-6">
            <li>Account information (name, email address, company name)</li>
            <li>Business data you input into the platform (clients, projects, invoices)</li>
            <li>Payment information (processed securely by our payment providers)</li>
            <li>Communications with our support team</li>
            <li>Usage data and analytics to improve our service</li>
          </ul>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">2. How We Use Your Information</h2>
          <p className="text-gray-600 leading-relaxed mb-4">We use the information we collect to:</p>
          <ul className="list-disc pl-6 text-gray-600 space-y-2 mb-6">
            <li>Provide, maintain, and improve our services</li>
            <li>Process transactions and send related information</li>
            <li>Send technical notices and support messages</li>
            <li>Respond to your comments and questions</li>
            <li>Monitor and analyze usage patterns to improve user experience</li>
          </ul>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">3. Data Security</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            We implement appropriate technical and organizational measures to protect your personal information against unauthorized access, alteration, disclosure, or destruction. Your data is encrypted in transit (TLS/SSL) and at rest.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">4. Data Retention</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            We retain your personal information for as long as your account is active or as needed to provide services. You may request deletion of your account and associated data at any time by contacting us.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">5. Third-Party Services</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            We may use third-party services for analytics, payments, and communication. These providers have their own privacy policies and we encourage you to review them. We do not sell your personal data to third parties.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">6. Your Rights</h2>
          <p className="text-gray-600 leading-relaxed mb-4">Depending on your location, you may have the right to:</p>
          <ul className="list-disc pl-6 text-gray-600 space-y-2 mb-6">
            <li>Access the personal data we hold about you</li>
            <li>Correct inaccurate data</li>
            <li>Request deletion of your data</li>
            <li>Object to or restrict processing of your data</li>
            <li>Data portability</li>
          </ul>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">7. Contact Us</h2>
          <p className="text-gray-600 leading-relaxed">
            If you have any questions about this Privacy Policy, please contact us at{' '}
            <a href="mailto:hello@studiomeshcrm.com" className="text-brand-600 hover:text-brand-500">
              hello@studiomeshcrm.com
            </a>.
          </p>
        </div>
      </div>
    </main>
  )
}

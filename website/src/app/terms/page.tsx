import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Terms of Service — Studio Mesh CRM',
  description: 'Terms and conditions for using Studio Mesh CRM.',
}

export default function TermsPage() {
  return (
    <main className="pt-24 pb-16 bg-white">
      <div className="max-w-3xl mx-auto px-6 lg:px-8">
        <h1 className="text-4xl font-extrabold text-gray-900 mb-2">Terms of Service</h1>
        <p className="text-gray-400 text-sm mb-10">Last updated: May 2025</p>

        <div className="prose prose-gray max-w-none">
          <p className="text-gray-600 leading-relaxed mb-6">
            By accessing or using Studio Mesh CRM ("the Service"), you agree to be bound by these Terms of Service. Please read them carefully before using the platform.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">1. Acceptance of Terms</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            By creating an account or using any part of the Service, you agree to these Terms. If you do not agree, you may not use the Service. These Terms apply to all users, including trial users, paying customers, and any team members invited to your account.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">2. Use of the Service</h2>
          <p className="text-gray-600 leading-relaxed mb-4">You agree to use the Service only for lawful purposes and in accordance with these Terms. You must not:</p>
          <ul className="list-disc pl-6 text-gray-600 space-y-2 mb-6">
            <li>Use the Service for any fraudulent or illegal purpose</li>
            <li>Attempt to gain unauthorized access to any part of the Service</li>
            <li>Transmit viruses or any code of a destructive nature</li>
            <li>Violate any applicable laws or regulations</li>
            <li>Resell or sublicense access to the Service without permission</li>
          </ul>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">3. Account Responsibilities</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            You are responsible for maintaining the confidentiality of your account credentials and for all activity that occurs under your account. Notify us immediately at hello@studiomeshcrm.com if you suspect unauthorized use of your account.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">4. Subscription and Billing</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            Paid plans are billed monthly or annually. All fees are non-refundable except where required by law. We reserve the right to modify pricing with 30 days' notice. Failure to pay may result in suspension of your account.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">5. Data Ownership</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            You retain ownership of all data you input into the Service. By using the Service, you grant us a limited license to store and process your data solely to provide the Service. We do not claim ownership over your content.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">6. Intellectual Property</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            The Service and its original content, features, and functionality are and will remain the exclusive property of Studio Mesh CRM. Our trademarks may not be used in connection with any product or service without prior written consent.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">7. Limitation of Liability</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            To the maximum extent permitted by law, Studio Mesh CRM shall not be liable for any indirect, incidental, special, consequential, or punitive damages. Our total liability shall not exceed the amount you paid us in the 12 months preceding the claim.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">8. Termination</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            You may cancel your account at any time. We reserve the right to suspend or terminate accounts that violate these Terms. Upon termination, your right to use the Service will immediately cease, and we may delete your data after a reasonable period.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">9. Changes to Terms</h2>
          <p className="text-gray-600 leading-relaxed mb-6">
            We may update these Terms from time to time. We will notify you of significant changes by email or through the Service. Continued use after changes constitutes acceptance of the new Terms.
          </p>

          <h2 className="text-xl font-bold text-gray-900 mt-10 mb-4">10. Contact</h2>
          <p className="text-gray-600 leading-relaxed">
            For questions about these Terms, contact us at{' '}
            <a href="mailto:hello@studiomeshcrm.com" className="text-brand-600 hover:text-brand-500">
              hello@studiomeshcrm.com
            </a>.
          </p>
        </div>
      </div>
    </main>
  )
}

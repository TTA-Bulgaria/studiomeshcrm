'use client'

import { useState, useEffect } from 'react'
import Link from 'next/link'
import { Menu, X, Zap } from 'lucide-react'

const APP_URL = 'https://app.studiomeshcrm.com'

export default function Navbar() {
  const [scrolled, setScrolled] = useState(false)
  const [open, setOpen] = useState(false)

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 20)
    window.addEventListener('scroll', onScroll)
    return () => window.removeEventListener('scroll', onScroll)
  }, [])

  return (
    <header className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${
      scrolled ? 'bg-white/95 backdrop-blur-sm shadow-sm border-b border-slate-100' : 'bg-white/80 backdrop-blur-sm'
    }`}>
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg bg-brand-600 flex items-center justify-center">
              <Zap className="w-4 h-4 text-white" />
            </div>
            <span className="font-bold text-lg text-slate-900">Studio Mesh</span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden md:flex items-center gap-8">
            {[
              { label: 'Features', href: '/#features' },
              { label: 'How it works', href: '/#how-it-works' },
              { label: 'Pricing', href: '/#pricing' },
            ].map(({ label, href }) => (
              <Link
                key={label}
                href={href}
                className="text-sm font-medium text-slate-600 hover:text-slate-900 transition-colors"
              >
                {label}
              </Link>
            ))}
          </nav>

          {/* CTA */}
          <div className="hidden md:flex items-center gap-3">
            <a
              href={`${APP_URL}/login`}
              className="text-sm font-medium text-slate-600 hover:text-slate-900 transition-colors"
            >
              Sign in
            </a>
            <a
              href={`${APP_URL}/register`}
              className="text-sm font-semibold bg-brand-600 hover:bg-brand-700 text-white px-4 py-2 rounded-lg transition-colors shadow-sm"
            >
              Start free trial
            </a>
          </div>

          {/* Mobile toggle */}
          <button
            onClick={() => setOpen(!open)}
            className="md:hidden p-2 rounded-lg text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-colors"
          >
            {open ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* Mobile menu */}
      {open && (
        <div className="md:hidden bg-white border-t border-slate-100 px-6 py-4 space-y-1">
          <Link href="/#features" onClick={() => setOpen(false)} className="block text-sm font-medium text-slate-700 py-2.5">Features</Link>
          <Link href="/#how-it-works" onClick={() => setOpen(false)} className="block text-sm font-medium text-slate-700 py-2.5">How it works</Link>
          <Link href="/#pricing" onClick={() => setOpen(false)} className="block text-sm font-medium text-slate-700 py-2.5">Pricing</Link>
          <div className="pt-3 border-t border-slate-100 flex flex-col gap-2">
            <a href={`${APP_URL}/login`} className="text-sm font-medium text-slate-700 py-2">Sign in</a>
            <a href={`${APP_URL}/register`} className="text-sm font-semibold bg-brand-600 text-white px-4 py-2.5 rounded-lg text-center">Start free trial</a>
          </div>
        </div>
      )}
    </header>
  )
}

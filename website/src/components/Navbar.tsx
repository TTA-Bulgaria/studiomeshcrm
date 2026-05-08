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
    <header className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${scrolled ? 'bg-white/95 backdrop-blur shadow-sm border-b border-gray-100' : 'bg-transparent'}`}>
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg gradient-bg flex items-center justify-center">
              <Zap className="w-4 h-4 text-white" />
            </div>
            <span className={`font-bold text-lg transition-colors ${scrolled ? 'text-gray-900' : 'text-white'}`}>
              Studio Mesh
            </span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden md:flex items-center gap-8">
            {[
              { label: 'Features', href: '/#features' },
              { label: 'Pricing', href: '/pricing' },
              { label: 'About', href: '/#about' },
            ].map(({ label, href }) => (
              <Link
                key={label}
                href={href}
                className={`text-sm font-medium transition-colors hover:text-brand-400 ${scrolled ? 'text-gray-600' : 'text-white/80'}`}
              >
                {label}
              </Link>
            ))}
          </nav>

          {/* CTA */}
          <div className="hidden md:flex items-center gap-3">
            <a
              href={`${APP_URL}/login`}
              className={`text-sm font-medium transition-colors ${scrolled ? 'text-gray-600 hover:text-brand-600' : 'text-white/80 hover:text-white'}`}
            >
              Sign in
            </a>
            <a
              href={`${APP_URL}/register`}
              className="text-sm font-semibold bg-brand-600 hover:bg-brand-700 text-white px-4 py-2 rounded-lg transition-colors"
            >
              Start free trial
            </a>
          </div>

          {/* Mobile toggle */}
          <button
            onClick={() => setOpen(!open)}
            className={`md:hidden p-2 rounded-lg ${scrolled ? 'text-gray-600' : 'text-white'}`}
          >
            {open ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* Mobile menu */}
      {open && (
        <div className="md:hidden bg-white border-t border-gray-100 px-6 py-4 space-y-3">
          <Link href="/#features" onClick={() => setOpen(false)} className="block text-sm font-medium text-gray-700 py-2">Features</Link>
          <Link href="/pricing" onClick={() => setOpen(false)} className="block text-sm font-medium text-gray-700 py-2">Pricing</Link>
          <Link href="/#about" onClick={() => setOpen(false)} className="block text-sm font-medium text-gray-700 py-2">About</Link>
          <div className="pt-2 border-t border-gray-100 flex flex-col gap-2">
            <a href={`${APP_URL}/login`} className="text-sm font-medium text-gray-700 py-2">Sign in</a>
            <a href={`${APP_URL}/register`} className="text-sm font-semibold bg-brand-600 text-white px-4 py-2.5 rounded-lg text-center">Start free trial</a>
          </div>
        </div>
      )}
    </header>
  )
}

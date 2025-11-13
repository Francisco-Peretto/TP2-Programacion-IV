import React from 'react';
import Header from './Header';

export default function Layout({children}) {
  return (
    <div className="min-h-screen bg-[var(--formal-bg)] text-[color:var(--formal-text)] antialiased">
      <Header />
      <main className="max-w-6xl mx-auto p-6">
        {children}
      </main>
      <footer className="footer muted text-sm text-center py-6">© {new Date().getFullYear()} — Proyecto</footer>
    </div>
  );
}

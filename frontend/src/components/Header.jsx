import React from 'react';
import { Link, useLocation } from 'wouter';
import { useAuthStore } from '../store/auth';

export default function Header(){
  const [location] = useLocation();
  const { isAuthenticated, authToken, logout } = useAuthStore ? useAuthStore() : { isAuthenticated:false, logout:()=>{} };

  return (
    <header className="border-b border-[color:var(--formal-border)] bg-[var(--formal-surface)]">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link href="/"><a className="text-xl font-semibold tracking-wide">Proyecto Cursos</a></Link>
        <nav className="flex items-center gap-3">
          <Link href="/"><a className="btn" disabled={location === "/"}>Inicio</a></Link>
          <Link href="/elementos"><a className="btn">Elementos</a></Link>
          {authToken && (
            <Link href="/admin"><a className="btn" disabled={location === "/admin"}>Admin</a></Link>
            )}
          {isAuthenticated ? (
            <button onClick={logout} className="btn">Cerrar sesión</button>
          ) : (
            <Link href="/login"><a className="btn" disabled={location === "/Login"}>Iniciar sesión</a></Link>
          )}
        </nav>
      </div>
    </header>
  );
}

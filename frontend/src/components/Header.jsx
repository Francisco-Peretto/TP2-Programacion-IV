// src/components/Header.jsx
import React from "react";
import { Link, useLocation } from "wouter";
import { useAuthStore } from "../store/auth";

function isAdmin(user) {
  if (!user) return false;

  const roleName =
    user.role?.name ?? // { role: { name: "Admin" } }
    user.role ??       // "Admin"
    user.roleName ?? "";

  if (typeof roleName === "string" && roleName) {
    return roleName === "Admin";
  }

  if (typeof user.roleId === "number") {
    return user.roleId === 1;
  }

  // fallback para el admin de seed
  if (user.email === "admin@demo.com") return true;

  return false;
}

export default function Header() {
  const [location] = useLocation();

  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const user = useAuthStore((s) => s.user);
  const logout = useAuthStore((s) => s.logout);

  const showAdmin = isAuthenticated && isAdmin(user);

  // helper para marcar el botón activo
  const navClass = (path) =>
    `btn ${location === path ? "opacity-75 pointer-events-none" : ""}`;

  return (
    <header className="border-b border-[color:var(--formal-border)] bg-[var(--formal-surface)]">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link href="/" className="text-xl font-semibold tracking-wide">
          Proyecto Cursos
        </Link>

        <nav className="flex items-center gap-3">
          <Link href="/" className={navClass("/")}>
            Inicio
          </Link>

          <Link href="/elementos" className={navClass("/elementos")}>
            Elementos
          </Link>

          {showAdmin && (
            <Link href="/admin" className={navClass("/admin")}>
              Admin
            </Link>
          )}

          {isAuthenticated ? (
            <button onClick={logout} className="btn">
              Cerrar sesión
            </button>
          ) : (
            <Link href="/login" className={navClass("/login")}>
              Iniciar sesión
            </Link>
          )}
        </nav>
      </div>
    </header>
  );
}

// src/components/Header.jsx
import React from "react";
import { Link, useLocation } from "wouter";
import { useAuthStore } from "../store/auth";

function isAdmin(user) {
  if (!user) return false;

  const roleName =
    user.role?.name ??
    user.role ??
    user.roleName ?? "";

  if (typeof roleName === "string" && roleName) {
    return roleName === "Admin";
  }

  if (typeof user.roleId === "number") {
    return user.roleId === 1;
  }

  // seed fallback
  if (user.email === "admin@demo.com") return true;

  return false;
}

export default function Header() {
  const [location] = useLocation();

  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const user = useAuthStore((s) => s.user);
  const logout = useAuthStore((s) => s.logout);

  const showAdmin = isAuthenticated && isAdmin(user);

  const navClass = (path) =>
    `btn ${location === path ? "opacity-75 pointer-events-none" : ""}`;

  return (
    <header className="border-b border-[color:var(--formal-border)] bg-[var(--formal-surface)]">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link href="/" className="text-xl font-semibold tracking-wide">
          Courses Project
        </Link>

        <nav className="flex items-center gap-3">
          <Link href="/" className={navClass("/")}>
            Home
          </Link>

          <Link href="/courses" className={navClass("/courses")}>
            Courses
          </Link>

          {showAdmin && (
            <Link href="/admin" className={navClass("/admin")}>
              Dashboard   {/* ⬅️ changed label here */}
            </Link>
          )}

          {isAuthenticated ? (
            <button onClick={logout} className="btn">
              Logout
            </button>
          ) : (
            <Link href="/login" className={navClass("/login")}>
              Login
            </Link>
          )}
        </nav>
      </div>
    </header>
  );
}

import React from "react";
import { Link } from "wouter";
import { useAuthStore } from "../store/auth";

export default function Home() {
  const { isAuthenticated, role, user, logout } = useAuthStore();

  return (
    <div className="p-6 space-y-3">
      <h1 className="text-xl font-semibold">Inicio</h1>
      {!isAuthenticated ? (
        <Link href="/login"><a className="text-blue-700 underline">Iniciar sesión</a></Link>
      ) : (
        <>
          <div>Hola {user?.name || user?.email}</div>
          <div>Rol: {role}</div>
          <button className="border px-3 py-1 rounded" onClick={logout}>Cerrar sesión</button>
        </>
      )}
      <div className="space-x-3">
        <Link href="/elementos"><a className="underline">Elementos</a></Link>
        <Link href="/admin"><a className="underline">Admin</a></Link>
      </div>
    </div>
  );
}

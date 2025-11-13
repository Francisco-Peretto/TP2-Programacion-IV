// src/components/ProtectedRoute.jsx
import React, { useEffect } from "react";
import { Route, useLocation } from "wouter";
import { useAuthStore } from "../store/auth";

function userHasRole(user, requiredRole) {
  if (!requiredRole) return true;
  if (!user) return false;

  const roleName =
    user.role?.name ??   // { role: { name: "Admin" } }
    user.role ??         // "Admin"
    user.roleName ?? ""; // "Admin"

  if (typeof roleName === "string" && roleName) {
    return roleName === requiredRole;
  }

  // Por ID de rol
  if (requiredRole === "Admin" && typeof user.roleId === "number") {
    return user.roleId === 1;
  }

  // Fallback: admin seed
  if (requiredRole === "Admin" && user.email === "admin@demo.com") {
    return true;
  }

  return false;
}

function Guard({ requiredRole, children }) {
  const [, navigate] = useLocation();

  // ⬅️ Selectores separados (sin objeto nuevo en cada render)
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const user = useAuthStore((s) => s.user);

  useEffect(() => {
    console.log("ProtectedRoute check:", {
      isAuthenticated,
      user,
      requiredRole,
    });

    if (!isAuthenticated) {
      navigate("/login");
      return;
    }

    if (requiredRole && !userHasRole(user, requiredRole)) {
      navigate("/");
      return;
    }
  }, [isAuthenticated, user, requiredRole, navigate]);

  // Mientras redirige o no tiene permisos, no mostramos nada
  if (
    !isAuthenticated ||
    (requiredRole && !userHasRole(user, requiredRole))
  ) {
    return null;
  }

  return children;
}

export default function ProtectedRoute({ path, requiredRole, children }) {
  return (
    <Route path={path}>
      {() => <Guard requiredRole={requiredRole}>{children}</Guard>}
    </Route>
  );
}

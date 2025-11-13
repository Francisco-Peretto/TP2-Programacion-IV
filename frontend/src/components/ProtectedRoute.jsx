import React, { useEffect } from "react";
import { Route, useLocation } from "wouter";
import { useAuthStore } from "../store/auth";

export default function ProtectedRoute({ path, requiredRole, children }) {
  const [, navigate] = useLocation();
  const { isAuthenticated, role } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated) navigate("/login");
    else if (requiredRole && role !== requiredRole) navigate("/");
  }, [isAuthenticated, role, requiredRole, navigate]);

  return (
    <Route path={path}>
      {isAuthenticated && (!requiredRole || role === requiredRole) ? children : null}
    </Route>
  );
}

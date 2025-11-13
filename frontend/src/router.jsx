import React, { Suspense } from "react";
import { Route, Switch } from "wouter";
import ProtectedRoute from "./components/ProtectedRoute";
import Home from "./pages/Home";
import Login from "./pages/Login";
import ElementList from "./pages/ElementList";
import ElementForm from "./pages/ElementForm";

const ElementDetail = React.lazy(() => import("./pages/ElementDetail"));
const AdminPanel    = React.lazy(() => import("./pages/AdminPanel"));

export default function AppRouter() {
  return (
    <Suspense fallback={<div className="p-6">Cargando…</div>}>
      <Switch>
        <Route path="/" component={Home} />
        <Route path="/login" component={Login} />
        <Route path="/elementos" component={ElementList} />
        <Route path="/elementos/:id">{(p) => <ElementDetail id={p.id} />}</Route>

        <ProtectedRoute path="/admin" requiredRole="Admin">
          <AdminPanel />
        </ProtectedRoute>

        <ProtectedRoute path="/admin/elementos/new" requiredRole="Admin">
          <ElementForm mode="create" />
        </ProtectedRoute>

        <Route>⚠️ 404 — Ruta no encontrada</Route>
      </Switch>
    </Suspense>
  );
}

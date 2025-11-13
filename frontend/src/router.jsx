import React, { Suspense } from "react";
import { Route, Switch } from "wouter";
import ProtectedRoute from "./components/ProtectedRoute";
import Home from "./pages/Home";
import Login from "./pages/Login";
import CourseList from "./pages/CourseList";
import CourseForm from "./pages/CourseForm";

const CourseDetail = React.lazy(() => import("./pages/CourseDetail"));
const AdminPanel = React.lazy(() => import("./pages/AdminPanel"));

export default function AppRouter() {
  return (
    <Suspense fallback={<div className="p-6">Loading…</div>}>
      <Switch>
        <Route path="/" component={Home} />
        <Route path="/login" component={Login} />

        {/* Courses list & detail */}
        <Route path="/courses" component={CourseList} />
        <Route path="/courses/:id">
          {(p) => <CourseDetail id={p.id} />}
        </Route>

        {/* Admin area */}
        <ProtectedRoute path="/admin" requiredRole="Admin">
          <AdminPanel />
        </ProtectedRoute>

        {/* New course (admin only) */}
        <ProtectedRoute path="/admin/courses/new" requiredRole="Admin">
          <CourseForm mode="create" />
        </ProtectedRoute>

        <Route>⚠️ 404 — Route not found</Route>
      </Switch>
    </Suspense>
  );
}

import { Route, Switch, Redirect } from "wouter";
import { Suspense, lazy } from "react";
import Home from "./pages/Home";
import Login from "./pages/Login";
import ElementList from "./pages/ElementList";
import ElementForm from "./pages/ElementForm";
import ProtectedRoute from "./components/ProtectedRoute";

const ElementDetail = lazy(() => import("./pages/ElementDetail"));
const AdminPanel = lazy(() => import("./pages/AdminPanel"));

export default function AppRouter() {
    return (
        <Suspense fallback={<div>Cargando...</div>}>
            <Switch>
                <Route path="/" component={Home} />
                <Route path="/login" component={Login} />
                <Route path="/elementos" component={ElementList} />
                <Route path="/elementos/:id" component={ElementDetail} />
                <ProtectedRoute path="/admin" roles={["Admin"]}>
                    <AdminPanel />
                </ProtectedRoute>
                <ProtectedRoute path="/admin/elementos/new" roles={["Admin"]}>
                    <ElementForm />
                </ProtectedRoute>
                <Redirect to="/" />
            </Switch>
        </Suspense>
    );
}

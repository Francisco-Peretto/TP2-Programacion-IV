import { useRoute } from "wouter";
import useAuth from "../store/auth";

export default function ProtectedRoute({ path, roles, children }) {
    const [match] = useRoute(path);
    const { isAuthenticated, user } = useAuth();

    if (!match) return null;
    if (!isAuthenticated) return <div>Acceso restringido. Inicia sesión.</div>;
    if (roles && !roles.includes(user?.role)) return <div>No autorizado.</div>;

    return <>{children}</>;
}

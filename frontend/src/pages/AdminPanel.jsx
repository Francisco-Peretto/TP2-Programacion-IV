import { useEffect, useState } from "react";
import api from "../api/axios";

export default function AdminPanel() {
    const [m, setM] = useState(null);

    useEffect(() => {
        api.get("/admin/metrics").then((r) => setM(r.data));
    }, []);

    if (!m) return <div>Cargando...</div>;

    return (
        <section>
            <h2>Panel Admin</h2>
            <div>Total cursos: {m.totalCourses}</div>
            <div>Total usuarios: {m.totalUsers}</div>
            <ul>
                {m.byCategory.map((x) => (
                    <li key={x.category}>{x.category}: {x.count}</li>
                ))}
            </ul>
        </section>
    );
}

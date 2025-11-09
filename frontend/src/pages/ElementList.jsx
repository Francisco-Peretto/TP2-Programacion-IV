import { useEffect, useState } from "react";
import api from "../api/axios";

export default function ElementList() {
    const [items, setItems] = useState([]);
    const [q, setQ] = useState("");
    const [category, setCategory] = useState("");
    const [page, setPage] = useState(1);

    useEffect(() => {
        const fetchData = async () => {
            const res = await api.get("/courses", { params: { page, pageSize: 10, q, category } });
            setItems(res.data.items || []);
        };
        fetchData();
    }, [q, category, page]);

    return (
        <div>
            <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Buscar..." />
            <select value={category} onChange={(e) => setCategory(e.target.value)}>
                <option value="">Todas</option>
                <option value="Web">Web</option>
                <option value="Data">Data</option>
            </select>
            <ul>
                {items.map((i) => (
                    <li key={i.id}>{i.title} — {i.category}</li>
                ))}
            </ul>
            <button onClick={() => setPage((p) => Math.max(1, p - 1))}>Prev</button>
            <button onClick={() => setPage((p) => p + 1)}>Next</button>
        </div>
    );
}

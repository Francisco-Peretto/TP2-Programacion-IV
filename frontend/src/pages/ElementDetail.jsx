import { useEffect, useState } from "react";
import { useRoute } from "wouter";
import api from "../api/axios";

export default function ElementDetail() {
    const [, params] = useRoute("/elementos/:id");
    const id = Number(params?.id);
    const [item, setItem] = useState(null);

    useEffect(() => {
        api.get(`/courses/${id}`).then((r) => setItem(r.data));
    }, [id]);

    if (!item) return <div>Cargando...</div>;

    return (
        <article>
            <h2>{item.title}</h2>
            <p>{item.description}</p>
            <p>Categoría: {item.category} | Precio: {item.price}</p>
        </article>
    );
}

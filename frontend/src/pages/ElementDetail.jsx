import React, { useEffect, useState } from "react";
import { api } from "../api/axios";

export default function ElementDetail({ id }) {
  const [item, setItem] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let alive = true;
    (async () => {
      setLoading(true);
      try {
        const { data } = await api.get(`/elementos/${id}`);
        if (alive) setItem(data);
      } finally {
        if (alive) setLoading(false);
      }
    })();
    return () => { alive = false; };
  }, [id]);

  if (loading) return <div className="p-6">Cargando…</div>;
  if (!item) return <div className="p-6">No encontrado</div>;

  return (
    <div className="p-6 space-y-2">
      <h1 className="text-xl font-semibold">{item.title}</h1>
      <div className="text-gray-700">{item.description}</div>
      <div className="text-sm text-gray-500">Categoría: {item.category}</div>
      <div className="text-sm text-gray-500">Cantidad: {item.quantity}</div>
    </div>
  );
}

import React, { useEffect, useState } from "react";
import { Link } from "wouter";
import { api } from "../api/axios";

export default function ElementList() {
  const [items, setItems] = useState([]);
  const [q, setQ] = useState("");
  const [category, setCategory] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let alive = true;
    (async () => {
      setLoading(true);
      try {
        const { data } = await api.get("/elementos");
        if (alive) setItems(data || []);
      } finally {
        if (alive) setLoading(false);
      }
    })();
    return () => { alive = false; };
  }, []);

  const filtered = items.filter(it => {
    const matchQ = !q || it.title?.toLowerCase().includes(q.toLowerCase());
    const matchCat = !category || it.category === category;
    return matchQ && matchCat;
  });

  return (
    <div className="p-6 space-y-4">
      <div className="flex gap-2 items-end">
        <div>
          <label className="block text-sm">Búsqueda</label>
          <input className="border px-3 py-2" value={q} onChange={e => setQ(e.target.value)} />
        </div>
        <div>
          <label className="block text-sm">Categoría</label>
          <input className="border px-3 py-2" value={category} onChange={e => setCategory(e.target.value)} />
        </div>
        <Link href="/admin/elementos/new" className="ml-auto">
          <a className="btn-primary">Nuevo</a>
        </Link>
      </div>

      {loading && <div>Cargando…</div>}
      {!loading && filtered.length === 0 && <div>No hay elementos</div>}

      <ul className="grid gap-2">
        {filtered.map((it) => (
          <li key={it.id} className="border rounded p-3 flex justify-between items-center">
            <div>
              <div className="font-semibold">{it.title}</div>
              <div className="text-sm text-gray-600">{it.category}</div>
            </div>
            <Link href={`/elementos/${it.id}`}>
              <a className="text-blue-700 underline">Ver detalle</a>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}

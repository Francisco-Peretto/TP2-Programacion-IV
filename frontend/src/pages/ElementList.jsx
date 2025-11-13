// src/pages/ElementList.jsx
import React, { useEffect, useState, useMemo } from "react";
import { api } from "../api/axios";
import { Link } from "wouter";
import { toast } from "react-toastify";

export default function ElementList() {
  const [items, setItems] = useState([]);      // ⬅️ SIEMPRE array
  const [search, setSearch] = useState("");

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const res = await api.get("/Course");  // ⬅️ NUEVA RUTA
        let data = res.data;

        // Normalizamos la forma de los datos
        if (Array.isArray(data)) {
          // OK, ya es array
        } else if (Array.isArray(data.items)) {
          data = data.items;
        } else if (Array.isArray(data.courses)) {
          data = data.courses;
        } else {
          console.warn("Respuesta inesperada de /api/Course:", data);
          data = [];
        }

        if (!cancelled) setItems(data);
      } catch (err) {
        console.error("Error cargando cursos:", err);
        if (!cancelled) {
          toast.error("No se pudieron cargar los elementos");
          setItems([]); // dejamos un array vacío para evitar más errores
        }
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, []);

  // Filtro de búsqueda simple (por nombre / título)
  const filtered = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return items;
    return items.filter((it) =>
      (it.name ?? it.title ?? "")
        .toString()
        .toLowerCase()
        .includes(term)
    );
  }, [items, search]);

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Elementos</h1>

        {/* Botón "Nuevo" que va a la ruta protegida */}
      <Link href="/admin/elementos/new" className="btn-primary">
        Nuevo
      </Link>
      </div>

      <div className="mb-4">
        <input
          className="border px-3 py-2 w-full max-w-sm"
          placeholder="Buscar…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {filtered.length === 0 ? (
        <p className="text-sm text-slate-400">No hay elementos para mostrar.</p>
      ) : (
        <ul className="space-y-3">
          {filtered.map((it) => (
            <li
              key={it.id}
              className="border border-slate-700 rounded-lg p-4 flex items-center justify-between"
            >
              <div>
                <p className="font-medium">
                  {it.name ?? it.title ?? `Elemento #${it.id}`}
                </p>
                <p className="text-sm text-slate-400">
                  {it.description ?? it.details ?? ""}
                </p>
              </div>
              <Link href={`/elementos/${it.id}`}>
                <a className="text-blue-400 text-sm hover:underline">
                  Ver detalle
                </a>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

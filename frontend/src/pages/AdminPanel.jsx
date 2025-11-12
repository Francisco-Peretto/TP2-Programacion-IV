import React, { useEffect, useState } from "react";
import { api } from "../api/axios";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from "recharts";

export default function AdminPanel() {
  const [loading, setLoading] = useState(true);
  const [totals, setTotals] = useState({ elements: 0, users: 0 });
  const [byCat, setByCat] = useState([]);

  useEffect(() => {
    let alive = true;
    (async () => {
      setLoading(true);
      try {
        const [{ data: elements }, { data: users }] = await Promise.all([
          api.get("/elementos"),
          api.get("/users"),
        ]);

        const elementsCount = elements?.length || 0;
        const usersCount = users?.length || 0;

        const map = new Map();
        elements?.forEach((e) => {
          const c = e.category || "Sin categoría";
          map.set(c, (map.get(c) || 0) + 1);
        });
        const chartData = Array.from(map, ([name, value]) => ({ name, value }));

        if (alive) {
          setTotals({ elements: elementsCount, users: usersCount });
          setByCat(chartData);
        }
      } finally {
        if (alive) setLoading(false);
      }
    })();
    return () => { alive = false; };
  }, []);

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-xl font-semibold">Panel de administración</h1>
      {loading && <div>Cargando…</div>}
      {!loading && (
        <>
          <div className="grid grid-cols-2 gap-4 max-w-xl">
            <div className="border rounded p-4">
              <div className="text-sm text-gray-500">Total elementos</div>
              <div className="text-2xl font-bold">{totals.elements}</div>
            </div>
            <div className="border rounded p-4">
              <div className="text-sm text-gray-500">Total usuarios</div>
              <div className="text-2xl font-bold">{totals.users}</div>
            </div>
          </div>

          <div className="h-72 w-full max-w-3xl">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={byCat}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar dataKey="value" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </>
      )}
    </div>
  );
}

// src/pages/CourseDetail.jsx
import React, { useEffect, useState } from "react";
import { api } from "../api/axios";

export default function CourseDetail({ id }) {
  const [item, setItem] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let alive = true;

    (async () => {
      setLoading(true);
      try {
        const { data } = await api.get(`/Course/${id}`);
        if (alive) setItem(data);
      } finally {
        if (alive) setLoading(false);
      }
    })();

    return () => {
      alive = false;
    };
  }, [id]);

  if (loading) return <div className="p-6">Loading…</div>;
  if (!item) return <div className="p-6">Not found</div>;

  return (
    <div className="p-6 space-y-3 max-w-xl mx-auto">
      <h1 className="text-2xl font-semibold">
        {item.name ?? item.title ?? `Course #${item.id}`}
      </h1>

      <div className="text-gray-300">
        {item.description ?? "No description available."}
      </div>

      <div className="text-sm text-gray-500">ID: {item.id}</div>
    </div>
  );
}

// src/pages/CourseList.jsx
import React, { useEffect, useState, useMemo } from "react";
import { api } from "../api/axios";
import { Link } from "wouter";
import { toast } from "react-toastify";

export default function CourseList() {
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const res = await api.get("/Course");
        let data = res.data;

        if (Array.isArray(data)) {
          // OK
        } else if (Array.isArray(data.items)) {
          data = data.items;
        } else if (Array.isArray(data.courses)) {
          data = data.courses;
        } else {
          console.warn("Unexpected response from /api/Course:", data);
          data = [];
        }

        if (!cancelled) setItems(data);
      } catch (err) {
        console.error("Error loading courses:", err);
        if (!cancelled) {
          toast.error("Could not load courses");
          setItems([]);
        }
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, []);

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
        <h1 className="text-xl font-semibold">Courses</h1>

        <Link href="/admin/courses/new" className="btn-primary">
          New course
        </Link>
      </div>

      <div className="mb-4">
        <input
          className="border px-3 py-2 w-full max-w-sm"
          placeholder="Search…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {filtered.length === 0 ? (
        <p className="text-sm text-slate-400">No courses to display.</p>
      ) : (
        <ul className="space-y-3">
          {filtered.map((it) => (
            <li
              key={it.id}
              className="border border-slate-700 rounded-lg p-4 flex items-center justify-between"
            >
              <div>
                <p className="font-medium">
                  {it.name ?? it.title ?? `Course #${it.id}`}
                </p>
                <p className="text-sm text-slate-400">
                  {it.description ?? it.details ?? ""}
                </p>
              </div>

              <Link
                href={`/courses/${it.id}`}
                className="text-blue-400 text-sm hover:underline"
              >
                View details
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

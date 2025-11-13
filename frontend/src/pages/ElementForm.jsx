import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { api } from "../api/axios";
import { useLocation } from "wouter";
import { toast } from "react-toastify";

const schema = z.object({
  title: z.string().min(1, "Título requerido"),
  description: z.string().min(1, "Descripción requerida"),
  category: z.string().min(1, "Categoría requerida"),
  quantity: z.string().transform(v => Number(v)).pipe(z.number().int().min(0, "No negativo")),
});

export default function ElementForm({ mode = "create", defaultValues }) {
  const [, navigate] = useLocation();
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(schema),
    defaultValues: defaultValues || { title: "", description: "", category: "", quantity: "0" },
  });

  const onSubmit = async (values) => {
    setSaving(true);
    try {
      const op = mode === "create"
        ? api.post("/elementos", values)
        : api.put(`/elementos/${defaultValues.id}`, values);

      await toast.promise(op, {
        pending: mode === "create" ? "Creando…" : "Actualizando…",
        success: mode === "create" ? "Elemento creado" : "Elemento actualizado",
        error: "No se pudo guardar el elemento",
      });

      navigate("/elementos");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-xl font-semibold mb-4">
        {mode === "create" ? "Crear elemento" : "Editar elemento"}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
        <div>
          <label className="block text-sm">Título</label>
          <input className="border px-3 py-2 w-full" {...register("title")} />
          {errors.title && <p className="text-red-600 text-sm">{errors.title.message}</p>}
        </div>
        <div>
          <label className="block text-sm">Descripción</label>
          <textarea className="border px-3 py-2 w-full" rows="3" {...register("description")} />
          {errors.description && <p className="text-red-600 text-sm">{errors.description.message}</p>}
        </div>
        <div>
          <label className="block text-sm">Categoría</label>
          <input className="border px-3 py-2 w-full" {...register("category")} />
          {errors.category && <p className="text-red-600 text-sm">{errors.category.message}</p>}
        </div>
        <div>
          <label className="block text-sm">Cantidad</label>
          <input className="border px-3 py-2 w-full" type="number" {...register("quantity")} />
          {errors.quantity && <p className="text-red-600 text-sm">{errors.quantity.message}</p>}
        </div>

        <button disabled={saving} className="btn">
          {saving ? "Guardando…" : "Guardar"}
        </button>
      </form>
    </div>
  );
}

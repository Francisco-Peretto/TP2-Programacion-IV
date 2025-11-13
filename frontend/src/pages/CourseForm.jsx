// src/pages/CourseForm.jsx
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { api } from "../api/axios";
import { useLocation } from "wouter";
import { toast } from "react-toastify";

const schema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().min(1, "Description is required"),
  category: z.string().min(1, "Category is required"),
  quantity: z
    .string()
    .transform((v) => Number(v))
    .pipe(z.number().int().min(0, "Quantity cannot be negative")),
});

export default function CourseForm({ mode = "create", defaultValues }) {
  const [, navigate] = useLocation();
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(schema),
    defaultValues:
      defaultValues || {
        title: "",
        description: "",
        category: "",
        quantity: "0",
      },
  });

  const onSubmit = async (values) => {
    setSaving(true);
    try {
      const op =
        mode === "create"
          ? api.post("/Course", values)
          : api.put(`/Course/${defaultValues.id}`, values);

      await toast.promise(op, {
        pending: mode === "create" ? "Creating…" : "Updating…",
        success: mode === "create" ? "Course created" : "Course updated",
        error: "Could not save the course",
      });

      navigate("/courses");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-xl font-semibold mb-4">
        {mode === "create" ? "Create course" : "Edit course"}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
        <div>
          <label className="block text-sm">Title</label>
          <input
            className="border px-3 py-2 w-full"
            {...register("title")}
          />
          {errors.title && (
            <p className="text-red-600 text-sm">{errors.title.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm">Description</label>
          <textarea
            className="border px-3 py-2 w-full"
            rows="3"
            {...register("description")}
          />
          {errors.description && (
            <p className="text-red-600 text-sm">
              {errors.description.message}
            </p>
          )}
        </div>

        <div>
          <label className="block text-sm">Category</label>
          <input
            className="border px-3 py-2 w-full"
            {...register("category")}
          />
          {errors.category && (
            <p className="text-red-600 text-sm">
              {errors.category.message}
            </p>
          )}
        </div>

        <div>
          <label className="block text-sm">Quantity</label>
          <input
            type="number"
            className="border px-3 py-2 w-full"
            {...register("quantity")}
          />
          {errors.quantity && (
            <p className="text-red-600 text-sm">
              {errors.quantity.message}
            </p>
          )}
        </div>

        <button disabled={saving} className="btn">
          {saving ? "Saving…" : "Save"}
        </button>
      </form>
    </div>
  );
}

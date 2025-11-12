import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { api } from "../api/axios";
import { useAuthStore } from "../store/auth";
import { useLocation } from "wouter";
import { toast } from "react-toastify";

const schema = z.object({
  email: z.string().min(1, "Email requerido").email("Email inválido"),
  password: z.string().min(6, "Mínimo 6 caracteres"),
});

export default function Login() {
  const [, navigate] = useLocation();
  const login = useAuthStore((s) => s.login);
  const [submitting, setSubmitting] = useState(false);

  const { register, handleSubmit, formState: { errors } } =
    useForm({ resolver: zodResolver(schema), mode: "onBlur" });

  const onSubmit = async (values) => {
    setSubmitting(true);
    try {
      const promise = api.post("/auth/login", values);
      const { data } = await toast.promise(promise, {
        pending: "Ingresando…",
        success: "¡Bienvenido!",
        error: {
          render({ data: err }) {
            return err?.response?.data?.title || err?.response?.data?.message || "Credenciales inválidas";
          },
        },
      });
      login({ token: data.token, user: data.user, role: data.user?.role || "User" });
      navigate("/");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-sm mx-auto p-6">
      <h1 className="text-xl font-semibold mb-4">Iniciar sesión</h1>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block text-sm">Email</label>
          <input className="border px-3 py-2 w-full" {...register("email")} />
          {errors.email && <p className="text-red-600 text-sm">{errors.email.message}</p>}
        </div>
        <div>
          <label className="block text-sm">Contraseña</label>
          <input type="password" className="border px-3 py-2 w-full" {...register("password")} />
          {errors.password && <p className="text-red-600 text-sm">{errors.password.message}</p>}
        </div>
        <button disabled={submitting} className="bg-blue-600 text-white px-4 py-2 rounded disabled:opacity-50">
          {submitting ? "Ingresando…" : "Ingresar"}
        </button>
      </form>
    </div>
  );
}

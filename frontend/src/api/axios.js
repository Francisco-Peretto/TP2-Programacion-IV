// src/api/axios.js
import axios from "axios";
import { toast } from "react-toastify";

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? "https://localhost:7245/api",

  // ⬇️ MUY IMPORTANTE PARA COOKIES
  withCredentials: true,
});

// --- Interceptor de request ---
api.interceptors.request.use(
  (config) => {
    // ⛔ ya NO enviamos ningún Authorization: Bearer
    // porque el backend usa cookies, no JWT
    return config;
  },
  (error) => Promise.reject(error)
);

// --- Función para extraer errores ---
function extractMessage(error) {
  const d = error?.response?.data;
  if (typeof d === "string") return d;
  if (d?.title) return d.title;
  if (d?.message) return d.message;
  if (Array.isArray(d?.errors)) return d.errors.join("\n");
  if (d?.errors && typeof d.errors === "object") {
    const first = Object.values(d.errors)[0];
    if (Array.isArray(first)) return first[0];
  }
  return error?.message || "Error de red";
}

// --- Interceptor de respuesta ---
api.interceptors.response.use(
  (res) => res,
  (err) => {
    const url = err?.config?.url || "";

    // No mostrar toast en /auth/login para evitar doble alerta
    if (!url.includes("/auth/login")) {
      toast.error(extractMessage(err));
    }

    // Auto logout si tu app lo necesita:
    // if ([401, 403].includes(err?.response?.status)) {
    //   useAuthStore.getState().logout();
    // }

    return Promise.reject(err);
  }
);

import axios from "axios";
import { useAuthStore } from "../store/auth";
import { toast } from "react-toastify";

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5109/api",
  timeout: 12000,
});

api.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().getToken?.();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

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

api.interceptors.response.use(
  (res) => res,
  (err) => {
    const url = err?.config?.url || "";
    if (!url.includes("/auth/login")) {
      toast.error(extractMessage(err));
    }
    // Optionally: auto-logout on 401/403
    // if ([401,403].includes(err?.response?.status)) useAuthStore.getState().logout();
    return Promise.reject(err);
  }
);

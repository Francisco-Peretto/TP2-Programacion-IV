// src/store/auth.js
import { create } from "zustand";
import { persist } from "zustand/middleware";
import { api } from "../api/axios";

export const useAuthStore = create(
  persist(
    (set, get) => ({
      // ya no manejamos token
      user: null,
      role: null,
      isAuthenticated: false,

      // login: marcamos autenticado sin depender de token
      login: ({ user, role }) =>
        set({
          user,
          role,
          isAuthenticated: true,
        }),

      // logout: limpiamos estado y avisamos al back
      logout: async () => {
        try {
          await api.post("/auth/logout");
        } catch {
          // si falla igual limpiamos el estado
        } finally {
          set({ user: null, role: null, isAuthenticated: false });
        }
      },

      // opcional: para hidratar desde /auth/me si lo agregás en el back
      // hydrateFromServer: async () => { ... }
    }),
    { name: "auth-store" }
  )
);

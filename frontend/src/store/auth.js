import { create } from "zustand";
import { persist } from "zustand/middleware";

export const useAuthStore = create(
  persist(
    (set, get) => ({
      token: null,
      user: null,
      role: null,
      isAuthenticated: false,
      login: ({ token, user, role }) => set({ token, user, role, isAuthenticated: !!token }),
      logout: () => set({ token: null, user: null, role: null, isAuthenticated: false }),
      getToken: () => get().token,
    }),
    { name: "auth-store" }
  )
);

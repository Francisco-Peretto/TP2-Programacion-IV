import { create } from "zustand";

const useAuth = create((set) => ({
    user: null,
    token: null,
    isAuthenticated: false,
    login: (user, token) => {
        localStorage.setItem("token", token);
        localStorage.setItem("user", JSON.stringify(user));
        set({ user, token, isAuthenticated: true });
    },
    logout: () => {
        localStorage.clear();
        set({ user: null, token: null, isAuthenticated: false });
    },
}));

export default useAuth;

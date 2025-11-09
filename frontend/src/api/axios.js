import axios from "axios";
import useAuth from "../store/auth";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "http://localhost:5000/api",
});

api.interceptors.request.use((config) => {
    const { token } = useAuth.getState();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export default api;

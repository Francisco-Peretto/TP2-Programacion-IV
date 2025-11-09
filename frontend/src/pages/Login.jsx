import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { loginSchema } from "../schemas/loginSchema";
import api from "../api/axios";
import useAuth from "../store/auth";
import { useLocation } from "wouter";

export default function Login() {
    const { login } = useAuth();
    const [, setLocation] = useLocation();
    const { register, handleSubmit, formState: { errors, isSubmitting } } =
        useForm({ resolver: zodResolver(loginSchema) });

    const onSubmit = async (data) => {
        const res = await api.post("/auth/login", data);
        const { token, user } = res.data;
        login(user, token);
        setLocation("/elementos");
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <input placeholder="Email" {...register("email")} />
            {errors.email && <span>{errors.email.message}</span>}
            <input type="password" placeholder="Password" {...register("password")} />
            {errors.password && <span>{errors.password.message}</span>}
            <button disabled={isSubmitting}>Ingresar</button>
        </form>
    );
}

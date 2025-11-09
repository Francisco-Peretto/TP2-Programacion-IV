import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { courseSchema } from "../schemas/courseSchema";
import api from "../api/axios";
import { useLocation } from "wouter";

export default function ElementForm() {
    const { register, handleSubmit, formState: { errors, isSubmitting } } =
        useForm({ resolver: zodResolver(courseSchema) });
    const [, setLocation] = useLocation();

    const onSubmit = async (data) => {
        await api.post("/courses", data);
        setLocation("/elementos");
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <input placeholder="Título" {...register("title")} />
            {errors.title && <span>{errors.title.message}</span>}
            <textarea placeholder="Descripción" {...register("description")} />
            <input placeholder="Categoría" {...register("category")} />
            <input type="number" step="0.01" placeholder="Precio" {...register("price")} />
            <label>
                <input type="checkbox" {...register("isActive")} /> Activo
            </label>
            <button disabled={isSubmitting}>Guardar</button>
        </form>
    );
}

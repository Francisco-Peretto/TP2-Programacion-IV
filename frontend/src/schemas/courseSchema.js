import { z } from "zod";

export const courseSchema = z.object({
    title: z.string().min(3, "Título demasiado corto"),
    description: z.string().optional(),
    category: z.string().min(2, "Debe tener categoría"),
    price: z.coerce.number().nonnegative("Precio inválido"),
    isActive: z.boolean().default(true),
});

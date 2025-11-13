import React from "react";
import { Link } from "wouter";
//import { useAuthStore } from "../store/auth";

export default function Home() {
  //const { isAuthenticated, role, user, logout } = useAuthStore();

  return (
    <div className="p-6 space-y-3">
      <h1 className="text-xl font-semibold">Inicio</h1>
      <section className="max-w-5xl mx-auto px-6 py-16 text-gray-200">
  <h1 className="text-4xl font-semibold text-blue-400 mb-4">
    Aprendé a programar con una formación moderna y profesional
  </h1>
  <p className="text-lg text-gray-300 mb-8 leading-relaxed">
    En nuestros cursos de programación descubrirás cómo transformar ideas en proyectos reales.
    Aprendé desde los fundamentos hasta las tecnologías más demandadas del mercado — con una metodología
    práctica, acompañamiento personalizado y contenidos actualizados constantemente.
  </p>

  <h2 className="text-2xl font-semibold text-blue-300 mb-3">Nuestros programas abarcan:</h2>
  <ul className="list-disc list-inside space-y-1 text-gray-400 mb-8">
    <li>Desarrollo <strong>Frontend y Backend</strong> con herramientas actuales.</li>
    <li><strong>Bases de datos</strong>, arquitecturas modernas y control de versiones.</li>
    <li><strong>Buenas prácticas</strong>, patrones de diseño y trabajo en equipo.</li>
    <li>Preparación para el mundo laboral y proyectos reales.</li>
  </ul>

  <h2 className="text-2xl font-semibold text-blue-300 mb-2">Tu crecimiento profesional empieza hoy</h2>
  <p className="text-gray-300 leading-relaxed">
    No importa si estás dando tus primeros pasos o buscás perfeccionar tus habilidades:
    nuestros cursos te guiarán para convertirte en un desarrollador seguro, productivo y preparado
    para los desafíos del futuro.
  </p>
</section>
    </div>
  );
}

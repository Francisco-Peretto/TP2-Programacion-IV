import React, { useEffect, useState } from "react";
import { api } from "../api/axios";

export default function AdminPanel() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [metrics, setMetrics] = useState({
    totalCourses: 0,
    totalStudents: 0,
    studentsPerCourse: [],
  });

  const [users, setUsers] = useState([]);
  const [courses, setCourses] = useState([]);

  // enroll form
  const [userId, setUserId] = useState("");
  const [courseId, setCourseId] = useState("");
  const [enrollMsg, setEnrollMsg] = useState("");

  // remove form
  const [removeUserId, setRemoveUserId] = useState("");
  const [removeCourseId, setRemoveCourseId] = useState("");
  const [removeMsg, setRemoveMsg] = useState("");

  // ---- helpers for labels ----
  const getUserId = (u) => u.id ?? u.userId ?? u.userID;
  const getCourseId = (c) => c.id ?? c.courseId ?? c.courseID;

  const getUserLabel = (u) => {
    const id = getUserId(u);
    const name = u.name || u.fullName || u.userName || u.email || "Usuario";
    return `${id} — ${name}`;
  };

  const getCourseLabel = (c) => {
    const id = getCourseId(c);
    const name = c.name || c.title || "Curso";
    return `${id} — ${name}`;
  };

  // preferir solo estudiantes (rol Student), si no hay, usar todos los usuarios
  const studentsOnly = users.filter((u) => {
    const roleName =
      u.role?.name ||
      u.roleName ||
      (Array.isArray(u.roles)
        ? u.roles.map((r) => r.name || r).join(",")
        : u.role || "");
    return (
      typeof roleName === "string" &&
      roleName.toLowerCase().includes("student")
    );
  });

  const selectableUsers = studentsOnly.length ? studentsOnly : users;

  // ---- load metrics + lists ----
  const loadMetrics = async () => {
    setLoading(true);
    setError("");
    try {
      const [{ data: metricsData }, { data: usersData }, { data: coursesData }] =
        await Promise.all([
          api.get("/Admin/metrics"),
          api.get("/User"),
          api.get("/Course"),
        ]);

      setMetrics({
        totalCourses: metricsData.totalCourses ?? 0,
        totalStudents: metricsData.totalStudents ?? 0,
        studentsPerCourse: Array.isArray(metricsData.studentsPerCourse)
          ? metricsData.studentsPerCourse
          : [],
      });

      // users: array or paged
      setUsers(
        Array.isArray(usersData)
          ? usersData
          : usersData.items ?? usersData.data ?? []
      );

      // courses: array or paged
      const extractedCourses = Array.isArray(coursesData)
        ? coursesData
        : coursesData.items ??
          coursesData.data ??
          coursesData.courses ??
          [];
      setCourses(extractedCourses);
    } catch (err) {
      console.error("Error loading admin metrics:", err);
      setError("No se pudieron cargar las métricas.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadMetrics();
  }, []);

  // ---- enroll action ----
  const handleEnroll = async (e) => {
    e.preventDefault();
    setEnrollMsg("");
    const uid = Number(userId);
    const cid = Number(courseId);
    if (!uid || !cid) {
      setEnrollMsg("Selecciona un usuario y un curso.");
      return;
    }
    try {
      await api.post("/Admin/enroll", { userId: uid, courseId: cid });
      setEnrollMsg("Estudiante inscripto correctamente.");
      setUserId("");
      setCourseId("");
      await loadMetrics();
    } catch (err) {
      console.error("Enroll error:", err);
      setEnrollMsg("Error al inscribir. Verifica los datos.");
    }
  };

  // ---- remove action ----
  const handleUnenroll = async (e) => {
    e.preventDefault();
    setRemoveMsg("");
    const uid = Number(removeUserId);
    const cid = Number(removeCourseId);
    if (!uid || !cid) {
      setRemoveMsg("Selecciona un usuario y un curso.");
      return;
    }
    try {
      await api.delete("/Admin/enroll", {
        data: { userId: uid, courseId: cid },
      });
      setRemoveMsg("Estudiante eliminado del curso.");
      setRemoveUserId("");
      setRemoveCourseId("");
      await loadMetrics();
    } catch (err) {
      console.error("Unenroll error:", err);
      setRemoveMsg("Error al eliminar. Verifica los datos.");
    }
  };

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-xl font-semibold">Dashboard</h1>

      {loading && <div>Cargando…</div>}

      {!loading && error && (
        <div className="text-red-400 text-sm">{error}</div>
      )}

      {!loading && !error && (
        <>
          {/* Totales */}
          <div className="grid grid-cols-2 gap-4 max-w-xl">
            <div className="border rounded p-4">
              <div className="text-sm text-gray-500">Total cursos</div>
              <div className="text-2xl font-bold">
                {metrics.totalCourses}
              </div>
            </div>
            <div className="border rounded p-4">
              <div className="text-sm text-gray-500">Total estudiantes</div>
              <div className="text-2xl font-bold">
                {metrics.totalStudents}
              </div>
            </div>
          </div>

          <div className="mt-8 max-w-5xl space-y-6">
            {/* Estudiantes por curso */}
            <div>
              <h2 className="text-lg font-semibold mb-3">
                Estudiantes por curso
              </h2>
              <div className="overflow-x-auto border rounded">
                <table className="min-w-full text-sm">
                  <thead className="bg-gray-900/40">
                    <tr>
                      <th className="px-4 py-2 text-left">Curso</th>
                      <th className="px-4 py-2 text-right">Estudiantes</th>
                    </tr>
                  </thead>
                  <tbody>
                    {metrics.studentsPerCourse.map((c) => (
                      <tr
                        key={c.courseId}
                        className="border-t border-gray-800"
                      >
                        <td className="px-4 py-2">
                          {c.courseName || `Curso #${c.courseId}`}
                        </td>
                        <td className="px-4 py-2 text-right">
                          {c.studentCount}
                        </td>
                      </tr>
                    ))}

                    {metrics.studentsPerCourse.length === 0 && (
                      <tr>
                        <td
                          colSpan={2}
                          className="px-4 py-3 text-center text-gray-400"
                        >
                          No hay cursos para mostrar.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Panel de inscribir / eliminar */}
            <div className="border rounded p-4 space-y-6">
              {/* Inscribir */}
              <div>
                <h2 className="text-lg font-semibold mb-3">
                  Inscribir estudiante en curso
                </h2>
                <form
                  onSubmit={handleEnroll}
                  className="grid gap-3 md:grid-cols-[2fr,2fr,auto]"
                >
                  <div className="flex flex-col gap-1">
                    <label htmlFor="userId">Usuario</label>
                    <select
                      id="userId"
                      className="bg-slate-900 border border-slate-700 rounded px-2 py-1"
                      value={userId}
                      onChange={(e) => setUserId(e.target.value)}
                    >
                      <option value="">Selecciona un usuario</option>
                      {selectableUsers.map((u) => (
                        <option key={getUserId(u)} value={getUserId(u)}>
                          {getUserLabel(u)}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="flex flex-col gap-1">
                    <label htmlFor="courseId">Curso</label>
                    <select
                      id="courseId"
                      className="bg-slate-900 border border-slate-700 rounded px-2 py-1"
                      value={courseId}
                      onChange={(e) => setCourseId(e.target.value)}
                    >
                      <option value="">Selecciona un curso</option>
                      {courses.map((c) => (
                        <option key={getCourseId(c)} value={getCourseId(c)}>
                          {getCourseLabel(c)}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="flex items-end">
                    <button
                      type="submit"
                      className="px-3 py-2 rounded bg-blue-600 hover:bg-blue-500 text-sm font-medium"
                    >
                      Inscribir
                    </button>
                  </div>
                </form>
                {enrollMsg && (
                  <div className="mt-2 text-xs text-gray-300">
                    {enrollMsg}
                  </div>
                )}
              </div>

              {/* Eliminar */}
              <div className="border-t border-slate-800 pt-4">
                <h2 className="text-lg font-semibold mb-3">
                  Eliminar estudiante de curso
                </h2>
                <form
                  onSubmit={handleUnenroll}
                  className="grid gap-3 md:grid-cols-[2fr,2fr,auto]"
                >
                  <div className="flex flex-col gap-1">
                    <label htmlFor="removeUserId">Usuario</label>
                    <select
                      id="removeUserId"
                      className="bg-slate-900 border border-slate-700 rounded px-2 py-1"
                      value={removeUserId}
                      onChange={(e) => setRemoveUserId(e.target.value)}
                    >
                      <option value="">Selecciona un usuario</option>
                      {selectableUsers.map((u) => (
                        <option key={getUserId(u)} value={getUserId(u)}>
                          {getUserLabel(u)}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="flex flex-col gap-1">
                    <label htmlFor="removeCourseId">Curso</label>
                    <select
                      id="removeCourseId"
                      className="bg-slate-900 border border-slate-700 rounded px-2 py-1"
                      value={removeCourseId}
                      onChange={(e) => setRemoveCourseId(e.target.value)}
                    >
                      <option value="">Selecciona un curso</option>
                      {courses.map((c) => (
                        <option key={getCourseId(c)} value={getCourseId(c)}>
                          {getCourseLabel(c)}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="flex items-end">
                    <button
                      type="submit"
                      className="px-3 py-2 rounded bg-red-600 hover:bg-red-500 text-sm font-medium"
                    >
                      Eliminar
                    </button>
                  </div>
                </form>
                {removeMsg && (
                  <div className="mt-2 text-xs text-gray-300">
                    {removeMsg}
                  </div>
                )}
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

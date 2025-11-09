// src/App.jsx
import { Suspense, lazy, useState } from "react";
import { Route, Switch, Redirect, Link } from "wouter";
import Home from "./pages/Home";
import Login from "./pages/Login";
import ElementList from "./pages/ElementList";
import ElementForm from "./pages/ElementForm";
import ProtectedRoute from "./components/ProtectedRoute";
import useAuth from "./store/auth";

const ElementDetail = lazy(() => import("./pages/ElementDetail"));
const AdminPanel = lazy(() => import("./pages/AdminPanel"));

export default function App() {
  const { isAuthenticated, user, logout } = useAuth();
  const [open, setOpen] = useState(false);

  return (
    <div className="app">
      <style>{`
        :root {
          --bg: #0f1115;
          --surface: #151924;
          --muted: #9aa4b2;
          --text: #e6e9ef;
          --brand: #6ea8fe;
          --ok: #3ddc97;
          --danger: #ff6b6b;
          --border: #22283a;
        }
        * { box-sizing: border-box; }
        body, html, #root, .app { height: 100%; margin: 0; }
        body { background: var(--bg); color: var(--text); font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, 'Helvetica Neue', Arial; }
        a { color: inherit; text-decoration: none; }
        .container { max-width: 1100px; margin: 0 auto; padding: 1rem; }
        /* NAV */
        .nav {
          position: sticky; top: 0; z-index: 10;
          background: rgba(15,17,21,.75); backdrop-filter: blur(6px);
          border-bottom: 1px solid var(--border);
        }
        .nav-inner { display: flex; align-items: center; justify-content: space-between; gap: .75rem; padding: .75rem 0; }
        .brand { display: flex; align-items: center; gap: .6rem; font-weight: 700; letter-spacing:.2px; }
        .brand .dot { width: 10px; height: 10px; border-radius: 999px; background: var(--brand); box-shadow: 0 0 14px var(--brand); }
        .menu { display: flex; gap: .75rem; }
        .menu a {
          padding: .5rem .75rem; border-radius: .6rem; color: var(--muted);
        }
        .menu a.active, .menu a:hover { color: var(--text); background: var(--surface); border: 1px solid var(--border); }
        .auth { display: flex; gap: .5rem; align-items:center; }
        .pill { padding: .35rem .6rem; border: 1px solid var(--border); border-radius: 999px; background: var(--surface); color: var(--muted); }
        .btn {
          padding: .45rem .8rem; border-radius: .55rem; border: 1px solid var(--border);
          background: linear-gradient(180deg, #1a2030, #141926); color: var(--text); cursor: pointer;
        }
        .btn:hover { transform: translateY(-1px); }
        .btn-danger { border-color: #2a1212; background: #1a1010; color: #ffb3b3; }
        /* LAYOUT */
        .hero {
          margin: 1.25rem 0 1rem; padding: 1rem; border: 1px solid var(--border);
          border-radius: 1rem; background: radial-gradient(1200px 400px at 0% -10%, rgba(110,168,254,.15), transparent 60%), var(--surface);
        }
        .grid { display: grid; gap: 1rem; grid-template-columns: 1fr; }
        @media (min-width: 900px){ .grid { grid-template-columns: 1fr 320px; } }
        .card {
          background: var(--surface); border: 1px solid var(--border);
          border-radius: 1rem; padding: 1rem;
        }
        .muted { color: var(--muted); }
        .footer { color: var(--muted); font-size: .9rem; text-align: center; padding: 1rem 0 2rem; }
        /* mobile nav */
        .hamburger { display:none; }
        @media (max-width: 700px){
          .menu { display: ${open ? "flex" : "none"}; position: absolute; left: 0; right: 0; top: 56px; background: var(--surface); padding: .5rem; border-bottom:1px solid var(--border); }
          .hamburger { display:block; }
        }
      `}</style>

      {/* Top Nav */}
      <nav className="nav">
        <div className="container nav-inner">
          <Link href="/">
            <a className="brand">
              <span className="dot" />
              <span>Courses App</span>
            </a>
          </Link>

          <button className="btn hamburger" onClick={() => setOpen(v => !v)} aria-label="menu">☰</button>

          <div className="menu" onClick={() => setOpen(false)}>
            <NavLink href="/">Home</NavLink>
            <NavLink href="/elementos">Cursos</NavLink>
            <NavLink href="/admin">Admin</NavLink>
            <NavLink href="/admin/elementos/new">Nuevo</NavLink>
          </div>

          <div className="auth">
            {isAuthenticated ? (
              <>
                <span className="pill">{user?.email} · {user?.role}</span>
                <button className="btn btn-danger" onClick={logout}>Logout</button>
              </>
            ) : (
              <Link href="/login"><a className="btn">Login</a></Link>
            )}
          </div>
        </div>
      </nav>

      {/* Page Shell */}
      <main className="container">
        <section className="hero">
          <div className="grid">
            <div className="card">
              <Suspense fallback={<div className="muted">Cargando…</div>}>
                <Switch>
                  <Route path="/" component={Home} />
                  <Route path="/login" component={Login} />
                  <Route path="/elementos" component={ElementList} />
                  <Route path="/elementos/:id" component={ElementDetail} />
                  <ProtectedRoute path="/admin" roles={["Admin"]}>
                    <AdminPanel />
                  </ProtectedRoute>
                  <ProtectedRoute path="/admin/elementos/new" roles={["Admin"]}>
                    <ElementForm />
                  </ProtectedRoute>
                  <Redirect to="/" />
                </Switch>
              </Suspense>
            </div>

            {/* Sidebar quick actions */}
            <aside className="card">
              <h3 style={{ marginTop: 0 }}>Acciones rápidas</h3>
              <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "grid", gap: ".5rem" }}>
                <li><Link href="/elementos"><a className="btn" style={{ display: "block", textAlign: "center" }}>Ver cursos</a></Link></li>
                <li><Link href="/admin"><a className="btn" style={{ display: "block", textAlign: "center" }}>Panel admin</a></Link></li>
                <li><Link href="/admin/elementos/new"><a className="btn" style={{ display: "block", textAlign: "center" }}>Crear curso</a></Link></li>
              </ul>
              <p className="muted" style={{ marginTop: ".9rem" }}>Usa el menú superior para navegar. Algunas rutas requieren rol <b>Admin</b>.</p>
            </aside>
          </div>
        </section>
      </main>

      <footer className="footer">
        <div className="container">© {new Date().getFullYear()} PGR4 — Dark UI</div>
      </footer>
    </div>
  );
}

/** Minimal link with active styling using wouter */
function NavLink({ href, children }) {
  const isActive = locationHashMatch(href);
  return (
    <Link href={href}>
      <a className={isActive ? "active" : ""}>{children}</a>
    </Link>
  );
}

function locationHashMatch(href) {
  // simple active check against current path
  try {
    const current = window.location.pathname.replace(/\/+$/, "");
    const target = href.replace(/\/+$/, "");
    return current === target;
  } catch {
    return false;
  }
}

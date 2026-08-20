import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import "./Layout.css";

function IconHome() {
  return (
    <svg viewBox="0 0 24 24">
      <path d="M3 11.5 12 4l9 7.5" />
      <path d="M5 10v9a1 1 0 0 0 1 1h4v-6h4v6h4a1 1 0 0 0 1-1v-9" />
    </svg>
  );
}

function IconScan() {
  return (
    <svg viewBox="0 0 24 24">
      <path d="M4 8V5a1 1 0 0 1 1-1h3" />
      <path d="M20 8V5a1 1 0 0 0-1-1h-3" />
      <path d="M4 16v3a1 1 0 0 0 1 1h3" />
      <path d="M20 16v3a1 1 0 0 1-1 1h-3" />
      <path d="M4 12h16" />
    </svg>
  );
}

function IconBox() {
  return (
    <svg viewBox="0 0 24 24">
      <path d="M12 3 3 7.5v9L12 21l9-4.5v-9L12 3Z" />
      <path d="M3 7.5 12 12l9-4.5" />
      <path d="M12 12v9" />
    </svg>
  );
}

function IconActivity() {
  return (
    <svg viewBox="0 0 24 24">
      <path d="M3 12h4l2-7 4 14 2-7h6" />
    </svg>
  );
}

export function Layout() {
  const { email, logout } = useAuth();

  return (
    <div className="layout">
      <header className="layout__topbar">
        <span className="layout__brand">OfficeQR</span>
        <div className="row">
          {email && <span className="muted">{email}</span>}
          <button className="btn btn--secondary" onClick={logout}>
            Çıkış
          </button>
        </div>
      </header>

      <main className="layout__content">
        <Outlet />
      </main>

      <nav className="layout__tabbar">
        <NavLink to="/" end className="layout__tab">
          <IconHome />
          Ana Sayfa
        </NavLink>
        <NavLink to="/scan" className="layout__tab layout__tab--primary">
          <IconScan />
          QR Okut
        </NavLink>
        <NavLink to="/items" className="layout__tab">
          <IconBox />
          Ürünler
        </NavLink>
        <NavLink to="/my-activity" className="layout__tab">
          <IconActivity />
          Aktivitelerim
        </NavLink>
      </nav>
    </div>
  );
}

import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import "./Layout.css";

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
          Ana Sayfa
        </NavLink>
        <NavLink to="/scan" className="layout__tab layout__tab--primary">
          QR Okut
        </NavLink>
        <NavLink to="/items" className="layout__tab">
          Ürünler
        </NavLink>
        <NavLink to="/my-activity" className="layout__tab">
          Aktivitelerim
        </NavLink>
      </nav>
    </div>
  );
}

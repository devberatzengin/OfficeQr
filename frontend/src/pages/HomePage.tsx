import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function HomePage() {
  const { isAdmin } = useAuth();

  return (
    <div className="page stack">
      <div className="card">
        <h2>Hoş geldin</h2>
        <p className="muted">
          Bir dolap, raf veya ürünün QR kodunu okutarak başlayabilir, ya da
          aşağıdan listelere göz atabilirsin.
        </p>
        <Link to="/scan" className="btn btn--primary btn--block">
          QR Okut
        </Link>
      </div>

      <div className="card stack">
        <h3>Listeler</h3>
        <Link to="/cabinets" className="list-item">
          <span className="list-item__title">Dolaplar</span>
          <span className="muted">›</span>
        </Link>
        <Link to="/items" className="list-item">
          <span className="list-item__title">Ürünler</span>
          <span className="muted">›</span>
        </Link>
      </div>

      {isAdmin && (
        <div className="card stack">
          <h3>Yeni Ekle</h3>
          <Link to="/cabinets/new" className="list-item">
            <span className="list-item__title">Yeni Dolap</span>
            <span className="muted">›</span>
          </Link>
          <Link to="/items/new" className="list-item">
            <span className="list-item__title">Yeni Ürün</span>
            <span className="muted">›</span>
          </Link>
        </div>
      )}

      {isAdmin && (
        <div className="card stack">
          <h3>Yönetim</h3>
          <Link to="/admin/users" className="list-item">
            <span className="list-item__title">Kullanıcı Yönetimi</span>
            <span className="muted">›</span>
          </Link>
        </div>
      )}
    </div>
  );
}

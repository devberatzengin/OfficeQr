import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="page center" style={{ minHeight: "60vh" }}>
      <div className="stack" style={{ textAlign: "center" }}>
        <h2>Sayfa bulunamadı</h2>
        <Link to="/" className="btn btn--primary">
          Ana Sayfaya Dön
        </Link>
      </div>
    </div>
  );
}

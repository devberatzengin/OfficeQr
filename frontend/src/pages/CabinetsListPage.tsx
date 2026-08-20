import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { cabinetApi } from "../api/endpoints";
import type { CabinetResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { useAuth } from "../auth/AuthContext";

export function CabinetsListPage() {
  const { isAdmin } = useAuth();
  const [cabinets, setCabinets] = useState<CabinetResponse[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    cabinetApi
      .getAll()
      .then(setCabinets)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Dolaplar yüklenemedi."));
  }, []);

  return (
    <div className="page stack">
      <div className="row row--between">
        <h2>Dolaplar</h2>
        {isAdmin && (
          <Link to="/cabinets/new" className="btn btn--primary">
            + Yeni Dolap
          </Link>
        )}
      </div>

      {error && <ErrorBanner message={error} />}
      {!cabinets && !error && <LoadingSpinner />}

      {cabinets && cabinets.length === 0 && (
        <p className="muted">Henüz dolap eklenmemiş.</p>
      )}

      {cabinets && cabinets.length > 0 && (
        <ul className="list">
          {cabinets.map((cabinet) => (
            <li key={cabinet.id}>
              <Link to={`/cabinets/${cabinet.id}`} className="list-item">
                <span className="list-item__title">
                  Dolap {cabinet.id.slice(0, 8)}
                </span>
                <span className="list-item__meta">Kapasite: {cabinet.capacity}</span>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

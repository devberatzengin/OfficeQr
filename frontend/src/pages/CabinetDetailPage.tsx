import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { cabinetApi, shelfApi } from "../api/endpoints";
import type { CabinetResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { useAuth } from "../auth/AuthContext";

interface ShelfWithCount {
  id: string;
  capacity: number;
  itemCount: number;
}

export function CabinetDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { isAdmin } = useAuth();
  const [cabinet, setCabinet] = useState<CabinetResponse | null>(null);
  const [shelvesWithCounts, setShelvesWithCounts] = useState<ShelfWithCount[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setCabinet(null);
    setShelvesWithCounts(null);
    setError(null);

    cabinetApi
      .getById(id)
      .then(setCabinet)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Dolap yüklenemedi."));

    cabinetApi
      .getShelves(id)
      .then(async (shelves) => {
        // Cabinet/shelves endpoint'i raf başına ürün sayısı dönmüyor, o yüzden
        // her raf için ayrıca items sorguluyoruz (raf sayısı küçük olduğundan sorun değil).
        const counts = await Promise.all(
          shelves.map(async (shelf) => ({
            id: shelf.id,
            capacity: shelf.capacity,
            itemCount: (await shelfApi.getItems(shelf.id)).length,
          })),
        );
        setShelvesWithCounts(counts);
      })
      .catch((err) => setError(err instanceof ApiError ? err.message : "Raflar yüklenemedi."));
  }, [id]);

  if (error) return <div className="page"><ErrorBanner message={error} /></div>;
  if (!cabinet) return <LoadingSpinner />;

  return (
    <div className="page stack">
      <div className="card">
        <h2>Dolap {cabinet.id.slice(0, 8)}</h2>
        <img src={cabinet.qrCode} alt="Dolap QR kodu" style={{ width: 160, alignSelf: "center" }} />
        <p className="muted">Boş raf kapasitesi: {cabinet.capacity}</p>
        {isAdmin && (
          <Link to={`/shelves/new?cabinetId=${cabinet.id}`} className="btn btn--primary btn--block">
            + Bu Dolaba Raf Ekle
          </Link>
        )}
      </div>

      <div className="card stack">
        <h3>Raflar</h3>
        {!shelvesWithCounts && <LoadingSpinner />}
        {shelvesWithCounts?.length === 0 && <p className="muted">Bu dolapta henüz raf yok.</p>}
        {shelvesWithCounts?.map((shelf) => (
          <Link key={shelf.id} to={`/shelves/${shelf.id}`} className="list-item">
            <span className="list-item__title">Raf {shelf.id.slice(0, 8)}</span>
            <span className="list-item__meta">
              {shelf.itemCount} ürün · kapasite {shelf.capacity}
            </span>
          </Link>
        ))}
      </div>
    </div>
  );
}

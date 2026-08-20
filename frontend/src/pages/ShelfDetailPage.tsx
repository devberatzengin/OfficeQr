import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { shelfApi } from "../api/endpoints";
import type { ItemResponse, ShelfResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { ScanPicker } from "../components/ScanPicker";
import { STATUS_LABELS, STATUS_BADGE_CLASS } from "../utils/itemStatus";
import { useAuth } from "../auth/AuthContext";

export function ShelfDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [shelf, setShelf] = useState<ShelfResponse | null>(null);
  const [items, setItems] = useState<ItemResponse[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isMoving, setIsMoving] = useState(false);

  function load() {
    if (!id) return;
    setShelf(null);
    setItems(null);
    setError(null);
    Promise.all([shelfApi.getById(id), shelfApi.getItems(id)])
      .then(([shelfData, itemsData]) => {
        setShelf(shelfData);
        setItems(itemsData);
      })
      .catch((err) => setError(err instanceof ApiError ? err.message : "Raf yüklenemedi."));
  }

  useEffect(load, [id]);

  async function handleMoveToCabinet(newCabinetId: string) {
    if (!id) return;
    setIsMoving(true);
    setError(null);
    try {
      await shelfApi.moveToCabinet(id, newCabinetId);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Raf taşınamadı.");
    } finally {
      setIsMoving(false);
    }
  }

  if (error && !shelf) return <div className="page"><ErrorBanner message={error} /></div>;
  if (!shelf) return <LoadingSpinner />;

  return (
    <div className="page stack">
      <div className="card stack">
        <h2>Raf {shelf.id.slice(0, 8)}</h2>
        <img src={shelf.qrCode} alt="Raf QR kodu" style={{ width: 160, alignSelf: "center" }} />
        <p className="muted">Kapasite: {shelf.capacity}</p>
        <Link to={`/cabinets/${shelf.cabinetId}`} className="muted">
          Bağlı olduğu dolaba git ›
        </Link>
        {isAdmin && (
          <button
            className="btn btn--primary btn--block"
            onClick={() => navigate(`/items/new?shelfId=${shelf.id}`)}
          >
            + Bu Rafa Ürün Ekle
          </button>
        )}
      </div>

      <div className="card stack">
        <h3>Raftaki Ürünler</h3>
        {items && items.length === 0 && <p className="muted">Bu rafta ürün yok.</p>}
        {items?.map((item) => (
          <Link key={item.id} to={`/items/${item.id}`} className="list-item">
            <span className="list-item__title">{item.name}</span>
            <span className={`badge ${STATUS_BADGE_CLASS[item.status]}`}>
              {STATUS_LABELS[item.status]}
            </span>
          </Link>
        ))}
        {!items && <LoadingSpinner />}
      </div>

      {isAdmin && (
        <div className="card stack">
          <h3>Rafı Başka Dolaba Taşı</h3>
          {error && <ErrorBanner message={error} />}
          <ScanPicker
            expectedType="cabinet"
            label="Hedef Dolap QR'ını Okut"
            onPick={handleMoveToCabinet}
          />
          {isMoving && <LoadingSpinner />}
        </div>
      )}
    </div>
  );
}

import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { itemApi, shelfApi } from "../api/endpoints";
import type { ItemMovementEntry, ItemResponse, ShelfResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner, NotImplementedBanner } from "../components/ErrorBanner";
import { ScanPicker } from "../components/ScanPicker";
import { describeGroup, formatOccurredAt, groupMovementEntries } from "../utils/movementLabels";
import { STATUS_LABELS, STATUS_BADGE_CLASS } from "../utils/itemStatus";
import { useAuth } from "../auth/AuthContext";
import { useUserNames } from "../hooks/useUserNames";

export function ItemDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAdmin, userId } = useAuth();

  const [item, setItem] = useState<ItemResponse | null>(null);
  const [shelf, setShelf] = useState<ShelfResponse | null>(null);
  const [history, setHistory] = useState<ItemMovementEntry[] | "unavailable" | null>(null);
  const movementGroups = Array.isArray(history) ? groupMovementEntries(history) : [];
  const actorNames = useUserNames(movementGroups.map((group) => group.first.actorUserId));
  const [error, setError] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [isBusy, setIsBusy] = useState(false);
  const [isReturning, setIsReturning] = useState(false);
  const [isMoving, setIsMoving] = useState(false);

  function load() {
    if (!id) return;
    setItem(null);
    setShelf(null);
    setHistory(null);
    setError(null);

    itemApi
      .getById(id)
      .then(async (data) => {
        setItem(data);
        if (data.shelfId) {
          shelfApi.getById(data.shelfId).then(setShelf).catch(() => setShelf(null));
        }
      })
      .catch((err) => setError(err instanceof ApiError ? err.message : "Ürün yüklenemedi."));

    itemApi
      .getHistory(id)
      .then(setHistory)
      .catch((err) => setHistory(err instanceof ApiError && err.notImplemented ? "unavailable" : []));
  }

  useEffect(load, [id]);

  async function handlePickup() {
    if (!id) return;
    setIsBusy(true);
    setActionError(null);
    try {
      await itemApi.pickup(id);
      load();
    } catch (err) {
      setActionError(err instanceof ApiError ? err.message : "İşlem başarısız oldu.");
    } finally {
      setIsBusy(false);
    }
  }

  async function handleReturn(targetShelfId: string) {
    if (!id) return;
    setIsBusy(true);
    setActionError(null);
    try {
      await itemApi.returnItem(id, targetShelfId);
      setIsReturning(false);
      load();
    } catch (err) {
      setActionError(err instanceof ApiError ? err.message : "İade işlemi başarısız oldu.");
    } finally {
      setIsBusy(false);
    }
  }

  async function handleMove(targetShelfId: string) {
    if (!id) return;
    setIsBusy(true);
    setActionError(null);
    try {
      await itemApi.moveToShelf(id, targetShelfId);
      setIsMoving(false);
      load();
    } catch (err) {
      setActionError(err instanceof ApiError ? err.message : "Taşıma işlemi başarısız oldu.");
    } finally {
      setIsBusy(false);
    }
  }

  async function handleDelete() {
    if (!id) return;
    if (!confirm("Bu ürünü silmek istediğine emin misin?")) return;
    setIsBusy(true);
    try {
      await itemApi.remove(id);
      navigate("/items", { replace: true });
    } catch (err) {
      setActionError(err instanceof ApiError ? err.message : "Ürün silinemedi.");
      setIsBusy(false);
    }
  }

  if (error) return <div className="page"><ErrorBanner message={error} /></div>;
  if (!item) return <LoadingSpinner />;

  const isAvailable = item.status === "Available";
  const isInUse = item.status === "InUse";

  return (
    <div className="page stack">
      <div className="card stack">
        <div className="row row--between">
          <h2>{item.name}</h2>
          <span className={`badge ${STATUS_BADGE_CLASS[item.status]}`}>
            {STATUS_LABELS[item.status]}
          </span>
        </div>

        <img src={item.qrCode} alt="Ürün QR kodu" style={{ width: 160, alignSelf: "center" }} />

        <div className="stack" style={{ gap: 4 }}>
          {shelf ? (
            <Link to={`/shelves/${shelf.id}`} className="muted">
              Raf {shelf.id.slice(0, 8)} · Dolap {shelf.cabinetId.slice(0, 8)} ›
            </Link>
          ) : (
            <span className="muted">Herhangi bir rafta değil.</span>
          )}
        </div>
      </div>

      {actionError && <ErrorBanner message={actionError} />}

      <div className="card stack">
        <h3>İşlemler</h3>

        {isAvailable && (
          <button className="btn btn--primary btn--block" onClick={handlePickup} disabled={isBusy}>
            Teslim Al
          </button>
        )}

        {isInUse && !isReturning && (
          <button
            className="btn btn--primary btn--block"
            onClick={() => setIsReturning(true)}
            disabled={isBusy}
          >
            İade Et
          </button>
        )}
        {isReturning && (
          <div className="stack">
            <p className="muted">İade edilecek rafın QR'ını okut.</p>
            <ScanPicker expectedType="shelf" label="Raf QR'ını Okut" onPick={handleReturn} />
          </div>
        )}

        {isAdmin && !isMoving && (
          <button
            className="btn btn--secondary btn--block"
            onClick={() => setIsMoving(true)}
            disabled={isBusy}
          >
            Başka Rafa Taşı
          </button>
        )}
        {isAdmin && isMoving && (
          <div className="stack">
            <p className="muted">Taşınacak rafın QR'ını okut.</p>
            <ScanPicker expectedType="shelf" label="Raf QR'ını Okut" onPick={handleMove} />
          </div>
        )}

        {isAdmin && (
          <button className="btn btn--danger btn--block" onClick={handleDelete} disabled={isBusy}>
            Ürünü Sil
          </button>
        )}
      </div>

      <div className="card stack">
        <h3>Hareket Geçmişi</h3>
        {history === null && <LoadingSpinner />}
        {history === "unavailable" && <NotImplementedBanner feature="Hareket geçmişi" />}
        {Array.isArray(history) && history.length === 0 && (
          <p className="muted">Henüz hareket kaydı yok.</p>
        )}
        {Array.isArray(history) &&
          movementGroups.map((group, i) => {
            const actorId = group.first.actorUserId;
            const actorLabel = !actorId
              ? "Bilinmiyor"
              : actorId === userId
                ? "Sen"
                : (actorNames[actorId] ?? "…");

            return (
              <div key={i} className="list-item" style={{ cursor: "default" }}>
                <span className="list-item__title">{describeGroup(group, actorLabel)}</span>
                <span className="list-item__meta">{formatOccurredAt(group.occurredAt)}</span>
              </div>
            );
          })}
      </div>
    </div>
  );
}

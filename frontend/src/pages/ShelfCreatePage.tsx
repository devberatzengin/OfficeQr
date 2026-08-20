import { useState, type FormEvent } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { shelfApi } from "../api/endpoints";
import { ApiError } from "../api/client";
import { ErrorBanner } from "../components/ErrorBanner";
import { ScanPicker } from "../components/ScanPicker";
import { useAuth } from "../auth/AuthContext";

export function ShelfCreatePage() {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [searchParams] = useSearchParams();
  const [cabinetId, setCabinetId] = useState(searchParams.get("cabinetId") ?? "");
  const [capacity, setCapacity] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (!isAdmin) {
    return (
      <div className="page">
        <div className="card">
          <p className="muted">Bu işlem için yönetici yetkisi gerekiyor.</p>
        </div>
      </div>
    );
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      const shelf = await shelfApi.create(capacity, cabinetId);
      navigate(`/shelves/${shelf.id}`, { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Raf oluşturulamadı.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="page">
      <div className="card">
        <h2>Yeni Raf</h2>
        <form onSubmit={handleSubmit} className="stack">
          <div className="field">
            <label htmlFor="cabinetId">Dolap ID</label>
            <input
              id="cabinetId"
              value={cabinetId}
              onChange={(e) => setCabinetId(e.target.value)}
              placeholder="Dolap QR'ını okutabilir ya da ID'yi yapıştırabilirsin"
              required
            />
            <ScanPicker expectedType="cabinet" label="Dolap QR'ını Okut" onPick={setCabinetId} />
          </div>
          <div className="field">
            <label htmlFor="capacity">Ürün kapasitesi</label>
            <input
              id="capacity"
              type="number"
              min={1}
              max={32767}
              value={capacity}
              onChange={(e) => setCapacity(Number(e.target.value))}
              required
            />
          </div>
          {error && <ErrorBanner message={error} />}
          <button className="btn btn--primary btn--block" disabled={isSubmitting}>
            {isSubmitting ? "Oluşturuluyor..." : "Rafı Oluştur"}
          </button>
        </form>
      </div>
    </div>
  );
}

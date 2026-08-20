import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { cabinetApi } from "../api/endpoints";
import { ApiError } from "../api/client";
import { ErrorBanner } from "../components/ErrorBanner";
import { useAuth } from "../auth/AuthContext";

export function CabinetCreatePage() {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
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
      const cabinet = await cabinetApi.create(capacity);
      navigate(`/cabinets/${cabinet.id}`, { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Dolap oluşturulamadı.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="page">
      <div className="card">
        <h2>Yeni Dolap</h2>
        <form onSubmit={handleSubmit} className="stack">
          <div className="field">
            <label htmlFor="capacity">Raf kapasitesi (bu dolaba kaç raf sığar)</label>
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
            {isSubmitting ? "Oluşturuluyor..." : "Dolabı Oluştur"}
          </button>
        </form>
      </div>
    </div>
  );
}

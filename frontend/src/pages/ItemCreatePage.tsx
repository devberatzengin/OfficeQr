import { useState, type FormEvent } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { itemApi } from "../api/endpoints";
import { ApiError } from "../api/client";
import { ErrorBanner } from "../components/ErrorBanner";
import { ScanPicker } from "../components/ScanPicker";
import { useAuth } from "../auth/AuthContext";

export function ItemCreatePage() {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [searchParams] = useSearchParams();
  const [name, setName] = useState("");
  const [shelfId, setShelfId] = useState(searchParams.get("shelfId") ?? "");
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
      const item = await itemApi.create(name, shelfId);
      navigate(`/items/${item.id}`, { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Ürün oluşturulamadı.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="page">
      <div className="card">
        <h2>Yeni Ürün</h2>
        <form onSubmit={handleSubmit} className="stack">
          <div className="field">
            <label htmlFor="name">Ürün adı</label>
            <input
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              minLength={2}
              maxLength={255}
              required
            />
          </div>
          <div className="field">
            <label htmlFor="shelfId">Raf ID</label>
            <input
              id="shelfId"
              value={shelfId}
              onChange={(e) => setShelfId(e.target.value)}
              placeholder="Raf QR'ını okutabilir ya da ID'yi yapıştırabilirsin"
              required
            />
            <ScanPicker expectedType="shelf" label="Raf QR'ını Okut" onPick={setShelfId} />
          </div>
          {error && <ErrorBanner message={error} />}
          <button className="btn btn--primary btn--block" disabled={isSubmitting}>
            {isSubmitting ? "Oluşturuluyor..." : "Ürünü Oluştur"}
          </button>
        </form>
      </div>
    </div>
  );
}

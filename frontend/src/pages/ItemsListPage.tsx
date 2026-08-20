import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { itemApi } from "../api/endpoints";
import type { PagedResponse, ItemResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { STATUS_LABELS, STATUS_BADGE_CLASS } from "../utils/itemStatus";

export function ItemsListPage() {
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [result, setResult] = useState<PagedResponse<ItemResponse> | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setResult(null);
    setError(null);
    itemApi
      .getAll({ page, pageSize: 10, search: search || undefined })
      .then(setResult)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Ürünler yüklenemedi."));
  }, [page, search]);

  function handleSearchSubmit(event: React.FormEvent) {
    event.preventDefault();
    setPage(1);
    setSearch(searchInput.trim());
  }

  return (
    <div className="page stack">
      <div className="row row--between">
        <h2>Ürünler</h2>
        <Link to="/items/new" className="btn btn--primary">
          + Yeni Ürün
        </Link>
      </div>

      <form onSubmit={handleSearchSubmit} className="row">
        <input
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          placeholder="Ürün adına göre ara..."
          style={{ flex: 1 }}
        />
        <button type="submit" className="btn btn--secondary">
          Ara
        </button>
      </form>

      {error && <ErrorBanner message={error} />}
      {!result && !error && <LoadingSpinner />}
      {result && result.items.length === 0 && <p className="muted">Sonuç bulunamadı.</p>}

      {result && result.items.length > 0 && (
        <ul className="list">
          {result.items.map((item) => (
            <li key={item.id}>
              <Link to={`/items/${item.id}`} className="list-item">
                <span className="list-item__title">{item.name}</span>
                <span className={`badge ${STATUS_BADGE_CLASS[item.status]}`}>
                  {STATUS_LABELS[item.status]}
                </span>
              </Link>
            </li>
          ))}
        </ul>
      )}

      {result && result.totalPages > 1 && (
        <div className="row row--between">
          <button
            type="button"
            className="btn btn--secondary"
            disabled={page <= 1}
            onClick={() => setPage((p) => p - 1)}
          >
            ‹ Önceki
          </button>
          <span className="muted">
            Sayfa {result.page} / {result.totalPages} ({result.totalCount} ürün)
          </span>
          <button
            type="button"
            className="btn btn--secondary"
            disabled={page >= result.totalPages}
            onClick={() => setPage((p) => p + 1)}
          >
            Sonraki ›
          </button>
        </div>
      )}
    </div>
  );
}

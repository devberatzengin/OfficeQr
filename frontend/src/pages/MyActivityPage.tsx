import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { itemApi } from "../api/endpoints";
import type { MyActivityEntry } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { describeGroup, formatOccurredAt, groupMovementEntries } from "../utils/movementLabels";

export function MyActivityPage() {
  const [activities, setActivities] = useState<MyActivityEntry[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const groups = activities
    ? groupMovementEntries(activities, (entry) => `${entry.itemId}|${entry.occurredAt}`)
    : [];

  useEffect(() => {
    itemApi
      .getMyActivity()
      .then(setActivities)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Aktiviteler yüklenemedi."));
  }, []);

  return (
    <div className="page stack">
      <h2>Aktivitelerim</h2>
      <p className="muted">Kendi yaptığın işlemler, en yeniden eskiye.</p>

      {error && <ErrorBanner message={error} />}
      {!activities && !error && <LoadingSpinner />}
      {activities && activities.length === 0 && (
        <p className="muted">Henüz bir aktiviten yok.</p>
      )}

      {activities && activities.length > 0 && (
        <ul className="list">
          {groups.map((group, i) => (
            <li key={i}>
              <Link to={`/items/${group.first.itemId}`} className="list-item">
                <span className="list-item__title">{group.first.itemName}</span>
                <span className="list-item__meta">
                  {describeGroup(group, "Sen")} · {formatOccurredAt(group.occurredAt)}
                </span>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

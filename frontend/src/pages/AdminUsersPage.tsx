import { useEffect, useState } from "react";
import { usersApi } from "../api/endpoints";
import type { AdminUserResponse } from "../api/types";
import { ApiError } from "../api/client";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorBanner } from "../components/ErrorBanner";
import { useAuth } from "../auth/AuthContext";

// Hangi kullanıcı üzerinde hangi işlem sürüyor (satır bazlı loading/disable için).
type PendingAction = "delete" | "deactivate" | "activate" | null;

export function AdminUsersPage() {
  const { isAdmin, userId: currentUserId } = useAuth();
  const [users, setUsers] = useState<AdminUserResponse[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [pending, setPending] = useState<Record<string, PendingAction>>({});

  useEffect(() => {
    if (!isAdmin) return;
    loadUsers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin]);

  function loadUsers() {
    setError(null);
    usersApi
      .getAll()
      .then(setUsers)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Kullanıcılar yüklenemedi."));
  }

  async function runAction(id: string, action: PendingAction, run: () => Promise<AdminUserResponse>) {
    setError(null);
    setPending((prev) => ({ ...prev, [id]: action }));
    try {
      const updated = await run();
      setUsers((prev) => prev?.map((u) => (u.id === id ? updated : u)) ?? prev);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "İşlem gerçekleştirilemedi.");
    } finally {
      setPending((prev) => ({ ...prev, [id]: null }));
    }
  }

  function handleDelete(user: AdminUserResponse) {
    const confirmed = window.confirm(
      `${user.email} kullanıcısını silmek istediğine emin misin? Bu işlem geri alınamaz.`,
    );
    if (!confirmed) return;

    runAction(user.id, "delete", async () => {
      await usersApi.remove(user.id);
      // Silinen kullanıcı listeden düşer.
      setUsers((prev) => prev?.filter((u) => u.id !== user.id) ?? prev);
      return { ...user, isActive: false };
    });
  }

  function handleDeactivate(user: AdminUserResponse) {
    runAction(user.id, "deactivate", () => usersApi.deactivate(user.id));
  }

  function handleActivate(user: AdminUserResponse) {
    runAction(user.id, "activate", () => usersApi.activate(user.id));
  }

  if (!isAdmin) {
    return (
      <div className="page">
        <div className="card">
          <p className="muted">Bu ekran için yönetici yetkisi gerekiyor.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="page stack">
      <div className="row row--between">
        <h2>Kullanıcı Yönetimi</h2>
      </div>

      {error && <ErrorBanner message={error} />}
      {!users && !error && <LoadingSpinner />}

      {users && users.length === 0 && <p className="muted">Kayıtlı kullanıcı yok.</p>}

      {users && users.length > 0 && (
        <ul className="list">
          {users.map((user) => {
            const isSelf = user.id === currentUserId;
            const action = pending[user.id] ?? null;
            const isBusy = action !== null;

            return (
              <li key={user.id} className="card stack">
                <div className="row row--between">
                  <div className="stack" style={{ gap: 4 }}>
                    <span className="list-item__title">{user.email}</span>
                    <div className="row">
                      <span className={`badge ${user.isActive ? "badge--info" : "badge--warning"}`}>
                        {user.isActive ? "Aktif" : "Pasif"}
                      </span>
                      {user.roles.includes("Admin") && (
                        <span className="badge badge--danger">Admin</span>
                      )}
                      {isSelf && <span className="muted">(sen)</span>}
                    </div>
                  </div>
                </div>

                <div className="row">
                  {user.isActive ? (
                    <button
                      className="btn btn--secondary"
                      disabled={isSelf || isBusy}
                      onClick={() => handleDeactivate(user)}
                    >
                      {action === "deactivate" ? "Pasife alınıyor..." : "Pasife Al"}
                    </button>
                  ) : (
                    <button
                      className="btn btn--secondary"
                      disabled={isBusy}
                      onClick={() => handleActivate(user)}
                    >
                      {action === "activate" ? "Aktifleştiriliyor..." : "Aktifleştir"}
                    </button>
                  )}

                  <button
                    className="btn btn--danger"
                    disabled={isSelf || isBusy}
                    onClick={() => handleDelete(user)}
                  >
                    {action === "delete" ? "Siliniyor..." : "Sil"}
                  </button>
                </div>

                {isSelf && (
                  <p className="muted">Kendi hesabını buradan silemez ya da pasife alamazsın.</p>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}

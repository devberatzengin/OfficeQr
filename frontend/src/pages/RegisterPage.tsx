import { useState, type FormEvent } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { authApi } from "../api/endpoints";
import { ApiError } from "../api/client";
import { ErrorBanner } from "../components/ErrorBanner";

export function RegisterPage() {
  const { isAuthenticated, login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (isAuthenticated) return <Navigate to="/" replace />;

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await authApi.register(email, password);
      // Kayıt sonrası aynı bilgilerle otomatik giriş yapıp ana sayfaya yönlendiriyoruz.
      await login(email, password);
      navigate("/", { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Kayıt başarısız oldu.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="page center" style={{ minHeight: "100%" }}>
      <div className="card" style={{ width: "100%", maxWidth: 380 }}>
        <h2>Hesap Oluştur</h2>
        <form onSubmit={handleSubmit} className="stack">
          <div className="field">
            <label htmlFor="email">E-posta</label>
            <input
              id="email"
              type="email"
              autoComplete="username"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="field">
            <label htmlFor="password">Şifre</label>
            <input
              id="password"
              type="password"
              autoComplete="new-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          {error && <ErrorBanner message={error} />}
          <button className="btn btn--primary btn--block" disabled={isSubmitting}>
            {isSubmitting ? "Kaydediliyor..." : "Kayıt Ol"}
          </button>
        </form>
        <p className="muted" style={{ textAlign: "center", marginBottom: 0 }}>
          Zaten hesabın var mı? <Link to="/login">Giriş yap</Link>
        </p>
      </div>
    </div>
  );
}

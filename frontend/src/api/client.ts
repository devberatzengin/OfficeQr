import type { ProblemDetails } from "./types";

// Boş bırakılırsa Vite dev server'daki proxy (vite.config.ts) kullanılır —
// böylece backend'de CORS ayarlanmasına gerek kalmaz. Frontend'i Vite dışında
// (ör. statik build olarak) sunacaksan .env'de tam URL vermen gerekir, o
// durumda backend'de CORS'un da açık olması gerekir (bkz. BACKEND_TODO.md).
const API_BASE_URL = import.meta.env.VITE_API_URL ?? "";
const TOKEN_STORAGE_KEY = "officeqr_access_token";

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setStoredToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN_STORAGE_KEY, token);
  else localStorage.removeItem(TOKEN_STORAGE_KEY);
}

export class ApiError extends Error {
  status: number;
  // Backend'de endpoint henüz eklenmediyse (404) ekranlar bunu ayırt edip
  // "bu özellik backend'de henüz yok" mesajı gösterebilsin diye.
  notImplemented: boolean;
  // Validasyon hatalarında (400) alan bazlı mesajlar — ValidationProblemDetails.errors.
  fieldErrors?: Record<string, string[]>;

  constructor(
    message: string,
    status: number,
    notImplemented: boolean = false,
    fieldErrors?: Record<string, string[]>,
  ) {
    super(message);
    this.status = status;
    this.notImplemented = notImplemented;
    this.fieldErrors = fieldErrors;
  }
}

async function request<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const token = getStoredToken();
  const headers = new Headers(options.headers);
  headers.set("Content-Type", "application/json");
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const res = await fetch(`${API_BASE_URL}${path}`, { ...options, headers });

  if (res.status === 204) return undefined as T;

  const isJson = res.headers.get("content-type")?.includes("application/json");
  const body = isJson ? await res.json() : undefined;

  if (!res.ok) {
    const problem = body as ProblemDetails | undefined;
    // Validasyon hatalarında errors dolu geliyor — alan mesajlarını birleştirip
    // tek bir okunur mesaj üretiyoruz (ekranlar isterse problem.errors'a da bakabilir).
    const fieldMessages = problem?.errors
      ? Object.values(problem.errors).flat().join(" ")
      : undefined;
    // 403: ASP.NET Core [Authorize(Roles=...)] varsayılan olarak boş body
    // döner (body === undefined), bu route'un eşleşmediği anlamına gelmez —
    // sadece yetkin yetersiz demektir.
    const friendlyMessage =
      res.status === 403
        ? problem?.detail || problem?.title || "Bu işlem için yönetici yetkisi gerekiyor."
        : fieldMessages || problem?.detail || problem?.title || `İstek başarısız oldu (${res.status})`;

    // Dev modunda mesajın altına ham teknik detayı ekliyoruz (hangi endpoint,
    // hangi status, backend'den ne döndü) — `npm run build` ile production'a
    // alınca bu blok otomatik kaybolur, sadece friendlyMessage kalır.
    const message = import.meta.env.DEV
      ? `${friendlyMessage}\n\n[DEV] ${options.method ?? "GET"} ${path} → ${res.status} ${res.statusText}\n${
          body
            ? JSON.stringify(body, null, 2)
            : res.status === 403
              ? "(403: boş body — ASP.NET Core'un rol yetkisi reddi için varsayılan davranışı, routing sorunu değil)"
              : "(response body boş ya da JSON değil — muhtemelen hiçbir controller bu route'u karşılamadı)"
        }`
      : friendlyMessage;

    throw new ApiError(message, res.status, res.status === 404, problem?.errors);
  }

  return body as T;
}

export const api = {
  get: <T>(path: string) => request<T>(path, { method: "GET" }),
  post: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "POST", body: body ? JSON.stringify(body) : undefined }),
  put: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "PUT", body: body ? JSON.stringify(body) : undefined }),
  delete: <T>(path: string) => request<T>(path, { method: "DELETE" }),
};

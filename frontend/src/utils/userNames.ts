import { usersApi } from "../api/endpoints";

// Modül seviyesinde cache — aynı kullanıcı ID'si sayfa/gezinme boyunca
// tekrar tekrar sorgulanmasın diye.
const cache = new Map<string, string>();
const inFlight = new Map<string, Promise<string>>();

function emailLocalPart(email: string): string {
  return email.split("@")[0];
}

export function resolveUserName(userId: string): Promise<string> {
  const cached = cache.get(userId);
  if (cached) return Promise.resolve(cached);

  const existing = inFlight.get(userId);
  if (existing) return existing;

  const promise = usersApi
    .getById(userId)
    .then((user) => {
      const name = emailLocalPart(user.email);
      cache.set(userId, name);
      return name;
    })
    .catch(() => userId.slice(0, 8))
    .finally(() => inFlight.delete(userId));

  inFlight.set(userId, promise);
  return promise;
}

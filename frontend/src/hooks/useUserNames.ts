import { useEffect, useState } from "react";
import { resolveUserName } from "../utils/userNames";

// Verilen kullanıcı ID'lerini e-posta kullanıcı adına çözer, ID -> isim
// map'i döner. Zaten çözülmüş ID'ler tekrar sorgulanmaz.
export function useUserNames(userIds: Array<string | null | undefined>): Record<string, string> {
  const key = Array.from(new Set(userIds.filter((id): id is string => !!id))).join(",");
  const [names, setNames] = useState<Record<string, string>>({});

  useEffect(() => {
    const uniqueIds = key ? key.split(",") : [];
    const missing = uniqueIds.filter((id) => !(id in names));
    if (missing.length === 0) return;

    let cancelled = false;
    Promise.all(missing.map(async (id) => [id, await resolveUserName(id)] as const)).then(
      (entries) => {
        if (cancelled) return;
        setNames((prev) => ({ ...prev, ...Object.fromEntries(entries) }));
      },
    );
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [key]);

  return names;
}

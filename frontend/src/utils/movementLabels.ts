interface BaseMovementEntry {
  action: string;
  type: "Shelf" | "User";
  phase: "Opened" | "Closed";
  shelfId: string | null;
  occurredAt: string;
}

export interface MovementGroup<T extends BaseMovementEntry> {
  occurredAt: string;
  reason: string;
  oldShelfId: string | null;
  newShelfId: string | null;
  first: T;
}

// Backend tek bir işlem sırasında (Pickup/Return/Move/...) aynı anda birden
// fazla history satırı yazabiliyor — ör. "İade Et" raf kapanışı + raf
// açılışı + kullanıcı kapanışı olmak üzere 3 satır üretebiliyor, hepsi aynı
// `now` değerini taşıyor (bkz. ItemService.MoveItemAsync). Bunları tek bir
// mantıksal olay olarak gruplayıp ekranda tek satır gösteriyoruz.
export function groupMovementEntries<T extends BaseMovementEntry>(
  entries: T[],
  groupKey: (entry: T) => string = (e) => e.occurredAt,
): MovementGroup<T>[] {
  const map = new Map<string, T[]>();
  for (const entry of entries) {
    const key = groupKey(entry);
    const list = map.get(key) ?? [];
    list.push(entry);
    map.set(key, list);
  }

  return Array.from(map.values()).map((group) => {
    const shelfClosed = group.find((e) => e.type === "Shelf" && e.phase === "Closed");
    const shelfOpened = group.find((e) => e.type === "Shelf" && e.phase === "Opened");
    const first = group[0];

    return {
      occurredAt: first.occurredAt,
      reason: first.action,
      oldShelfId: shelfClosed?.shelfId ?? null,
      newShelfId: shelfOpened?.shelfId ?? null,
      first,
    };
  });
}

function shelfLabel(shelfId: string | null): string | null {
  return shelfId ? `Raf ${shelfId.slice(0, 8)}` : null;
}

export function describeGroup<T extends BaseMovementEntry>(
  group: MovementGroup<T>,
  actorLabel: string,
): string {
  const oldShelf = shelfLabel(group.oldShelfId);
  const newShelf = shelfLabel(group.newShelfId);

  switch (group.reason) {
    case "Created":
      return newShelf
        ? `${actorLabel} tarafından oluşturulup ${newShelf}'a yerleştirildi`
        : `${actorLabel} tarafından oluşturuldu`;
    case "PickedUp":
      return `${actorLabel} tarafından teslim alındı`;
    case "Returned":
      return newShelf
        ? `${actorLabel} tarafından ${newShelf}'a iade edildi`
        : `${actorLabel} tarafından iade edildi`;
    case "Moved":
      if (oldShelf && newShelf) return `${actorLabel} tarafından ${oldShelf}'dan ${newShelf}'e taşındı`;
      if (newShelf) return `${actorLabel} tarafından ${newShelf}'e taşındı`;
      return `${actorLabel} tarafından taşındı`;
    case "Updated":
      return newShelf
        ? `${actorLabel} tarafından ${newShelf}'a güncellendi`
        : `${actorLabel} tarafından güncellendi`;
    case "Removed":
      return `${actorLabel} tarafından sistemden kaldırıldı`;
    default:
      return `${actorLabel} tarafından ${group.reason}`;
  }
}

export function formatOccurredAt(occurredAt: string): string {
  return new Date(occurredAt).toLocaleString("tr-TR");
}

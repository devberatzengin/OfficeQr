export type QrEntityType = "cabinet" | "shelf" | "item";

export interface ParsedQr {
  type: QrEntityType;
  id: string;
}

// Backend'in QrCodeGenerator.cs'i artık "WMS:{TYPE}:{Id}" formatı üretiyor
// (ör. "WMS:ITEM:7d282733-...", "WMS:CABINET:...", "WMS:SHELF:..."). Eski
// "{ClassName}:{Id}" formatını da (ör. "Item:7d282733-...") tanımaya devam
// ediyoruz ki daha önce basılmış QR etiketleri geçersiz olmasın.
const PATTERNS: { regex: RegExp; type: QrEntityType }[] = [
  { regex: /^Cabinet:(.+)$/i, type: "cabinet" },
  { regex: /^Shelf:(.+)$/i, type: "shelf" },
  { regex: /^Item:(.+)$/i, type: "item" },
  { regex: /^WMS:CABINET:(.+)$/i, type: "cabinet" },
  { regex: /^WMS:SHELF:(.+)$/i, type: "shelf" },
  { regex: /^WMS:ITEM:(.+)$/i, type: "item" },
  { regex: /^WMS:PRODUCT:(.+)$/i, type: "item" },
];

export function parseQr(rawText: string): ParsedQr | null {
  const text = rawText.trim();
  for (const { regex, type } of PATTERNS) {
    const match = text.match(regex);
    if (match) return { type, id: match[1] };
  }
  return null;
}

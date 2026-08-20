export type QrEntityType = "cabinet" | "shelf" | "item";

export interface ParsedQr {
  type: QrEntityType;
  id: string;
}

// Backend'in QrCodeGenerator.cs'i şu an "{ClassName}:{Id}" formatı üretiyor
// (ör. "Item:7d282733-..."). Spesifikasyon "WMS:PRODUCT:{publicId}" formatını
// istiyor (bkz. BACKEND_TODO.md). İkisini de tanıyoruz ki backend formatı
// değişince frontend'de hiçbir şey değişmesin.
const PATTERNS: { regex: RegExp; type: QrEntityType }[] = [
  { regex: /^Cabinet:(.+)$/i, type: "cabinet" },
  { regex: /^Shelf:(.+)$/i, type: "shelf" },
  { regex: /^Item:(.+)$/i, type: "item" },
  { regex: /^WMS:CABINET:(.+)$/i, type: "cabinet" },
  { regex: /^WMS:SHELF:(.+)$/i, type: "shelf" },
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

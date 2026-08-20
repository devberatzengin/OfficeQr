import { useCallback, useState } from "react";
import { QrScanner } from "./QrScanner";
import { parseQr, type QrEntityType } from "../qr/parseQr";

interface ScanPickerProps {
  expectedType: QrEntityType;
  label: string;
  onPick: (id: string) => void;
}

const TYPE_LABEL: Record<QrEntityType, string> = {
  cabinet: "dolap",
  shelf: "raf",
  item: "ürün",
};

// "X QR'ı okut" butonu: tıklanınca kamerayı açar, doğru tipte bir QR
// okununca id'yi yukarı bildirir, yanlış tipte QR okunursa uyarır.
export function ScanPicker({ expectedType, label, onPick }: ScanPickerProps) {
  const [isScanning, setIsScanning] = useState(false);
  const [mismatchError, setMismatchError] = useState<string | null>(null);

  const handleScan = useCallback(
    (text: string) => {
      const parsed = parseQr(text);
      if (!parsed) {
        setMismatchError("Bu bir OfficeQR kodu değil.");
        return;
      }
      if (parsed.type !== expectedType) {
        setMismatchError(
          `Bir ${TYPE_LABEL[expectedType]} QR'ı bekleniyordu, ${TYPE_LABEL[parsed.type]} QR'ı okundu.`,
        );
        return;
      }
      setMismatchError(null);
      setIsScanning(false);
      onPick(parsed.id);
    },
    [expectedType, onPick],
  );

  if (!isScanning) {
    return (
      <button type="button" className="btn btn--secondary" onClick={() => setIsScanning(true)}>
        {label}
      </button>
    );
  }

  return (
    <div className="stack">
      <QrScanner active onScan={handleScan} />
      {mismatchError && <div className="error-banner">{mismatchError}</div>}
      <button type="button" className="btn btn--secondary" onClick={() => setIsScanning(false)}>
        Vazgeç
      </button>
    </div>
  );
}

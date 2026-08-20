import { useCallback, useState } from "react";
import { useNavigate } from "react-router-dom";
import { QrScanner } from "../components/QrScanner";
import { parseQr } from "../qr/parseQr";

const ROUTE_BY_TYPE = {
  cabinet: "/cabinets",
  shelf: "/shelves",
  item: "/items",
} as const;

export function ScanPage() {
  const navigate = useNavigate();
  const [unknownCode, setUnknownCode] = useState<string | null>(null);

  const handleScan = useCallback(
    (text: string) => {
      const parsed = parseQr(text);
      if (!parsed) {
        setUnknownCode(text);
        return;
      }
      setUnknownCode(null);
      navigate(`${ROUTE_BY_TYPE[parsed.type]}/${parsed.id}`);
    },
    [navigate],
  );

  return (
    <div className="page stack">
      <div className="card stack">
        <h2>QR Okut</h2>
        <p className="muted">
          Bir dolap, raf veya ürün QR kodunu kameraya göster.
        </p>
        <QrScanner active onScan={handleScan} />
        {unknownCode && (
          <div className="error-banner">
            Bu QR bir OfficeQR kodu değil: "{unknownCode}"
          </div>
        )}
      </div>
    </div>
  );
}

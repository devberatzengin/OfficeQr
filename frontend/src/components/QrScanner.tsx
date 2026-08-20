import { useEffect, useRef, useState } from "react";
import { BrowserQRCodeReader, type IScannerControls } from "@zxing/browser";
import "./QrScanner.css";

interface QrScannerProps {
  onScan: (text: string) => void;
  active: boolean;
}

// Kamerayla sürekli QR okuyan bileşen. Bir kod bulunduğunda `onScan` çağrılır;
// aynı karede tekrar tekrar tetiklenmesin diye kısa bir "cooldown" uyguluyoruz.
export function QrScanner({ onScan, active }: QrScannerProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const controlsRef = useRef<IScannerControls | null>(null);
  const lastScanRef = useRef<{ text: string; at: number }>({ text: "", at: 0 });
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!active) return;

    let cancelled = false;
    let stream: MediaStream | null = null;
    let stallTimer: ReturnType<typeof setTimeout> | null = null;
    const codeReader = new BrowserQRCodeReader();

    function fail(err: unknown, hint = "") {
      if (cancelled) return;
      // err her zaman DOMException olmuyor (zxing kendi hata tiplerini de
      // fırlatabiliyor), o yüzden instanceof yerine duck-typing ile
      // name/message okuyoruz ki gerçek hata ekranda görünsün.
      const name = (err as { name?: string })?.name ?? "UnknownError";
      const message = (err as { message?: string })?.message ?? String(err);
      setError(`${hint} [${name}] ${message}`.trim());
    }

    async function start() {
      const video = videoRef.current!;

      // Cihazları etiketlerine göre (ör. "back"/"rear") seçmek yerine
      // doğrudan facingMode:environment istiyoruz — izin verilmeden önce
      // tarayıcılar (özellikle iOS Safari) cihaz etiketlerini boş döndürür.
      try {
        stream = await navigator.mediaDevices.getUserMedia({
          video: { facingMode: { ideal: "environment" } },
        });
      } catch (err) {
        const name = (err as { name?: string })?.name;
        if (name === "NotAllowedError") {
          fail(err, "Kamera izni reddedildi — tarayıcı ayarlarından bu site için kamera iznini aç.");
        } else if (name === "NotFoundError" || name === "OverconstrainedError") {
          fail(err, "Uygun kamera bulunamadı.");
        } else if (!window.isSecureContext) {
          fail(err, "Kamera yalnızca https (ya da localhost) bağlantıda çalışır.");
        } else {
          fail(err);
        }
        return;
      }

      if (cancelled) {
        stream.getTracks().forEach((t) => t.stop());
        return;
      }

      video.srcObject = stream;
      try {
        await video.play();
      } catch (err) {
        fail(err, "Kamera görüntüsü oynatılamadı.");
        return;
      }

      // Bazı cihazlarda izin verilip stream bağlanıyor ama hiç kare
      // gelmiyor (ekran simsiyah kalıyor) — bunu ayırt edebilmek için
      // birkaç saniye içinde gerçekten kare gelip gelmediğini kontrol ediyoruz.
      stallTimer = setTimeout(() => {
        if (!cancelled && video.readyState < video.HAVE_CURRENT_DATA) {
          setError(
            "Kameradan görüntü gelmiyor (izin verildi ama akış başlamadı). Sayfayı yenilemeyi veya başka bir tarayıcı denemeyi dene.",
          );
        }
      }, 4000);

      try {
        const controls = await codeReader.decodeFromStream(stream, video, (result) => {
          if (!result) return;
          const text = result.getText();
          const now = Date.now();
          const last = lastScanRef.current;
          if (text === last.text && now - last.at < 2500) return;
          lastScanRef.current = { text, at: now };
          onScan(text);
        });

        if (cancelled) {
          controls.stop();
          return;
        }
        controlsRef.current = controls;
      } catch (err) {
        fail(err, "Tarama başlatılamadı.");
      }
    }

    start();

    return () => {
      cancelled = true;
      if (stallTimer) clearTimeout(stallTimer);
      controlsRef.current?.stop();
      controlsRef.current = null;
      stream?.getTracks().forEach((t) => t.stop());
    };
  }, [active, onScan]);

  if (!active) return null;

  return (
    <div className="qr-scanner">
      <video ref={videoRef} className="qr-scanner__video" muted playsInline />
      <div className="qr-scanner__frame" />
      {error && <p className="qr-scanner__error">{error}</p>}
    </div>
  );
}

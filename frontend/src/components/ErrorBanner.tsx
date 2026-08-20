export function ErrorBanner({ message }: { message: string }) {
  // Dev modunda api/client.ts mesajın sonuna "\n\n[DEV] ..." bloğu ekliyor —
  // varsa onu ayrı, monospace bir kutuda gösteriyoruz ki okunması kolay olsun.
  const devSplitIndex = message.indexOf("\n\n[DEV]");
  if (devSplitIndex === -1) {
    return <div className="error-banner">{message}</div>;
  }

  const friendly = message.slice(0, devSplitIndex);
  const devDetail = message.slice(devSplitIndex + 2);

  return (
    <div className="error-banner">
      <div>{friendly}</div>
      <pre className="error-banner__dev">{devDetail}</pre>
    </div>
  );
}

// Backend'de henüz olmayan bir endpoint çağrıldığında (404) gösterilir.
export function NotImplementedBanner({ feature }: { feature: string }) {
  return (
    <div className="info-banner">
      "{feature}" özelliği backend'de henüz eklenmedi. Bkz. BACKEND_TODO.md
    </div>
  );
}

# OfficeQr

QR kod tabanlı ofis depo/envanter yönetim sistemi. Dolap → Raf → Ürün hiyerarşisiyle ofis malzemelerinin nerede olduğunu, kimde olduğunu ve hareket geçmişini takip etmek için geliştirildi.

```
Dolap A
├── Raf A1
│   ├── HDMI Kablosu
│   ├── Mouse
│   └── Adaptör
└── Raf A2
    ├── Tornavida Seti
    └── Multimetre
```

## Teknolojiler

**Backend:** .NET 10 · ASP.NET Core Web API · Entity Framework Core · PostgreSQL (Npgsql) · FluentValidation · AutoMapper · QRCoder · ASP.NET Core Identity (rol tabanlı yetkilendirme, Bearer token)

**Frontend:** React 19 · TypeScript · Vite · React Router · `@zxing/browser` (kamera ile QR okuma) · responsive, mobil öncelikli tasarım

**Altyapı:** Docker · Docker Compose

## Özellikler

- **Dolap / Raf / Ürün** hiyerarşisi, her biri için QR kod üretimi
- **QR okutma akışları** — dolap okutunca rafları ve raf başına ürün sayısını, raf okutunca bağlı dolabı ve içindeki ürünleri, ürün okutunca durumunu ve konumunu gösterir
- **Ürün durumu** (`Available` / `InUse` / `Maintenance` / `Lost` / `Disposed`) — durum geçişleri iş kuralı olarak servis katmanında zorunlu kılınır (ör. `Available` olmayan bir ürün teslim alınamaz)
- **Teslim al / iade et / rafa taşı** akışları, her biri raf kapasitesini ve kullanıcı atamasını günceller
- **Hareket geçmişi** — her ürün için kim, ne zaman, hangi rafa/kullanıcıya, hangi sebeple hareket etti; ayrıca "Aktivitelerim" ile kendi işlemlerinin özeti
- **Rol tabanlı yetkilendirme** — `Admin` (dolap/raf/ürün oluşturma, güncelleme, silme, taşıma), `User` (görüntüleme, teslim alma, iade etme); frontend rolü `GET /api/auth/me` ile öğrenip admin olmayan kullanıcılarda ilgili butonları gizler
- **RFC 7807 (ProblemDetails)** formatında tutarlı hata yanıtları

## Mimari notları

- **Repository + Unit of Work** deseni — her entity için tipli repository, tek bir `IUnitOfWork` üzerinden erişilir
- **Raf/dolap kapasitesi** "kalan slot sayacı" olarak modellenmiştir (sabit maksimum değil), çocuk eklenince azalır, çıkınca artar
- **Hareket geçmişi** "aç/kapa" deseniyle tutulur (`ItemShelfHistory`, `ItemUserHistory`) — her satırın bir açılış (`PlacedAt`/`AssignedAt`) ve bir kapanış (`RemovedAt`/`ReturnedAt`) anı olabilir, aktör ve sebep bilgisiyle birlikte
- Çoklu adımlı işlemler (taşıma, teslim alma, iade) tek bir `SaveChangesAsync` çağrısında toplanır — EF Core bunu zaten tek DB transaction'ı içinde yürüttüğü için ara adımda hata olursa yarım veri kalmaz

## Hızlı Başlangıç

### Docker ile (backend + veritabanı)

```bash
docker compose up
```

Bu komut PostgreSQL'i ve backend API'yi (`http://localhost:5000`) tek seferde ayağa kaldırır, migration'ları otomatik uygular.

### Yerel geliştirme (backend + frontend ayrı ayrı)

Frontend geliştirirken backend'i Docker yerine doğrudan çalıştırmak daha pratik (Vite'ın dev proxy'si `localhost:5188`'i bekliyor):

```bash
# 1) Veritabanı
docker compose up -d postgres

# 2) Backend (http://localhost:5188)
cd OfficeQr
dotnet run --launch-profile http

# 3) Frontend (https://localhost:5173 — kamera erişimi için self-signed HTTPS)
cd frontend
npm install
npm run dev
```

İlk açılışta backend, `appsettings.Development.json`'daki `Seed:AdminEmail` / `Seed:AdminPassword` ile bir geliştirme Admin hesabı oluşturur (varsayılan: `admin@officeqr.local`). Diğer kullanıcılar `/api/auth/register` ile `User` rolünde kaydolur.

## Proje yapısı

```
OfficeQr/           # .NET backend (Controllers, Services, Entities, Dtos, Data, Migrations, ...)
frontend/            # React + TS + Vite frontend
docs/                # Ek dokümantasyon (AutoMapper rehberi, veritabanı şeması)
docker-compose.yaml  # Postgres + backend
Dockerfile           # Backend container tanımı
```

## Bilinen sınırlamalar / yapılacaklar

- QR içeriği şu an `{ClassName}:{Id}` formatında (ör. `Item:guid`) — hedef `WMS:PRODUCT:{publicId}` formatına henüz geçilmedi.
- Optimistic concurrency (aynı ürünün eşzamanlı teslim alınması) üzerinde çalışılıyor.
- xUnit test projesi henüz eklenmedi.

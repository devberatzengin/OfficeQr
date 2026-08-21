// Backend DTO şekilleriyle birebir eşleşen tipler.
// (OfficeQr/Dtos/**/Response.cs, CreateRequest.cs, UpdateRequest.cs dosyalarına bak)

export interface CabinetResponse {
  id: string;
  qrCode: string;
  capacity: number;
}

export interface ShelfResponse {
  id: string;
  qrCode: string;
  capacity: number;
  cabinetId: string;
}

export interface ItemResponse {
  id: string;
  qrCode: string;
  name: string;
  status: ItemStatus;
  userId: string | null;
  shelfId: string | null;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface LoginResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

// OfficeQr/Dtos/Auth/MeResponse.cs ile birebir eşleşiyor.
export interface MeResponse {
  id: string;
  email: string;
  isAdmin: boolean;
}

// OfficeQr/Dtos/User/UserResponse.cs ile birebir eşleşiyor.
export interface UserSummary {
  id: string;
  email: string;
}

// OfficeQr/Dtos/User/UserResponse.cs'in tam hali — admin kullanıcı yönetimi
// ekranında kullanılıyor (UserSummary sadece id/email taşıyor).
export interface AdminUserResponse {
  id: string;
  email: string;
  isActive: boolean;
  createdOn: string;
  roles: string[];
}

// Backend artık RFC 7807 "Problem Details" formatında hata dönüyor
// (bkz. OfficeQr/Middleware/GlobalExceptionHandler.cs). Validasyon hatalarında
// `errors` alanı da doluyor (alan adı -> hata mesajları).
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

// OfficeQr/Entity/Enums/ItemStatus.cs ile birebir eşleşiyor.
export type ItemStatus =
  | "Available"
  | "InUse"
  | "Maintenance"
  | "Lost"
  | "Disposed";

// OfficeQr/Dtos/Item/HistoryEntryResponse.cs ile birebir eşleşiyor.
// Bir satır (ItemShelfHistory/ItemUserHistory) artık iki ayrı olay üretebilir:
// açıldığında (Phase: "Opened") ve kapandığında (Phase: "Closed") — ör. bir
// "Teslim Al" + "İade Et" çifti iki ayrı olay olarak görünür.
export interface ItemMovementEntry {
  action: string;
  type: "Shelf" | "User";
  phase: "Opened" | "Closed";
  shelfId: string | null;
  userId: string | null;
  actorUserId: string | null;
  occurredAt: string;
}

// OfficeQr/Dtos/Item/MyActivityEntryResponse.cs ile birebir eşleşiyor.
// ItemMovementEntry'den farkı: tek bir item'a değil, tüm item'lar genelinde
// AKTÖRÜ ben olan satırlara bakıyor — o yüzden hangi item olduğunu da taşıyor.
export interface MyActivityEntry {
  itemId: string;
  itemName: string;
  action: string;
  type: "Shelf" | "User";
  phase: "Opened" | "Closed";
  shelfId: string | null;
  occurredAt: string;
}

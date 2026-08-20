import { api } from "./client";
import type {
  CabinetResponse,
  ItemMovementEntry,
  ItemResponse,
  LoginResponse,
  MeResponse,
  MyActivityEntry,
  PagedResponse,
  ShelfResponse,
  UserSummary,
} from "./types";

export const authApi = {
  login: (email: string, password: string) =>
    api.post<LoginResponse>("/api/auth/login", { email, password }),
  register: (email: string, password: string) =>
    api.post<{ success: boolean; email: string; message: string }>(
      "/api/auth/register",
      { email, password },
    ),
  logout: () => api.post<void>("/api/auth/logout"),
  me: () => api.get<MeResponse>("/api/auth/me"),
};

export const usersApi = {
  getById: (id: string) => api.get<UserSummary>(`/api/Users/${id}`),
};

export const cabinetApi = {
  getAll: () => api.get<CabinetResponse[]>("/api/Cabinet"),
  getById: (id: string) => api.get<CabinetResponse>(`/api/Cabinet/${id}`),
  create: (capacity: number) =>
    api.post<CabinetResponse>("/api/Cabinet", { capacity }),
  update: (id: string, capacity: number) =>
    api.put<CabinetResponse>("/api/Cabinet", { id, capacity }),
  remove: (id: string) =>
    api.delete<boolean>(`/api/Cabinet?cabinetId=${id}`),
  getShelves: (id: string) => api.get<ShelfResponse[]>(`/api/Cabinet/${id}/shelves`),
};

export const shelfApi = {
  getById: (id: string) => api.get<ShelfResponse>(`/api/Shelf/${id}`),
  create: (capacity: number, cabinetId: string) =>
    api.post<ShelfResponse>("/api/Shelf", { capacity, cabinetId }),
  update: (id: string, capacity: number) =>
    api.put<ShelfResponse>("/api/Shelf", { id, capacity }),
  remove: (id: string) => api.delete<boolean>(`/api/Shelf/${id}`),
  getItems: (id: string) => api.get<ItemResponse[]>(`/api/Shelf/${id}/items`),
  moveToCabinet: (id: string, cabinetId: string) =>
    api.put<ShelfResponse>(`/api/Shelf/${id}/cabinet`, { cabinetId }),
};

export interface ItemListParams {
  page?: number;
  pageSize?: number;
  search?: string;
}

export const itemApi = {
  getAll: (params: ItemListParams = {}) => {
    const query = new URLSearchParams();
    if (params.page) query.set("page", String(params.page));
    if (params.pageSize) query.set("pageSize", String(params.pageSize));
    if (params.search) query.set("search", params.search);
    const qs = query.toString();
    return api.get<PagedResponse<ItemResponse>>(`/api/Item${qs ? `?${qs}` : ""}`);
  },
  getById: (id: string) => api.get<ItemResponse>(`/api/Item/${id}`),
  create: (name: string, shelfId: string) =>
    api.post<ItemResponse>("/api/Item", { name, shelfId }),
  update: (id: string, changes: { name?: string; userId?: string; shelfId?: string }) =>
    api.put<ItemResponse>("/api/Item", { id, ...changes }),
  remove: (id: string) => api.delete<boolean>(`/api/Item/${id}`),
  moveToShelf: (id: string, shelfId: string, userId?: string) =>
    api.put<ItemResponse>(`/api/Item/${id}/shelf`, { shelfId, userId }),

  pickup: (id: string) => api.post<ItemResponse>(`/api/Item/${id}/pickup`),
  returnItem: (id: string, shelfId: string) =>
    api.post<ItemResponse>(`/api/Item/${id}/return`, { shelfId }),

  getHistory: (id: string) =>
    api.get<ItemMovementEntry[]>(`/api/Item/${id}/history`),
  getMyActivity: () => api.get<MyActivityEntry[]>("/api/Item/my-activity"),
};

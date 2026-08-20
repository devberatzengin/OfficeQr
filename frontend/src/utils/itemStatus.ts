import type { ItemStatus } from "../api/types";

export const STATUS_LABELS: Record<ItemStatus, string> = {
  Available: "Boşta",
  InUse: "Teslim edilmiş",
  Maintenance: "Bakımda",
  Lost: "Kayıp",
  Disposed: "İmha edilmiş",
};

export const STATUS_BADGE_CLASS: Record<ItemStatus, string> = {
  Available: "badge--info",
  InUse: "badge--warning",
  Maintenance: "badge--warning",
  Lost: "badge--danger",
  Disposed: "badge--danger",
};

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;
  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }

  const text = await res.text();
  return text ? JSON.parse(text) : ({} as T);
}

// ─── Types ────────────────────────────────────────────────────────────────────
export interface VideoResponse {
  id: string; title: string; sourceUrl: string; status: string;
  durationSeconds?: number; thumbnailUrl?: string; createdAtUtc: string;
}
export interface ShortClipResponse {
  id: string; videoId: string; title: string; description?: string;
  startTime: number; endTime: number; outputUri?: string;
  viralityScore?: number; hashtags?: string; status: string; createdAtUtc: string;
}
export interface ScheduleResponse {
  id: string; shortClipId: string; socialAccountId: string;
  platform: string; publishAtUtc: string; status: string;
  externalPostId?: string; failureReason?: string;
}
export interface SubscriptionPlanResponse {
  id: string; tier: number; name: string; description?: string;
  monthlyPrice: number; yearlyPrice: number; features?: string;
  monthlyVideoLimit: number;
  // Fallbacks for UI compatibility
  monthlyPriceUsd?: number; yearlyPriceUsd?: number; maxVideosPerMonth?: number; maxShortsPerVideo?: number;
}
export interface PaymentResponse {
  id: string; provider: string; providerPaymentId: string;
  amount: number; currency: string; status: string; createdAtUtc: string;
}
export interface UserUsageResponse {
  limitExceeded?: boolean;
  currentPlanName?: string;
  currentTier?: number | string;
  videosUsedThisMonth?: number;
  monthlyVideoLimit?: number;
  // Fallbacks
  videosThisMonth?: number;
  shortsThisMonth?: number;
  maxVideosPerMonth?: number;
  maxShortsPerVideo?: number;
  subscriptionTier?: string;
}
export interface AdminUserDto {
  id: string; email: string; displayName?: string; role: string;
  subscriptionTier: string; isEmailVerified: boolean;
  lastLoginAtUtc?: string; createdAtUtc: string;
}
export interface AdminSubscriptionDto {
  id: string; userEmail: string; userDisplayName?: string;
  provider: string; providerPaymentId: string;
  amount: number; currency: string; status: string; createdAtUtc: string;
}
export interface AdminRevenueResponse {
  totalRevenue: number; monthlyRevenue: number; weeklyRevenue: number;
  totalTransactions: number;
  byProvider: { provider: string; total: number; count: number }[];
  last30Days: { date: string; amount: number }[];
  userTiers: { tier: string; count: number }[];
}
export interface AdminSystemStatsResponse {
  totalUsers: number; totalVideos: number; totalShorts: number;
  totalSchedules: number; totalPayments: number; totalSocial: number;
  newUsersToday: number;
  videosByStatus: { status: string; count: number }[];
  schedulesByStatus: { status: string; count: number }[];
  serverTime: string;
}
export interface AdminAuditLogDto {
  id: string; userId?: string; action: string; entityName?: string;
  entityId?: string; metadataJson?: string; createdAtUtc: string;
}
export interface AdminAnalyticsResponse {
  activeUsersLast7Days: number;
  userGrowthLast30Days: { date: string; count: number }[];
  videoFunnel: { status: string; count: number }[];
  publishingSuccessRate: { status: string; count: number }[];
  platformBreakdown: { platform: string; count: number }[];
  socialBreakdown: { platform: string; count: number }[];
}

// ─── API ──────────────────────────────────────────────────────────────────────
export const api = {
  auth: {
    login: (body: { email: string; password: string }) =>
      request<any>("/api/v1/auth/login", { method: "POST", body: JSON.stringify(body) }),
    register: (body: { email: string; password: string; displayName: string }) =>
      request<any>("/api/v1/auth/register", { method: "POST", body: JSON.stringify(body) }),
    me: () => request<any>("/api/v1/auth/me"),
    verifyEmail: (token: string) =>
      request<any>("/api/v1/auth/verify-email", { method: "POST", body: JSON.stringify({ token }) }),
    forgotPassword: (email: string) =>
      request<any>("/api/v1/auth/forgot-password", { method: "POST", body: JSON.stringify({ email }) }),
    resetPassword: (body: { token: string; newPassword: string }) =>
      request<any>("/api/v1/auth/reset-password", { method: "POST", body: JSON.stringify(body) }),
    refresh: (refreshToken: string) =>
      request<any>("/api/v1/auth/refresh", { method: "POST", body: JSON.stringify({ refreshToken }) }),
  },
  videos: {
    list: () => request<VideoResponse[]>("/api/v1/videos"),
    getById: (id: string) => request<VideoResponse>(`/api/v1/videos/${id}`),
    delete: (id: string) => request<void>(`/api/v1/videos/${id}`, { method: "DELETE" }),
    getShorts: (videoId: string) => request<ShortClipResponse[]>(`/api/v1/videos/${videoId}/shorts`),
    deleteShort: (clipId: string) => request<void>(`/api/v1/videos/shorts/${clipId}`, { method: "DELETE" }),
    downloadShortUrl: (clipId: string) => `${API_BASE}/api/v1/videos/shorts/${clipId}/download`,
    downloadVideoUrl: (videoId: string) => `${API_BASE}/api/v1/videos/${videoId}/download`,
    submitYouTube: (body: { sourceUrl: string; title: string }) =>
      request<VideoResponse>("/api/v1/videos/youtube-import", { method: "POST", body: JSON.stringify(body) }),
    uploadFile: async (file: File, title: string): Promise<VideoResponse> => {
      const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;
      const form = new FormData();
      form.append("file", file);
      if (title) form.append("title", title);
      const res = await fetch(`${API_BASE}/api/v1/videos/upload`, {
        method: "POST",
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        body: form,
      });
      if (!res.ok) { const t = await res.text(); throw new Error(t || `HTTP ${res.status}`); }
      return res.json();
    },
    triggerProcess: (videoId: string) =>
      request<any>(`/api/v1/videos/${videoId}/process`, { method: "POST", body: JSON.stringify({}) }),
    stopProcess: (videoId: string) =>
      request<any>(`/api/v1/videos/${videoId}/stop`, { method: "POST" }),
    downloadWithProgress: (
      url: string,
      fileName: string,
      onProgress: (percent: number, loadedStr: string, totalStr: string) => void
    ): Promise<void> => {
      return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open("GET", url, true);
        const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;
        if (token) {
          xhr.setRequestHeader("Authorization", `Bearer ${token}`);
        }
        xhr.responseType = "blob";

        xhr.onprogress = (event) => {
          if (event.lengthComputable && event.total > 0) {
            const percent = Math.min(100, Math.round((event.loaded / event.total) * 100));
            const loadedMb = (event.loaded / (1024 * 1024)).toFixed(1);
            const totalMb = (event.total / (1024 * 1024)).toFixed(1);
            onProgress(percent, `${loadedMb} MB`, `${totalMb} MB`);
          } else {
            const loadedMb = (event.loaded / (1024 * 1024)).toFixed(1);
            onProgress(-1, `${loadedMb} MB`, "Unknown");
          }
        };

        xhr.onload = () => {
          if (xhr.status >= 200 && xhr.status < 300) {
            const blob = xhr.response;
            const blobUrl = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = blobUrl;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            setTimeout(() => window.URL.revokeObjectURL(blobUrl), 2000);
            onProgress(100, "", "");
            resolve();
          } else {
            reject(new Error(`Download failed (HTTP ${xhr.status})`));
          }
        };

        xhr.onerror = () => reject(new Error("Network error during download"));
        xhr.send();
      });
    },
    uploadFileWithProgress: (
      file: File,
      title: string,
      onProgress: (percent: number, loadedStr: string, totalStr: string) => void
    ): Promise<VideoResponse> => {
      return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", `${API_BASE}/api/v1/videos/upload`, true);
        const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;
        if (token) {
          xhr.setRequestHeader("Authorization", `Bearer ${token}`);
        }
        const form = new FormData();
        form.append("file", file);
        if (title) form.append("title", title);

        xhr.upload.onprogress = (event) => {
          if (event.lengthComputable && event.total > 0) {
            const percent = Math.min(100, Math.round((event.loaded / event.total) * 100));
            const loadedMb = (event.loaded / (1024 * 1024)).toFixed(1);
            const totalMb = (event.total / (1024 * 1024)).toFixed(1);
            onProgress(percent, `${loadedMb} MB`, `${totalMb} MB`);
          } else {
            const loadedMb = (event.loaded / (1024 * 1024)).toFixed(1);
            onProgress(-1, `${loadedMb} MB`, "Unknown");
          }
        };

        xhr.onload = () => {
          if (xhr.status >= 200 && xhr.status < 300) {
            try {
              const res = JSON.parse(xhr.responseText);
              resolve(res);
            } catch {
              reject(new Error("Invalid JSON response"));
            }
          } else {
            reject(new Error(xhr.responseText || `HTTP ${xhr.status}`));
          }
        };

        xhr.onerror = () => reject(new Error("Network error during upload"));
        xhr.send(form);
      });
    },
  },
  schedules: {
    list: () => request<ScheduleResponse[]>("/api/v1/schedules"),
    create: (body: { shortClipId: string; publishAtUtc: string; platform: number; socialAccountId: string }) =>
      request<ScheduleResponse>("/api/v1/schedules", { method: "POST", body: JSON.stringify(body) }),
    update: (id: string, body: { publishAtUtc: string }) =>
      request<ScheduleResponse>(`/api/v1/schedules/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    delete: (id: string) => request<void>(`/api/v1/schedules/${id}`, { method: "DELETE" }),
  },
  payments: {
    getPlans: () => request<SubscriptionPlanResponse[]>("/api/v1/payments/plans"),
    getHistory: () => request<PaymentResponse[]>("/api/v1/payments/history"),
    getUsage: () => request<UserUsageResponse>("/api/v1/payments/usage"),
    createCheckout: (body: { planId: string; provider: string; billingCycle: string; successUrl: string; cancelUrl: string }) =>
      request<{ paymentUrl?: string }>("/api/v1/payments/checkout", { method: "POST", body: JSON.stringify(body) }),
  },
  socialAccounts: {
    list: () => request<any[]>("/api/v1/social-accounts"),
    connect: (body: { platform: number; displayName: string; channelName: string; accessToken: string }) =>
      request<any>("/api/v1/social-accounts", { method: "POST", body: JSON.stringify(body) }),
    delete: (id: string) => request<void>(`/api/v1/social-accounts/${id}`, { method: "DELETE" }),
  },
  admin: {
    getSystemStats: () => request<AdminSystemStatsResponse>("/api/v1/admin/system-stats"),
    getUsers: (page: number, pageSize: number, search?: string) =>
      request<{ total: number; page: number; pageSize: number; users: AdminUserDto[] }>(
        `/api/v1/admin/users?page=${page}&pageSize=${pageSize}${search ? `&search=${encodeURIComponent(search)}` : ""}`),
    changeRole: (userId: string, role: string) =>
      request<any>(`/api/v1/admin/users/${userId}/role`, { method: "PATCH", body: JSON.stringify({ role }) }),
    changeTier: (userId: string, tier: string) =>
      request<any>(`/api/v1/admin/users/${userId}/tier`, { method: "PATCH", body: JSON.stringify({ tier }) }),
    deleteUser: (userId: string) => request<void>(`/api/v1/admin/users/${userId}`, { method: "DELETE" }),
    getSubscriptions: (page: number, pageSize: number) =>
      request<{ total: number; page: number; pageSize: number; rows: AdminSubscriptionDto[] }>(
        `/api/v1/admin/subscriptions?page=${page}&pageSize=${pageSize}`),
    getRevenue: () => request<AdminRevenueResponse>("/api/v1/admin/revenue"),
    getAuditLogs: (page: number, pageSize: number, action?: string) =>
      request<{ total: number; page: number; pageSize: number; logs: AdminAuditLogDto[] }>(
        `/api/v1/admin/audit-logs?page=${page}&pageSize=${pageSize}${action ? `&action=${encodeURIComponent(action)}` : ""}`),
    getAnalytics: () => request<AdminAnalyticsResponse>("/api/v1/admin/analytics"),
  },
  gemini: {
    getModels: () => request<any[]>("/api/v1/gemini/models"),
    publish: (body: { title: string; durationSeconds: number; viralityScore: number; hook?: string; description?: string; hashtags?: string[]; model: string }) =>
      request<any>("/api/v1/gemini/publish", { method: "POST", body: JSON.stringify(body) }),
  },
};

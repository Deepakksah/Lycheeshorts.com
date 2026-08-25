"use client";

import React, { useState, useEffect, useCallback } from "react";
import {
  api, VideoResponse, ShortClipResponse, ScheduleResponse,
  SubscriptionPlanResponse, PaymentResponse, UserUsageResponse,
  AdminUserDto, AdminSubscriptionDto, AdminRevenueResponse,
  AdminSystemStatsResponse, AdminAuditLogDto, AdminAnalyticsResponse
} from "../lib/api";
import { Sidebar } from "../components/Sidebar";
import { Header } from "../components/Header";
import { WorkspaceTab } from "../components/WorkspaceTab";
import { SchedulerTab } from "../components/SchedulerTab";
import { SocialTab } from "../components/SocialTab";
import { AdminTab } from "../components/AdminTab";
import { BillingTab } from "../components/BillingTab";
import { SettingsTab } from "../components/SettingsTab";
import { AuthModal } from "../components/AuthModal";
import { SettingsModal } from "../components/SettingsModal";
import { FloatingThemeWidget, DockPosition, ThemeColor } from "../components/FloatingThemeWidget";
import { Settings } from "lucide-react";

export default function DashboardPage() {
  const [token, setToken] = useState<string | null>(null);
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);
  const [dockPosition, setDockPosition] = useState<DockPosition>("right");
  const [selectedColor, setSelectedColor] = useState<ThemeColor>("rose");
  const [authMode, setAuthMode] = useState<"login" | "register" | "verify" | "forgot" | "reset">("login");
  const [authEmail, setAuthEmail] = useState("admin@Lychee.com");
  const [authPassword, setAuthPassword] = useState("123456");
  const [authDisplayName, setAuthDisplayName] = useState("");
  const [verificationToken, setVerificationToken] = useState("");
  const [resetToken, setResetToken] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [currentUser, setCurrentUser] = useState<any>(null);
  const [authError, setAuthError] = useState("");

  const [videos, setVideos] = useState<VideoResponse[]>([]);
  const [selectedVideo, setSelectedVideo] = useState<VideoResponse | null>(null);
  const [selectedVideoShorts, setSelectedVideoShorts] = useState<ShortClipResponse[]>([]);
  const [schedules, setSchedules] = useState<ScheduleResponse[]>([]);

  const [youtubeUrl, setYoutubeUrl] = useState("");
  const [youtubeTitle, setYoutubeTitle] = useState("");
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [uploadTitle, setUploadTitle] = useState("");

  const [scheduleClipId, setScheduleClipId] = useState("");
  const [scheduleDate, setScheduleDate] = useState("");
  const [scheduleTime, setScheduleTime] = useState("");
  const [schedulePlatform, setSchedulePlatform] = useState("1");
  const [shortsCache, setShortsCache] = useState<Record<string, ShortClipResponse>>({});
  const [editingScheduleId, setEditingScheduleId] = useState<string | null>(null);
  const [rescheduleDateTime, setRescheduleDateTime] = useState("");

  const [activeTab, setActiveTab] = useState<"workspace" | "scheduler" | "billing" | "social" | "admin" | "settings">("workspace");
  const [socialAccounts, setSocialAccounts] = useState<any[]>([]);
  const [isConnectingPlatform, setIsConnectingPlatform] = useState<number | null>(null);
  const [socialDisplayName, setSocialDisplayName] = useState("");
  const [socialChannelName, setSocialChannelName] = useState("");
  const [socialSecretKey, setSocialSecretKey] = useState("");
  const [selectedSocialAccountId, setSelectedSocialAccountId] = useState("");
  const [connectionStep, setConnectionStep] = useState<"form" | "step1" | "step2" | "step3" | "success">("form");

  const [plans, setPlans] = useState<SubscriptionPlanResponse[]>([]);
  const [paymentHistory, setPaymentHistory] = useState<PaymentResponse[]>([]);
  const [usage, setUsage] = useState<UserUsageResponse | null>(null);
  const [billingCycle, setBillingCycle] = useState<"monthly" | "yearly">("monthly");

  const [adminSubTab, setAdminSubTab] = useState<"overview" | "users" | "subscriptions" | "revenue" | "auditlogs" | "analytics">("overview");
  const [adminUsers, setAdminUsers] = useState<AdminUserDto[]>([]);
  const [adminUsersTotal, setAdminUsersTotal] = useState(0);
  const [adminUsersPage, setAdminUsersPage] = useState(1);
  const [adminUserSearch, setAdminUserSearch] = useState("");
  const [adminSubs, setAdminSubs] = useState<AdminSubscriptionDto[]>([]);
  const [adminSubsTotal, setAdminSubsTotal] = useState(0);
  const [adminSubsPage, setAdminSubsPage] = useState(1);
  const [adminRevenue, setAdminRevenue] = useState<AdminRevenueResponse | null>(null);
  const [adminStats, setAdminStats] = useState<AdminSystemStatsResponse | null>(null);
  const [adminAuditLogs, setAdminAuditLogs] = useState<AdminAuditLogDto[]>([]);
  const [adminAuditTotal, setAdminAuditTotal] = useState(0);
  const [adminAuditPage, setAdminAuditPage] = useState(1);
  const [adminAuditSearch, setAdminAuditSearch] = useState("");
  const [adminAnalytics, setAdminAnalytics] = useState<AdminAnalyticsResponse | null>(null);
  const [adminLoading, setAdminLoading] = useState(false);

  const [loading, setLoading] = useState(false);
  const [actionError, setActionError] = useState("");

  const fetchUserData = useCallback(async () => {
    try { setCurrentUser(await api.auth.me()); } catch { handleLogout(); }
  }, []);

  useEffect(() => {
    if (typeof window !== "undefined") {
      const urlParams = new URLSearchParams(window.location.search);
      const tabParam = urlParams.get("tab");
      if (tabParam && ["workspace","scheduler","billing","social","admin","settings"].includes(tabParam)) {
        setActiveTab(tabParam as any);
      }
    }
    const savedToken = localStorage.getItem("token");
    if (savedToken) { setToken(savedToken); fetchUserData(); }
  }, [fetchUserData]);

  const loadBillingData = useCallback(async () => {
    try {
      const [p, h, u] = await Promise.all([api.payments.getPlans(), api.payments.getHistory(), api.payments.getUsage()]);
      setPlans(p); setPaymentHistory(h); setUsage(u);
    } catch {}
  }, []);

  const loadSocialAccounts = useCallback(async () => {
    try { setSocialAccounts(await api.socialAccounts.list()); } catch {}
  }, []);

  const loadDashboardData = useCallback(async () => {
    try {
      setActionError("");
      const [videoList, scheduleList, usageInfo, accountsList] = await Promise.all([
        api.videos.list(), api.schedules.list(),
        api.payments.getUsage().catch(() => null),
        api.socialAccounts.list().catch(() => [])
      ]);
      setVideos(videoList); setSchedules(scheduleList);
      if (usageInfo) setUsage(usageInfo);
      setSocialAccounts(accountsList);

      const cache: Record<string, ShortClipResponse> = {};
      await Promise.all(videoList.map(async v => {
        if (v.status === "Processed") {
          try { (await api.videos.getShorts(v.id)).forEach(c => { cache[c.id] = c; }); } catch {}
        }
      }));
      setShortsCache(cache);

      if (selectedVideo) {
        const updatedShorts = await api.videos.getShorts(selectedVideo.id);
        setSelectedVideoShorts(updatedShorts);
      }
    } catch (err: any) {
      setActionError(err.message || "Failed to load data.");
    }
  }, [selectedVideo]);

  useEffect(() => {
    if (token) { loadDashboardData(); loadBillingData(); loadSocialAccounts(); }
  }, [token, loadDashboardData, loadBillingData, loadSocialAccounts]);

  useEffect(() => {
    if (!token || videos.length === 0) return;
    const hasProcessing = videos.some(v => v.status === "Queued" || v.status === "Processing");
    if (!hasProcessing) return;
    const interval = setInterval(async () => {
      try {
        const updated = await api.videos.list();
        setVideos(updated);
        if (selectedVideo) {
          const match = updated.find(v => v.id === selectedVideo.id);
          if (match && match.status !== selectedVideo.status) {
            setSelectedVideo(match);
            setSelectedVideoShorts(await api.videos.getShorts(match.id));
          }
        }
      } catch {}
    }, 3000);
    return () => clearInterval(interval);
  }, [token, videos, selectedVideo]);

  const handleAuthSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); setLoading(true); setAuthError("");
    try {
      if (authMode === "login") {
        const r = await api.auth.login({ email: authEmail, password: authPassword });
        localStorage.setItem("token", r.accessToken); setToken(r.accessToken);
        setCurrentUser({ userId: r.userId, email: r.email, displayName: r.displayName, role: r.role });
        setAuthPassword("");
      } else if (authMode === "register") {
        const r = await api.auth.register({ email: authEmail, password: authPassword, displayName: authDisplayName });
        localStorage.setItem("token", r.accessToken); setToken(r.accessToken);
        setCurrentUser({ userId: r.userId, email: r.email, displayName: r.displayName, role: r.role });
        setAuthPassword("");
      } else if (authMode === "verify") {
        await api.auth.verifyEmail(verificationToken); setVerificationToken(""); setAuthMode("login");
      } else if (authMode === "forgot") {
        await api.auth.forgotPassword(authEmail); setAuthMode("reset");
      } else if (authMode === "reset") {
        await api.auth.resetPassword({ token: resetToken, newPassword });
        setResetToken(""); setNewPassword(""); setAuthMode("login");
      }
    } catch (err: any) { setAuthError(err.message || "Authentication failed."); }
    finally { setLoading(false); }
  };

  const handleLogout = () => {
    localStorage.removeItem("token"); setToken(null); setCurrentUser(null);
    setVideos([]); setSelectedVideo(null); setSelectedVideoShorts([]); setSchedules([]);
  };

  const loadAdminData = useCallback(async () => {
    if (!token) return; setAdminLoading(true);
    try {
      if (adminSubTab === "overview") { setAdminStats(await api.admin.getSystemStats()); }
      else if (adminSubTab === "users") {
        const r = await api.admin.getUsers(adminUsersPage, 10, adminUserSearch);
        setAdminUsers(r.users); setAdminUsersTotal(r.total);
      } else if (adminSubTab === "subscriptions") {
        const r = await api.admin.getSubscriptions(adminSubsPage, 10);
        setAdminSubs(r.rows); setAdminSubsTotal(r.total);
      } else if (adminSubTab === "revenue") { setAdminRevenue(await api.admin.getRevenue()); }
      else if (adminSubTab === "auditlogs") {
        const r = await api.admin.getAuditLogs(adminAuditPage, 20, adminAuditSearch);
        setAdminAuditLogs(r.logs); setAdminAuditTotal(r.total);
      } else if (adminSubTab === "analytics") { setAdminAnalytics(await api.admin.getAnalytics()); }
    } catch (err: any) { setActionError(err.message || "Failed to load admin data."); }
    finally { setAdminLoading(false); }
  }, [token, adminSubTab, adminUsersPage, adminUserSearch, adminSubsPage, adminAuditPage, adminAuditSearch]);

  useEffect(() => {
    if (token && activeTab === "admin") loadAdminData();
  }, [token, activeTab, adminSubTab, adminUsersPage, adminSubsPage, adminAuditPage, loadAdminData]);

  const handleAdminChangeRole = async (userId: string, role: string) => {
    setLoading(true);
    try { await api.admin.changeRole(userId, role); loadAdminData(); }
    catch (err: any) { setActionError(err.message || "Failed."); } finally { setLoading(false); }
  };
  const handleAdminChangeTier = async (userId: string, tier: string) => {
    setLoading(true);
    try { await api.admin.changeTier(userId, tier); loadAdminData(); }
    catch (err: any) { setActionError(err.message || "Failed."); } finally { setLoading(false); }
  };
  const handleAdminDeleteUser = async (userId: string) => {
    if (!window.confirm("Permanently delete this user?")) return;
    setLoading(true);
    try { await api.admin.deleteUser(userId); loadAdminData(); }
    catch (err: any) { setActionError(err.message || "Failed."); } finally { setLoading(false); }
  };

  const handleImportYoutube = async (e: React.FormEvent) => {
    e.preventDefault(); if (!youtubeUrl) return; setLoading(true); setActionError("");
    try {
      const v = await api.videos.submitYouTube({ sourceUrl: youtubeUrl, title: youtubeTitle });
      setYoutubeUrl(""); setYoutubeTitle(""); setVideos(p => [v, ...p]); setSelectedVideo(v);
    } catch (err: any) { setActionError(err.message || "Failed to import."); } finally { setLoading(false); }
  };
  const handleUpload = async (e: React.FormEvent) => {
    e.preventDefault(); if (!uploadFile) return; setLoading(true); setActionError("");
    try {
      const v = await api.videos.uploadFile(uploadFile, uploadTitle);
      setUploadFile(null); setUploadTitle(""); setVideos(p => [v, ...p]); setSelectedVideo(v);
    } catch (err: any) { setActionError(err.message || "Failed to upload."); } finally { setLoading(false); }
  };
  const handleTriggerAiProcessing = async (videoId: string) => {
    setLoading(true); setActionError("");
    try {
      await api.videos.triggerProcess(videoId);
      setVideos(p => p.map(v => v.id === videoId ? { ...v, status: "Processing" } : v));
      if (selectedVideo?.id === videoId) setSelectedVideo(p => p ? { ...p, status: "Processing" } : null);
    } catch (err: any) { setActionError(err.message || "Failed to trigger processing."); } finally { setLoading(false); }
  };
  const handleStopVideoProcessing = async (videoId: string) => {
    setLoading(true); setActionError("");
    try {
      await api.videos.stopProcess(videoId);
      setVideos(p => p.map(v => v.id === videoId ? { ...v, status: "Cancelled" } : v));
      if (selectedVideo?.id === videoId) setSelectedVideo(p => p ? { ...p, status: "Cancelled" } : null);
    } catch (err: any) { setActionError(err.message || "Failed to stop processing."); } finally { setLoading(false); }
  };
  const handleDeleteVideo = async (videoId: string) => {
    if (!window.confirm("Delete this video and its short clips?")) return;
    setLoading(true); setActionError("");
    try {
      await api.videos.delete(videoId);
      setVideos(p => p.filter(v => v.id !== videoId));
      if (selectedVideo?.id === videoId) { setSelectedVideo(null); setSelectedVideoShorts([]); }
    } catch (err: any) { setActionError(err.message || "Failed to delete video."); } finally { setLoading(false); }
  };
  const handleDeleteShortClip = async (clipId: string) => {
    if (!window.confirm("Delete this short clip?")) return;
    setLoading(true); setActionError("");
    try {
      await api.videos.deleteShort(clipId);
      setSelectedVideoShorts(p => p.filter(c => c.id !== clipId));
    } catch (err: any) { setActionError(err.message || "Failed to delete clip."); } finally { setLoading(false); }
  };
  const handleCreateSchedule = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!scheduleClipId || !scheduleDate || !scheduleTime) { setActionError("Please fill all scheduling fields."); return; }
    setLoading(true); setActionError("");
    try {
      const publishAtUtc = new Date(`${scheduleDate}T${scheduleTime}:00`).toISOString();
      const s = await api.schedules.create({ shortClipId: scheduleClipId, publishAtUtc, platform: parseInt(schedulePlatform) || 1, socialAccountId: selectedSocialAccountId || "" });
      setSchedules(p => [s, ...p]); setScheduleClipId(""); setScheduleDate(""); setScheduleTime("");
    } catch (err: any) { setActionError(err.message || "Failed to create schedule."); } finally { setLoading(false); }
  };
  const handleDeleteSchedule = async (id: string) => {
    setLoading(true); setActionError("");
    try { await api.schedules.delete(id); setSchedules(p => p.filter(s => s.id !== id)); }
    catch (err: any) { setActionError(err.message || "Failed to delete."); } finally { setLoading(false); }
  };
  const handleReschedule = async (id: string) => {
    if (!rescheduleDateTime) return; setLoading(true); setActionError("");
    try {
      const updated = await api.schedules.update(id, { publishAtUtc: new Date(rescheduleDateTime).toISOString() });
      setSchedules(p => p.map(s => s.id === id ? updated : s)); setEditingScheduleId(null); setRescheduleDateTime("");
    } catch (err: any) { setActionError(err.message || "Failed to reschedule."); } finally { setLoading(false); }
  };
  const handleCheckout = async (planId: string, provider: "Stripe" | "Razorpay") => {
    setLoading(true); setActionError("");
    try {
      const r = await api.payments.createCheckout({ planId, provider, billingCycle, successUrl: window.location.href, cancelUrl: window.location.href });
      if (r.paymentUrl) window.location.href = r.paymentUrl;
    } catch (err: any) { setActionError(err.message || "Payment failed."); } finally { setLoading(false); }
  };
  const handleConnectSocial = async (e: React.FormEvent) => {
    e.preventDefault(); if (isConnectingPlatform === null) return; setLoading(true); setActionError("");
    setConnectionStep("step1"); await new Promise(r => setTimeout(r, 600));
    setConnectionStep("step2"); await new Promise(r => setTimeout(r, 600));
    setConnectionStep("step3"); await new Promise(r => setTimeout(r, 600));
    try {
      const acc = await api.socialAccounts.connect({ platform: isConnectingPlatform, displayName: socialDisplayName, channelName: socialChannelName, accessToken: socialSecretKey });
      setConnectionStep("success"); await new Promise(r => setTimeout(r, 800));
      setSocialAccounts(p => [acc, ...p]); setIsConnectingPlatform(null);
      setSocialDisplayName(""); setSocialChannelName(""); setSocialSecretKey(""); setConnectionStep("form");
    } catch (err: any) { setConnectionStep("form"); setActionError(err.message || "Failed to connect."); }
    finally { setLoading(false); }
  };
  const handleDisconnectSocial = async (id: string) => {
    if (!window.confirm("Disconnect this social account?")) return; setLoading(true); setActionError("");
    try { await api.socialAccounts.delete(id); setSocialAccounts(p => p.filter(a => a.id !== id)); }
    catch (err: any) { setActionError(err.message || "Failed to disconnect."); } finally { setLoading(false); }
  };

  const getPlatformLabel = (p: string) => ({ "1": "YouTube Shorts", "2": "Instagram Reels", "3": "Facebook Reels" }[p] || p);
  const getStatusBadge = (s: string) => {
    const map: Record<string, string> = {
      Scheduled: "bg-blue-50 text-blue-700 border-blue-200",
      Processing: "bg-amber-50 text-amber-700 border-amber-200",
      Publishing: "bg-amber-50 text-amber-700 border-amber-200",
      Published: "bg-emerald-50 text-emerald-700 border-emerald-200",
      Success: "bg-emerald-50 text-emerald-700 border-emerald-200",
      Failed: "bg-rose-50 text-rose-700 border-rose-200",
    };
    return <span className={`rounded-full border px-2 py-0.5 text-[10px] font-bold ${map[s] || "bg-slate-100 text-slate-600 border-slate-200"}`}>{s}</span>;
  };

  if (!token) {
    return (
      <AuthModal
        authMode={authMode} setAuthMode={setAuthMode}
        authEmail={authEmail} setAuthEmail={setAuthEmail}
        authPassword={authPassword} setAuthPassword={setAuthPassword}
        authDisplayName={authDisplayName} setAuthDisplayName={setAuthDisplayName}
        verificationToken={verificationToken} setVerificationToken={setVerificationToken}
        resetToken={resetToken} setResetToken={setResetToken}
        newPassword={newPassword} setNewPassword={setNewPassword}
        authError={authError} setAuthError={setAuthError}
        loading={loading} handleAuthSubmit={handleAuthSubmit}
      />
    );
  }

  return (
    <main className="min-h-screen bg-slate-50 text-slate-900 flex flex-col font-sans antialiased">
      <div className="flex flex-1 min-h-screen">
        <Sidebar activeTab={activeTab} setActiveTab={setActiveTab} currentUser={currentUser} handleLogout={handleLogout} />
        <section className="flex-1 flex flex-col min-w-0 bg-slate-50 relative">
          <Header
            activeTab={activeTab}
            setActiveTab={setActiveTab}
            currentUser={currentUser}
            handleLogout={handleLogout}
            onOpenSettings={() => setIsSettingsOpen(true)}
          />
          {activeTab === "workspace" ? (
            <WorkspaceTab
              videos={videos} selectedVideo={selectedVideo} setSelectedVideo={setSelectedVideo}
              selectedVideoShorts={selectedVideoShorts} schedules={schedules}
              youtubeUrl={youtubeUrl} setYoutubeUrl={setYoutubeUrl}
              youtubeTitle={youtubeTitle} setYoutubeTitle={setYoutubeTitle}
              handleImportYoutube={handleImportYoutube}
              uploadFile={uploadFile} setUploadFile={setUploadFile}
              uploadTitle={uploadTitle} setUploadTitle={setUploadTitle}
              handleUpload={handleUpload} handleTriggerAiProcessing={handleTriggerAiProcessing}
              handleStopVideoProcessing={handleStopVideoProcessing}
              handleDeleteVideo={handleDeleteVideo} handleDeleteShortClip={handleDeleteShortClip}
              scheduleClipId={scheduleClipId} setScheduleClipId={setScheduleClipId}
              scheduleDate={scheduleDate} setScheduleDate={setScheduleDate}
              scheduleTime={scheduleTime} setScheduleTime={setScheduleTime}
              schedulePlatform={schedulePlatform} setSchedulePlatform={setSchedulePlatform}
              selectedSocialAccountId={selectedSocialAccountId} setSelectedSocialAccountId={setSelectedSocialAccountId}
              socialAccounts={socialAccounts} handleCreateSchedule={handleCreateSchedule}
              handleDeleteSchedule={handleDeleteSchedule} loading={loading} actionError={actionError}
            />
          ) : activeTab === "scheduler" ? (
            <SchedulerTab
              schedules={schedules} shortsCache={shortsCache}
              editingScheduleId={editingScheduleId} setEditingScheduleId={setEditingScheduleId}
              rescheduleDateTime={rescheduleDateTime} setRescheduleDateTime={setRescheduleDateTime}
              handleReschedule={handleReschedule} handleDeleteSchedule={handleDeleteSchedule}
              loading={loading} getPlatformLabel={getPlatformLabel} getStatusBadge={getStatusBadge}
            />
          ) : activeTab === "social" ? (
            <SocialTab
              socialAccounts={socialAccounts} isConnectingPlatform={isConnectingPlatform}
              setIsConnectingPlatform={setIsConnectingPlatform} connectionStep={connectionStep}
              setConnectionStep={setConnectionStep} socialDisplayName={socialDisplayName}
              setSocialDisplayName={setSocialDisplayName} socialChannelName={socialChannelName}
              setSocialChannelName={setSocialChannelName} socialSecretKey={socialSecretKey}
              setSocialSecretKey={setSocialSecretKey} handleConnectSocial={handleConnectSocial}
              handleDisconnectSocial={handleDisconnectSocial} setActionError={setActionError}
              loading={loading}
            />
          ) : activeTab === "admin" ? (
            <AdminTab
              adminSubTab={adminSubTab} setAdminSubTab={setAdminSubTab} adminLoading={adminLoading}
              adminStats={adminStats} adminUsers={adminUsers} adminUsersTotal={adminUsersTotal}
              adminUsersPage={adminUsersPage} setAdminUsersPage={setAdminUsersPage}
              adminUserSearch={adminUserSearch} setAdminUserSearch={setAdminUserSearch}
              handleAdminChangeRole={handleAdminChangeRole} handleAdminChangeTier={handleAdminChangeTier}
              handleAdminDeleteUser={handleAdminDeleteUser} adminSubs={adminSubs}
              adminSubsTotal={adminSubsTotal} adminSubsPage={adminSubsPage} setAdminSubsPage={setAdminSubsPage}
              adminRevenue={adminRevenue} adminAuditLogs={adminAuditLogs}
              adminAuditTotal={adminAuditTotal} adminAuditPage={adminAuditPage}
              setAdminAuditPage={setAdminAuditPage} adminAuditSearch={adminAuditSearch}
              setAdminAuditSearch={setAdminAuditSearch} adminAnalytics={adminAnalytics}
              loadAdminData={loadAdminData}
            />
          ) : activeTab === "billing" ? (
            <BillingTab
              plans={plans} usage={usage} paymentHistory={paymentHistory}
              billingCycle={billingCycle} setBillingCycle={setBillingCycle}
              handleCheckout={handleCheckout} loading={loading}
            />
          ) : (
            <SettingsTab
              currentUser={currentUser}
              usage={usage}
              dockPosition={dockPosition}
              setDockPosition={setDockPosition}
              selectedColor={selectedColor}
              setSelectedColor={setSelectedColor}
            />
          )}

          {/* Floating Theme & AI Customizer (Left / Right Dockable & Color Palettes) */}
          <FloatingThemeWidget
            onOpenSettings={() => setActiveTab("settings")}
            activeTab={activeTab}
            setActiveTab={setActiveTab}
          />

          {/* Settings Modal (Can also be triggered via Quick Modal if needed) */}
          <SettingsModal
            isOpen={isSettingsOpen}
            onClose={() => setIsSettingsOpen(false)}
            currentUser={currentUser}
            dockPosition={dockPosition}
            setDockPosition={setDockPosition}
            selectedColor={selectedColor}
            setSelectedColor={setSelectedColor}
            usage={usage}
          />
        </section>
      </div>
    </main>
  );
}

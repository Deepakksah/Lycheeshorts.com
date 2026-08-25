"use client";

import React from "react";
import {
  AdminUserDto, AdminSubscriptionDto, AdminRevenueResponse,
  AdminSystemStatsResponse, AdminAuditLogDto, AdminAnalyticsResponse
} from "../lib/api";
import {
  Shield, Users, CreditCard, BarChart3, ScrollText, TrendingUp,
  ChevronLeft, ChevronRight, Trash2, Edit3, RefreshCw, Loader2,
  Activity, Server, Video, Scissors, Calendar, Globe, CheckCircle2, XCircle
} from "lucide-react";

interface AdminTabProps {
  adminSubTab: "overview" | "users" | "subscriptions" | "revenue" | "auditlogs" | "analytics";
  setAdminSubTab: (t: "overview" | "users" | "subscriptions" | "revenue" | "auditlogs" | "analytics") => void;
  adminLoading: boolean;
  adminStats: AdminSystemStatsResponse | null;
  adminUsers: AdminUserDto[];
  adminUsersTotal: number;
  adminUsersPage: number;
  setAdminUsersPage: (p: number) => void;
  adminUserSearch: string;
  setAdminUserSearch: (v: string) => void;
  handleAdminChangeRole: (userId: string, role: string) => void;
  handleAdminChangeTier: (userId: string, tier: string) => void;
  handleAdminDeleteUser: (userId: string) => void;
  adminSubs: AdminSubscriptionDto[];
  adminSubsTotal: number;
  adminSubsPage: number;
  setAdminSubsPage: (p: number) => void;
  adminRevenue: AdminRevenueResponse | null;
  adminAuditLogs: AdminAuditLogDto[];
  adminAuditTotal: number;
  adminAuditPage: number;
  setAdminAuditPage: (p: number) => void;
  adminAuditSearch: string;
  setAdminAuditSearch: (v: string) => void;
  adminAnalytics: AdminAnalyticsResponse | null;
  loadAdminData: () => void;
}

const subTabs = [
  { id: "overview",      label: "System Overview", icon: Server },
  { id: "users",         label: "User Management",  icon: Users },
  { id: "subscriptions", label: "Subscriptions",   icon: CreditCard },
  { id: "revenue",       label: "Revenue Analytics",icon: TrendingUp },
  { id: "auditlogs",     label: "Audit Logs",      icon: ScrollText },
  { id: "analytics",     label: "Platform Metrics",icon: BarChart3 },
] as const;

export const AdminTab: React.FC<AdminTabProps> = ({
  adminSubTab, setAdminSubTab, adminLoading,
  adminStats, adminUsers, adminUsersTotal, adminUsersPage, setAdminUsersPage,
  adminUserSearch, setAdminUserSearch, handleAdminChangeRole, handleAdminChangeTier, handleAdminDeleteUser,
  adminSubs, adminSubsTotal, adminSubsPage, setAdminSubsPage,
  adminRevenue, adminAuditLogs, adminAuditTotal, adminAuditPage, setAdminAuditPage,
  adminAuditSearch, setAdminAuditSearch, adminAnalytics, loadAdminData
}) => {
  return (
    <div className="flex-1 flex flex-col min-h-0 overflow-hidden w-full bg-slate-50">
      {/* Top Bar Navigation */}
      <div className="bg-white border-b border-slate-200 px-8 flex items-center justify-between py-3 shrink-0 shadow-sm">
        <div className="flex items-center gap-2">
          <div className="h-8 w-8 rounded-xl bg-gradient-to-r from-rose-600 to-red-500 flex items-center justify-center text-white shadow-md">
            <Shield size={16} />
          </div>
          <div>
            <h1 className="text-sm font-black text-slate-900 tracking-tight">Lychee Enterprise Control Console</h1>
            <p className="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Full System Administration</p>
          </div>
        </div>

        {/* Navigation Tabs */}
        <div className="flex items-center gap-1 bg-slate-100 p-1 rounded-xl border border-slate-200">
          {subTabs.map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              onClick={() => setAdminSubTab(id)}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg text-xs font-black transition-all ${
                adminSubTab === id
                  ? "bg-white text-rose-600 shadow-sm"
                  : "text-slate-500 hover:text-slate-800"
              }`}
            >
              <Icon size={14} />
              {label}
            </button>
          ))}
        </div>

        <button
          onClick={loadAdminData}
          disabled={adminLoading}
          className="flex items-center gap-2 px-4 py-2 rounded-xl text-xs font-black bg-slate-900 text-white hover:bg-slate-800 disabled:opacity-50 transition-all shadow-sm"
        >
          <RefreshCw size={13} className={adminLoading ? "animate-spin" : ""} />
          Sync Data
        </button>
      </div>

      {/* Main Full Page Content */}
      <div className="flex-1 overflow-y-auto p-8 w-full">
        {adminLoading ? (
          <div className="flex flex-col items-center justify-center py-32">
            <Loader2 size={40} className="animate-spin text-rose-500 mb-3" />
            <p className="text-xs font-black text-slate-500 uppercase tracking-widest">Fetching System Telemetry...</p>
          </div>
        ) : (
          <div className="w-full space-y-8">
            {/* OVERVIEW */}
            {adminSubTab === "overview" && adminStats && (
              <div className="space-y-6 w-full">
                <div className="flex items-center justify-between">
                  <div>
                    <h2 className="text-xl font-black text-slate-900">System Telemetry & Health</h2>
                    <p className="text-xs text-slate-500 mt-0.5">Real-time stats across all user workspaces, videos, and billing</p>
                  </div>
                  <span className="text-xs font-mono font-bold bg-white px-3 py-1.5 rounded-xl border border-slate-200 text-slate-600 shadow-sm">
                    UTC Time: {new Date(adminStats.serverTime).toLocaleString()}
                  </span>
                </div>

                <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-4">
                  {[
                    { label: "Total Users", value: adminStats.totalUsers, icon: Users, color: "text-violet-600", bg: "bg-violet-50" },
                    { label: "Total Videos", value: adminStats.totalVideos, icon: Video, color: "text-blue-600", bg: "bg-blue-50" },
                    { label: "Short Clips", value: adminStats.totalShorts, icon: Scissors, color: "text-rose-600", bg: "bg-rose-50" },
                    { label: "New Today", value: adminStats.newUsersToday, icon: Activity, color: "text-emerald-600", bg: "bg-emerald-50" },
                    { label: "Schedules", value: adminStats.totalSchedules, icon: Calendar, color: "text-amber-600", bg: "bg-amber-50" },
                    { label: "Payments", value: adminStats.totalPayments, icon: CreditCard, color: "text-indigo-600", bg: "bg-indigo-50" },
                    { label: "Social Channels", value: adminStats.totalSocial, icon: Globe, color: "text-pink-600", bg: "bg-pink-50" },
                  ].map(({ label, value, icon: Icon, color, bg }) => (
                    <div key={label} className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5 hover:shadow-md transition-all">
                      <div className={`h-10 w-10 rounded-xl ${bg} flex items-center justify-center mb-3`}>
                        <Icon size={18} className={color} />
                      </div>
                      <p className={`text-3xl font-black ${color}`}>{value}</p>
                      <p className="text-xs text-slate-500 font-bold mt-1 uppercase tracking-wider">{label}</p>
                    </div>
                  ))}
                </div>

                {/* Status Breakdown Grids */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Video Processing Queue Status</h3>
                    <div className="space-y-3">
                      {adminStats.videosByStatus.map(s => (
                        <div key={s.status} className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                          <span className="text-xs font-bold text-slate-700">{s.status}</span>
                          <span className="text-xs font-black text-rose-600 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{s.count} Videos</span>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Schedule Dispatch Pipeline</h3>
                    <div className="space-y-3">
                      {adminStats.schedulesByStatus.map(s => (
                        <div key={s.status} className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                          <span className="text-xs font-bold text-slate-700">{s.status}</span>
                          <span className="text-xs font-black text-blue-600 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{s.count} Scheduled</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* USERS */}
            {adminSubTab === "users" && (
              <div className="space-y-4 w-full">
                <div className="flex items-center justify-between">
                  <div>
                    <h2 className="text-xl font-black text-slate-900">User Account Directory ({adminUsersTotal})</h2>
                    <p className="text-xs text-slate-500 mt-0.5">Manage user roles, subscription tiers, and account permissions</p>
                  </div>
                  <input
                    type="text" value={adminUserSearch} onChange={e => setAdminUserSearch(e.target.value)}
                    placeholder="Filter by email or name..."
                    className="px-4 py-2 bg-white border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 w-80 shadow-sm"
                  />
                </div>

                <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden w-full">
                  <table className="w-full text-xs">
                    <thead className="bg-slate-100/70 border-b border-slate-200">
                      <tr>
                        {["User Email", "Display Name", "System Role", "Subscription Tier", "Email Verified", "Last Login", "Actions"].map(h => (
                          <th key={h} className="text-left px-6 py-4 font-black text-slate-700 text-xs uppercase tracking-wider">{h}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {adminUsers.map(u => (
                        <tr key={u.id} className="hover:bg-slate-50/80 transition-colors">
                          <td className="px-6 py-4 font-bold text-slate-900">{u.email}</td>
                          <td className="px-6 py-4 text-slate-600 font-medium">{u.displayName || "—"}</td>
                          <td className="px-6 py-4">
                            <select value={u.role} onChange={e => handleAdminChangeRole(u.id, e.target.value)}
                              className="bg-slate-50 border border-slate-200 rounded-xl px-3 py-1.5 text-xs font-bold text-slate-800 focus:outline-none focus:border-rose-400">
                              <option>User</option><option>Admin</option><option>Agency</option>
                            </select>
                          </td>
                          <td className="px-6 py-4">
                            <select value={u.subscriptionTier} onChange={e => handleAdminChangeTier(u.id, e.target.value)}
                              className="bg-slate-50 border border-slate-200 rounded-xl px-3 py-1.5 text-xs font-bold text-slate-800 focus:outline-none focus:border-rose-400">
                              <option>Free</option><option>Starter</option><option>Pro</option><option>Agency</option><option>Enterprise</option>
                            </select>
                          </td>
                          <td className="px-6 py-4">
                            {u.isEmailVerified
                              ? <span className="inline-flex items-center gap-1 text-emerald-600 bg-emerald-50 border border-emerald-200 px-2.5 py-1 rounded-full text-[10px] font-bold"><CheckCircle2 size={12} /> Verified</span>
                              : <span className="inline-flex items-center gap-1 text-rose-600 bg-rose-50 border border-rose-200 px-2.5 py-1 rounded-full text-[10px] font-bold"><XCircle size={12} /> Unverified</span>}
                          </td>
                          <td className="px-6 py-4 text-slate-500 font-medium">
                            {u.lastLoginAtUtc ? new Date(u.lastLoginAtUtc).toLocaleString() : "Never"}
                          </td>
                          <td className="px-6 py-4">
                            <button onClick={() => handleAdminDeleteUser(u.id)} className="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all" title="Delete User">
                              <Trash2 size={14} />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>

                  {/* Pagination Footer */}
                  <div className="px-6 py-4 bg-slate-50 border-t border-slate-200 flex items-center justify-between">
                    <span className="text-xs font-bold text-slate-500">Showing Page {adminUsersPage} · {adminUsersTotal} Total Users</span>
                    <div className="flex gap-2">
                      <button onClick={() => setAdminUsersPage(Math.max(1, adminUsersPage - 1))} disabled={adminUsersPage === 1}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        <ChevronLeft size={14} /> Previous
                      </button>
                      <button onClick={() => setAdminUsersPage(adminUsersPage + 1)} disabled={adminUsersPage * 10 >= adminUsersTotal}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        Next <ChevronRight size={14} />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* SUBSCRIPTIONS */}
            {adminSubTab === "subscriptions" && (
              <div className="space-y-4 w-full">
                <div>
                  <h2 className="text-xl font-black text-slate-900">Subscription & Transaction Ledger ({adminSubsTotal})</h2>
                  <p className="text-xs text-slate-500 mt-0.5">Audit recurring payments across Stripe and Razorpay integrations</p>
                </div>

                <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden w-full">
                  <table className="w-full text-xs">
                    <thead className="bg-slate-100/70 border-b border-slate-200">
                      <tr>
                        {["User Account", "Provider", "Reference ID", "Amount Paid", "Status", "Timestamp"].map(h => (
                          <th key={h} className="text-left px-6 py-4 font-black text-slate-700 text-xs uppercase tracking-wider">{h}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {adminSubs.map(s => (
                        <tr key={s.id} className="hover:bg-slate-50/80 transition-colors">
                          <td className="px-6 py-4 font-bold text-slate-900">{s.userEmail}</td>
                          <td className="px-6 py-4 font-medium text-slate-700">{s.provider}</td>
                          <td className="px-6 py-4 font-mono text-slate-500 text-[11px]">{s.providerPaymentId || "N/A"}</td>
                          <td className="px-6 py-4 font-black text-slate-900">{s.currency} {s.amount.toFixed(2)}</td>
                          <td className="px-6 py-4">
                            <span className={`px-3 py-1 rounded-full text-[10px] font-black border ${s.status === "Completed" ? "bg-emerald-50 text-emerald-600 border-emerald-200" : "bg-slate-100 text-slate-600 border-slate-200"}`}>
                              {s.status}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-slate-500 font-medium">{new Date(s.createdAtUtc).toLocaleString()}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>

                  <div className="px-6 py-4 bg-slate-50 border-t border-slate-200 flex items-center justify-between">
                    <span className="text-xs font-bold text-slate-500">Page {adminSubsPage} · {adminSubsTotal} Total Transactions</span>
                    <div className="flex gap-2">
                      <button onClick={() => setAdminSubsPage(Math.max(1, adminSubsPage - 1))} disabled={adminSubsPage === 1}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        <ChevronLeft size={14} /> Previous
                      </button>
                      <button onClick={() => setAdminSubsPage(adminSubsPage + 1)} disabled={adminSubsPage * 10 >= adminSubsTotal}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        Next <ChevronRight size={14} />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* REVENUE */}
            {adminSubTab === "revenue" && adminRevenue && (
              <div className="space-y-6 w-full">
                <div>
                  <h2 className="text-xl font-black text-slate-900">Revenue & Financial Analytics</h2>
                  <p className="text-xs text-slate-500 mt-0.5">Comprehensive view of platform income, provider splits, and cash flow</p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                  {[
                    { label: "Total Lifetime Revenue", value: `$${adminRevenue.totalRevenue.toFixed(2)}`, color: "text-emerald-600" },
                    { label: "Monthly Revenue", value: `$${adminRevenue.monthlyRevenue.toFixed(2)}`, color: "text-blue-600" },
                    { label: "Weekly Revenue", value: `$${adminRevenue.weeklyRevenue.toFixed(2)}`, color: "text-violet-600" },
                    { label: "Completed Transactions", value: adminRevenue.totalTransactions, color: "text-rose-600" },
                  ].map(({ label, value, color }) => (
                    <div key={label} className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 text-center">
                      <p className={`text-3xl font-black ${color}`}>{value}</p>
                      <p className="text-xs text-slate-500 font-bold mt-1 uppercase tracking-wider">{label}</p>
                    </div>
                  ))}
                </div>

                {adminRevenue.byProvider.length > 0 && (
                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Payment Gateway Volume</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      {adminRevenue.byProvider.map(p => (
                        <div key={p.provider} className="flex items-center justify-between p-4 bg-slate-50 rounded-xl border border-slate-200">
                          <span className="font-black text-slate-800 text-sm">{p.provider} Integration</span>
                          <div className="flex gap-4 items-center">
                            <span className="font-black text-emerald-600 text-base">${p.total.toFixed(2)}</span>
                            <span className="text-xs font-bold text-slate-400 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{p.count} txns</span>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )}

            {/* AUDIT LOGS */}
            {adminSubTab === "auditlogs" && (
              <div className="space-y-4 w-full">
                <div className="flex items-center justify-between">
                  <div>
                    <h2 className="text-xl font-black text-slate-900">Security Audit Logs ({adminAuditTotal})</h2>
                    <p className="text-xs text-slate-500 mt-0.5">Track system mutations, admin actions, and authorization changes</p>
                  </div>
                  <input
                    type="text" value={adminAuditSearch} onChange={e => setAdminAuditSearch(e.target.value)}
                    placeholder="Search audit actions..."
                    className="px-4 py-2 bg-white border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 w-72 shadow-sm"
                  />
                </div>

                <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden w-full">
                  <table className="w-full text-xs">
                    <thead className="bg-slate-100/70 border-b border-slate-200">
                      <tr>
                        {["Action Executed", "Target Entity", "User ID", "Timestamp"].map(h => (
                          <th key={h} className="text-left px-6 py-4 font-black text-slate-700 text-xs uppercase tracking-wider">{h}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {adminAuditLogs.map(log => (
                        <tr key={log.id} className="hover:bg-slate-50/80 transition-colors">
                          <td className="px-6 py-4 font-black text-rose-600">{log.action}</td>
                          <td className="px-6 py-4 text-slate-700 font-medium">{log.entityName || "—"}</td>
                          <td className="px-6 py-4 text-slate-500 font-mono text-xs">{log.userId ? log.userId : "System Internal"}</td>
                          <td className="px-6 py-4 text-slate-500 font-medium">{new Date(log.createdAtUtc).toLocaleString()}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>

                  <div className="px-6 py-4 bg-slate-50 border-t border-slate-200 flex items-center justify-between">
                    <span className="text-xs font-bold text-slate-500">Page {adminAuditPage} · {adminAuditTotal} Log Entries</span>
                    <div className="flex gap-2">
                      <button onClick={() => setAdminAuditPage(Math.max(1, adminAuditPage - 1))} disabled={adminAuditPage === 1}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        <ChevronLeft size={14} /> Previous
                      </button>
                      <button onClick={() => setAdminAuditPage(adminAuditPage + 1)} disabled={adminAuditPage * 20 >= adminAuditTotal}
                        className="px-3 py-1.5 text-xs font-bold text-slate-600 hover:bg-white border border-slate-200 disabled:opacity-40 rounded-xl transition-all flex items-center gap-1">
                        Next <ChevronRight size={14} />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* ANALYTICS */}
            {adminSubTab === "analytics" && adminAnalytics && (
              <div className="space-y-6 w-full">
                <div>
                  <h2 className="text-xl font-black text-slate-900">Enterprise Metrics & Growth Funnel</h2>
                  <p className="text-xs text-slate-500 mt-0.5">High-level growth, social reach, and publishing success rates</p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Active Platform Creators (7 Days)</h3>
                    <p className="text-4xl font-black text-violet-600">{adminAnalytics.activeUsersLast7Days}</p>
                  </div>

                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Publishing Success Ratio</h3>
                    <div className="space-y-2">
                      {adminAnalytics.publishingSuccessRate.map(s => (
                        <div key={s.status} className="flex justify-between items-center text-xs p-2.5 bg-slate-50 rounded-xl border border-slate-100">
                          <span className="font-bold text-slate-700">{s.status}</span>
                          <span className="font-black text-slate-900 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{s.count}</span>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Video Processing Funnel</h3>
                    <div className="space-y-2">
                      {adminAnalytics.videoFunnel.map(s => (
                        <div key={s.status} className="flex justify-between items-center text-xs p-2.5 bg-slate-50 rounded-xl border border-slate-100">
                          <span className="font-bold text-slate-700">{s.status}</span>
                          <span className="font-black text-slate-900 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{s.count}</span>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">
                    <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-4">Channel Distribution</h3>
                    <div className="space-y-2">
                      {adminAnalytics.platformBreakdown.map(p => (
                        <div key={p.platform} className="flex justify-between items-center text-xs p-2.5 bg-slate-50 rounded-xl border border-slate-100">
                          <span className="font-bold text-slate-700">{p.platform}</span>
                          <span className="font-black text-slate-900 bg-white px-2.5 py-1 rounded-lg border border-slate-200">{p.count}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

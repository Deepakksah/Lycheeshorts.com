"use client";

import React, { useState } from "react";
import { Youtube, Instagram, Facebook, Plus, Trash2, CheckCircle2, Loader2, ChevronLeft, ChevronRight, Link2, Unlink, AlertCircle } from "lucide-react";

interface SocialTabProps {
  socialAccounts: any[];
  isConnectingPlatform: number | null;
  setIsConnectingPlatform: (p: number | null) => void;
  connectionStep: "form" | "step1" | "step2" | "step3" | "success";
  setConnectionStep: (s: "form" | "step1" | "step2" | "step3" | "success") => void;
  socialDisplayName: string; setSocialDisplayName: (v: string) => void;
  socialChannelName: string; setSocialChannelName: (v: string) => void;
  socialSecretKey: string; setSocialSecretKey: (v: string) => void;
  handleConnectSocial: (e: React.FormEvent) => void;
  handleDisconnectSocial: (id: string) => void;
  setActionError: (v: string) => void;
  loading: boolean;
}

const PLATFORMS = [
  { id: 1, name: "YouTube Shorts", Icon: Youtube, color: "text-red-500", bg: "bg-red-50", border: "border-red-200" },
  { id: 2, name: "Instagram Reels", Icon: Instagram, color: "text-pink-500", bg: "bg-pink-50", border: "border-pink-200" },
  { id: 3, name: "Facebook Reels", Icon: Facebook, color: "text-blue-500", bg: "bg-blue-50", border: "border-blue-200" },
];

export const SocialTab: React.FC<SocialTabProps> = ({
  socialAccounts, isConnectingPlatform, setIsConnectingPlatform, connectionStep, setConnectionStep,
  socialDisplayName, setSocialDisplayName, socialChannelName, setSocialChannelName,
  socialSecretKey, setSocialSecretKey, handleConnectSocial, handleDisconnectSocial,
  setActionError, loading
}) => {
  return (
    <div className="flex-1 overflow-y-auto p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        <div>
          <h2 className="text-lg font-black text-slate-900">Social Channels</h2>
          <p className="text-xs text-slate-500 mt-0.5">Connect your channels to auto-publish AI-generated shorts</p>
        </div>

        {/* Platform cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {PLATFORMS.map(({ id, name, Icon, color, bg, border }) => {
            const connected = socialAccounts.filter(a => a.platform === id || a.platform === id.toString());
            const isConnecting = isConnectingPlatform === id;
            return (
              <div key={id} className={`bg-white rounded-2xl border shadow-sm p-5 ${isConnecting ? `${border} shadow-sm` : "border-slate-200"}`}>
                <div className="flex items-center justify-between mb-4">
                  <div className={`h-10 w-10 rounded-xl ${bg} ${border} border flex items-center justify-center`}>
                    <Icon size={20} className={color} />
                  </div>
                  <span className={`text-[10px] font-black px-2 py-0.5 rounded-full border ${connected.length > 0 ? "bg-emerald-50 text-emerald-600 border-emerald-200" : "bg-slate-100 text-slate-500 border-slate-200"}`}>
                    {connected.length > 0 ? `${connected.length} Connected` : "Not Connected"}
                  </span>
                </div>
                <h3 className="font-black text-slate-800 text-sm mb-1">{name}</h3>
                {connected.map(acc => (
                  <div key={acc.id} className="flex items-center justify-between mt-2 p-2 bg-slate-50 rounded-xl border border-slate-200">
                    <div className="min-w-0">
                      <p className="text-xs font-bold text-slate-700 truncate">{acc.displayName}</p>
                      <p className="text-[10px] text-slate-400 truncate">@{acc.channelName}</p>
                    </div>
                    <button
                      onClick={() => handleDisconnectSocial(acc.id)}
                      className="p-1.5 text-slate-400 hover:text-rose-500 hover:bg-rose-50 rounded-lg transition-all border border-transparent hover:border-rose-200"
                    >
                      <Unlink size={12} />
                    </button>
                  </div>
                ))}
                <button
                  onClick={() => { setIsConnectingPlatform(isConnecting ? null : id); setConnectionStep("form"); }}
                  className={`mt-3 w-full py-2 rounded-xl text-xs font-black transition-all border flex items-center justify-center gap-1.5 ${isConnecting ? "bg-slate-100 text-slate-600 border-slate-200" : `${bg} ${color} ${border}`}`}
                >
                  {isConnecting ? <><ChevronLeft size={12} /> Cancel</> : <><Plus size={12} /> Connect Channel</>}
                </button>
              </div>
            );
          })}
        </div>

        {/* Connect form */}
        {isConnectingPlatform !== null && (
          <div className="bg-white rounded-2xl border border-rose-200 shadow-sm p-6">
            <h3 className="font-black text-slate-800 mb-4 flex items-center gap-2">
              <Link2 size={14} className="text-rose-500" />
              Connect {PLATFORMS.find(p => p.id === isConnectingPlatform)?.name}
            </h3>

            {connectionStep === "form" ? (
              <form onSubmit={handleConnectSocial} className="space-y-4">
                <div>
                  <label className="text-[11px] font-bold text-slate-500 mb-1 block">Display Name</label>
                  <input type="text" value={socialDisplayName} onChange={e => setSocialDisplayName(e.target.value)}
                    placeholder="My YouTube Channel" required
                    className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:border-rose-400" />
                </div>
                <div>
                  <label className="text-[11px] font-bold text-slate-500 mb-1 block">Channel / Handle Name</label>
                  <input type="text" value={socialChannelName} onChange={e => setSocialChannelName(e.target.value)}
                    placeholder="@mychannel" required
                    className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:border-rose-400" />
                </div>
                <div>
                  <label className="text-[11px] font-bold text-slate-500 mb-1 block">Access Token / API Key</label>
                  <input type="password" value={socialSecretKey} onChange={e => setSocialSecretKey(e.target.value)}
                    placeholder="Paste OAuth access token..." required
                    className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:border-rose-400" />
                </div>
                <button type="submit" disabled={loading}
                  className="w-full py-3 bg-gradient-to-r from-rose-600 to-red-500 text-white text-sm font-black rounded-xl hover:from-rose-500 hover:to-red-400 disabled:opacity-50 transition-all flex items-center justify-center gap-2">
                  {loading ? <Loader2 size={14} className="animate-spin" /> : <CheckCircle2 size={14} />}
                  Connect Channel
                </button>
              </form>
            ) : connectionStep === "success" ? (
              <div className="text-center py-8">
                <CheckCircle2 size={40} className="mx-auto text-emerald-500 mb-3" />
                <p className="font-black text-slate-800 text-lg">Channel Connected!</p>
                <p className="text-xs text-slate-500 mt-1">You can now schedule clips to this channel.</p>
              </div>
            ) : (
              <div className="text-center py-8">
                <Loader2 size={32} className="mx-auto text-rose-400 mb-3 animate-spin" />
                <p className="font-bold text-slate-700">
                  {connectionStep === "step1" ? "Verifying credentials..." : connectionStep === "step2" ? "Authenticating channel..." : "Finalizing connection..."}
                </p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

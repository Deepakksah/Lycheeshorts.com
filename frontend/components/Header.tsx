"use client";

import React from "react";
import { PlaySquare, CalendarClock, Layers, CreditCard, Shield, LayoutDashboard, Settings } from "lucide-react";

interface HeaderProps {
  activeTab: "workspace" | "scheduler" | "social" | "billing" | "admin";
  setActiveTab: (tab: "workspace" | "scheduler" | "social" | "billing" | "admin") => void;
  currentUser: any;
  handleLogout: () => void;
  onOpenSettings?: () => void;
}

const tabLabels: Record<string, string> = {
  workspace: "Workspace & Library",
  scheduler: "Scheduler Calendar",
  social:    "Social Channels",
  billing:   "Billing & Plans",
  admin:     "Admin Control Suite",
};

const tabIcons: Record<string, React.FC<{ size?: number; className?: string }>> = {
  workspace: PlaySquare,
  scheduler: CalendarClock,
  social:    Layers,
  billing:   CreditCard,
  admin:     Shield,
};

export const Header: React.FC<HeaderProps> = ({ activeTab, setActiveTab, currentUser, onOpenSettings }) => {
  const Icon = tabIcons[activeTab] || LayoutDashboard;
  const label = tabLabels[activeTab] || "";

  return (
    <header className="sticky top-0 z-10 bg-white/80 backdrop-blur-md border-b border-slate-200/80 shadow-xs flex items-center justify-between px-6 py-3">
      <div className="flex items-center gap-3">
        <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-rose-50 border border-rose-100">
          <Icon size={16} className="text-rose-500" />
        </div>
        <div>
          <h1 className="text-sm font-black text-slate-900 leading-none">{label}</h1>
          <p className="text-[10px] text-slate-400 mt-0.5">Lychee Web Dashboard</p>
        </div>
      </div>

      <div className="flex items-center gap-3">
        {/* Mobile navigation buttons */}
        <div className="flex md:hidden gap-1">
          {(["workspace","scheduler","social","billing"] as const).map(tab => {
            const TabIcon = tabIcons[tab];
            return (
              <button
                key={tab}
                onClick={() => setActiveTab(tab)}
                className={`p-2 rounded-lg transition-all ${activeTab === tab ? "bg-rose-100 text-rose-600" : "text-slate-400 hover:text-slate-600"}`}
              >
                <TabIcon size={16} />
              </button>
            );
          })}
        </div>

        {/* Settings button in header */}
        {onOpenSettings && (
          <button
            onClick={onOpenSettings}
            title="Settings & Configurations"
            className="p-2 text-slate-500 hover:text-rose-600 hover:bg-rose-50 border border-slate-200 hover:border-rose-200 rounded-xl transition-all flex items-center gap-1.5 text-xs font-bold"
          >
            <Settings size={15} />
            <span className="hidden md:inline">Settings</span>
          </button>
        )}

        {/* User profile avatar badge in Header */}
        {currentUser && (
          <div className="flex items-center gap-2.5 pl-3 border-l border-slate-200">
            <div className="h-7 w-7 rounded-full bg-rose-950 border border-rose-800/60 flex items-center justify-center text-[10px] font-bold text-rose-300">
              {currentUser.displayName ? currentUser.displayName.slice(0, 2).toUpperCase() : "US"}
            </div>
            <div className="hidden sm:block text-left">
              <p className="text-xs font-bold text-slate-800 leading-tight">{currentUser.displayName || "User"}</p>
              <p className="text-[10px] text-slate-400 font-medium">{currentUser.email}</p>
            </div>
          </div>
        )}
      </div>
    </header>
  );
};

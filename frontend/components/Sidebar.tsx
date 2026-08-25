"use client";

import React, { useState } from "react";
import { PlaySquare, CalendarClock, Layers, CreditCard, Shield, LogOut, Menu } from "lucide-react";

interface SidebarProps {
  activeTab: "workspace" | "scheduler" | "social" | "billing" | "admin";
  setActiveTab: (tab: "workspace" | "scheduler" | "social" | "billing" | "admin") => void;
  currentUser: any;
  handleLogout: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ activeTab, setActiveTab, currentUser, handleLogout }) => {
  const [isCollapsed, setIsCollapsed] = useState(false);

  const navItems = [
    { id: "workspace", label: "Workspace & Library", icon: PlaySquare },
    { id: "scheduler", label: "Scheduler Calendar", icon: CalendarClock },
    { id: "social",    label: "Social Channels",    icon: Layers },
    { id: "billing",   label: "Billing & Plans",    icon: CreditCard },
    ...(currentUser?.role === "Admin" ? [{ id: "admin", label: "Admin Control Suite", icon: Shield }] : []),
  ] as const;

  return (
    <aside className={`${isCollapsed ? "w-20" : "w-64"} shrink-0 bg-zinc-950 border-r border-rose-950/40 p-4 flex flex-col justify-between hidden md:flex shadow-xl z-20 transition-all duration-300`}>
      <div className="space-y-6">
        {/* Header */}
        <div className={`flex items-center ${isCollapsed ? "justify-center flex-col gap-3" : "justify-between"} px-1 py-1`}>
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-gradient-to-tr from-rose-700 via-rose-600 to-red-500 shadow-md shadow-rose-950/50 hover:scale-105 transition-transform">
              <PlaySquare className="text-white fill-white/20" size={20} />
            </div>
            {!isCollapsed && (
              <span className="text-base font-black tracking-tight text-white block leading-none">
                Lychee <span className="text-rose-500 font-normal text-xs uppercase tracking-widest block mt-0.5">Shorts AI</span>
              </span>
            )}
          </div>
          <button
            onClick={() => setIsCollapsed(!isCollapsed)}
            title={isCollapsed ? "Expand Sidebar" : "Collapse Sidebar"}
            className="p-2 text-zinc-400 hover:text-white hover:bg-rose-950/50 border border-transparent hover:border-rose-800/40 rounded-xl transition-all flex items-center justify-center"
          >
            <Menu size={20} />
          </button>
        </div>

        {/* Nav */}
        <nav className="space-y-1.5">
          {navItems.map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              onClick={() => setActiveTab(id as any)}
              title={isCollapsed ? label : undefined}
              className={`w-full flex items-center ${isCollapsed ? "justify-center" : "gap-3"} px-3 py-3 rounded-xl text-xs font-bold transition-all ${
                activeTab === id
                  ? "bg-rose-600/20 text-rose-300 border border-rose-500/30 shadow-sm"
                  : "text-zinc-400 hover:text-white hover:bg-zinc-900 border border-transparent"
              }`}
            >
              <Icon size={18} className={activeTab === id ? "text-rose-400" : "text-zinc-400"} />
              {!isCollapsed && <span className="truncate">{label}</span>}
            </button>
          ))}
        </nav>
      </div>

      {/* User profile + Sign Out — single compact row */}
      <div className="pt-4 border-t border-zinc-900">
        {isCollapsed ? (
          <button
            onClick={handleLogout}
            title="Sign Out"
            className="w-full flex items-center justify-center py-2.5 rounded-xl text-zinc-400 hover:text-rose-400 hover:bg-rose-950/30 border border-transparent hover:border-rose-900/40 transition-all"
          >
            <LogOut size={18} />
          </button>
        ) : (
          <div className="flex items-center gap-2 px-2">
            <div className="h-8 w-8 shrink-0 rounded-full bg-rose-950 border border-rose-800/60 flex items-center justify-center text-xs font-bold text-rose-300">
              {currentUser?.displayName ? currentUser.displayName.slice(0, 2).toUpperCase() : "US"}
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-bold text-white truncate leading-tight">{currentUser?.displayName || "User"}</p>
              <p className="text-[10px] text-zinc-500 truncate">{currentUser?.email}</p>
            </div>
            <button
              onClick={handleLogout}
              title="Sign Out"
              className="shrink-0 p-2 rounded-xl text-zinc-500 hover:text-rose-400 hover:bg-rose-950/30 border border-transparent hover:border-rose-900/40 transition-all"
            >
              <LogOut size={15} />
            </button>
          </div>
        )}
      </div>
    </aside>
  );
};

"use client";

import React, { useState, useEffect } from "react";
import {
  Palette, Sun, Moon, Sparkles, Sliders, ArrowLeftRight,
  ChevronLeft, ChevronRight, Check, Settings, Eye, Zap,
  Monitor, Compass, X, RefreshCw
} from "lucide-react";

export type ThemeColor = "rose" | "violet" | "blue" | "emerald" | "amber" | "fuchsia";
export type DockPosition = "left" | "right";
export type ThemeMode = "light" | "dark" | "system";

interface FloatingThemeWidgetProps {
  onOpenSettings?: () => void;
  activeTab?: string;
  setActiveTab?: (tab: any) => void;
}

export const colorThemes: Record<ThemeColor, {
  name: string;
  primary: string;
  gradient: string;
  ring: string;
  badge: string;
  accent: string;
  bgLight: string;
  borderLight: string;
}> = {
  rose: {
    name: "Lychee Rose",
    primary: "#e11d48",
    gradient: "from-rose-600 to-red-500",
    ring: "ring-rose-500",
    badge: "bg-rose-500",
    accent: "text-rose-500",
    bgLight: "bg-rose-50",
    borderLight: "border-rose-200",
  },
  violet: {
    name: "Cyber Violet",
    primary: "#7c3aed",
    gradient: "from-violet-600 to-purple-600",
    ring: "ring-violet-500",
    badge: "bg-violet-500",
    accent: "text-violet-500",
    bgLight: "bg-violet-50",
    borderLight: "border-violet-200",
  },
  blue: {
    name: "Ocean Blue",
    primary: "#0284c7",
    gradient: "from-sky-500 to-blue-600",
    ring: "ring-blue-500",
    badge: "bg-blue-500",
    accent: "text-blue-500",
    bgLight: "bg-sky-50",
    borderLight: "border-sky-200",
  },
  emerald: {
    name: "Emerald Mint",
    primary: "#059669",
    gradient: "from-emerald-500 to-teal-600",
    ring: "ring-emerald-500",
    badge: "bg-emerald-500",
    accent: "text-emerald-500",
    bgLight: "bg-emerald-50",
    borderLight: "border-emerald-200",
  },
  amber: {
    name: "Sunset Orange",
    primary: "#ea580c",
    gradient: "from-amber-500 to-orange-600",
    ring: "ring-orange-500",
    badge: "bg-orange-500",
    accent: "text-orange-500",
    bgLight: "bg-amber-50",
    borderLight: "border-amber-200",
  },
  fuchsia: {
    name: "Neon Fuchsia",
    primary: "#c026d3",
    gradient: "from-fuchsia-500 to-pink-600",
    ring: "ring-fuchsia-500",
    badge: "bg-fuchsia-500",
    accent: "text-fuchsia-500",
    bgLight: "bg-fuchsia-50",
    borderLight: "border-fuchsia-200",
  },
};

export const FloatingThemeWidget: React.FC<FloatingThemeWidgetProps> = ({
  onOpenSettings,
  activeTab,
  setActiveTab,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [dockPosition, setDockPosition] = useState<DockPosition>("right");
  const [selectedColor, setSelectedColor] = useState<ThemeColor>("rose");
  const [themeMode, setThemeMode] = useState<ThemeMode>("light");

  // Load preferences from localStorage on mount
  useEffect(() => {
    if (typeof window !== "undefined") {
      const savedPos = localStorage.getItem("lychee_dock_pos") as DockPosition;
      const savedColor = localStorage.getItem("lychee_theme_color") as ThemeColor;
      const savedMode = localStorage.getItem("lychee_theme_mode") as ThemeMode;

      if (savedPos && (savedPos === "left" || savedPos === "right")) {
        setDockPosition(savedPos);
      }
      if (savedColor && colorThemes[savedColor]) {
        setSelectedColor(savedColor);
        applyThemeColor(savedColor);
      }
      if (savedMode) {
        setThemeMode(savedMode);
      }
    }
  }, []);

  const applyThemeColor = (colorKey: ThemeColor) => {
    setSelectedColor(colorKey);
    if (typeof window !== "undefined") {
      localStorage.setItem("lychee_theme_color", colorKey);
      const theme = colorThemes[colorKey];
      document.documentElement.style.setProperty("--theme-primary", theme.primary);
      document.documentElement.setAttribute("data-theme-color", colorKey);
    }
  };

  const handleToggleDock = () => {
    const newPos = dockPosition === "right" ? "left" : "right";
    setDockPosition(newPos);
    if (typeof window !== "undefined") {
      localStorage.setItem("lychee_dock_pos", newPos);
    }
  };

  const currentTheme = colorThemes[selectedColor] || colorThemes.rose;

  return (
    <aside aria-label="Theme customizer"
      className={`fixed top-1/2 -translate-y-1/2 z-50 transition-all duration-300 ${
        dockPosition === "left" ? "left-0" : "right-0"
      }`}
    >
      {/* Floating Trigger Tab */}
      {!isOpen ? (
        <button
          onClick={() => setIsOpen(true)}
          title={`Theme & Layout Customizer (Docked ${dockPosition})`}
          className={`flex items-center gap-2 px-3 py-3.5 bg-zinc-950/95 hover:bg-zinc-900 text-white shadow-2xl backdrop-blur-md border border-zinc-700/60 hover:scale-105 active:scale-95 transition-all group ${
            dockPosition === "left"
              ? "rounded-r-2xl border-l-0 pl-3.5"
              : "rounded-l-2xl border-r-0 pr-3.5"
          }`}
        >
          {dockPosition === "left" && (
            <div className="h-3 w-3 rounded-full bg-gradient-to-tr from-rose-500 to-amber-400 ring-2 ring-white/30 animate-pulse" />
          )}

          <div className="flex flex-col items-center gap-1">
            <Palette size={18} className="text-rose-400 group-hover:rotate-45 transition-transform duration-300" />
            <span className="text-[9px] font-black uppercase tracking-tighter text-zinc-300 [writing-mode:vertical-rl] rotate-180">
              Customizer
            </span>
          </div>

          {dockPosition === "right" && (
            <div className="h-3 w-3 rounded-full bg-gradient-to-tr from-rose-500 to-amber-400 ring-2 ring-white/30 animate-pulse" />
          )}
        </button>
      ) : (
        /* Floating Customizer Drawer / Card */
        <div
          className={`w-72 bg-white/95 backdrop-blur-xl border border-slate-200 shadow-2xl p-5 flex flex-col gap-4 animate-in slide-in-from-${dockPosition} duration-300 ${
            dockPosition === "left"
              ? "rounded-r-3xl border-l-0 shadow-rose-950/10"
              : "rounded-l-3xl border-r-0 shadow-rose-950/10"
          }`}
        >
          {/* Header */}
          <div className="flex items-center justify-between border-b border-slate-100 pb-3">
            <div className="flex items-center gap-2">
              <div className="h-8 w-8 rounded-xl bg-gradient-to-tr from-rose-600 to-red-500 flex items-center justify-center text-white shadow-md shadow-rose-500/20">
                <Palette size={15} />
              </div>
              <div>
                <h3 className="text-xs font-black text-slate-900">UI Customizer</h3>
                <p className="text-[10px] text-slate-400">Position & Color Themes</p>
              </div>
            </div>
            <button
              onClick={() => setIsOpen(false)}
              className="p-1.5 text-slate-400 hover:text-slate-700 hover:bg-slate-100 rounded-lg transition-all"
            >
              <X size={16} />
            </button>
          </div>

          {/* Dock Position Switcher: Left vs Right */}
          <div>
            <div className="flex items-center justify-between mb-2">
              <span className="text-[11px] font-black text-slate-700 uppercase tracking-wider flex items-center gap-1.5">
                <ArrowLeftRight size={12} className="text-rose-500" /> Floating Position
              </span>
              <span className="text-[10px] font-bold text-slate-400 capitalize">
                Docked: {dockPosition}
              </span>
            </div>

            <div className="grid grid-cols-2 gap-2 bg-slate-100 p-1 rounded-xl">
              <button
                type="button"
                onClick={() => {
                  setDockPosition("left");
                  localStorage.setItem("lychee_dock_pos", "left");
                }}
                className={`flex items-center justify-center gap-1.5 py-2 rounded-lg text-xs font-bold transition-all ${
                  dockPosition === "left"
                    ? "bg-white text-rose-600 shadow-xs"
                    : "text-slate-500 hover:text-slate-800"
                }`}
              >
                <ChevronLeft size={14} /> Float Left
              </button>
              <button
                type="button"
                onClick={() => {
                  setDockPosition("right");
                  localStorage.setItem("lychee_dock_pos", "right");
                }}
                className={`flex items-center justify-center gap-1.5 py-2 rounded-lg text-xs font-bold transition-all ${
                  dockPosition === "right"
                    ? "bg-white text-rose-600 shadow-xs"
                    : "text-slate-500 hover:text-slate-800"
                }`}
              >
                Float Right <ChevronRight size={14} />
              </button>
            </div>
          </div>

          {/* Color Themes Palette */}
          <div>
            <span className="text-[11px] font-black text-slate-700 uppercase tracking-wider mb-2.5 flex items-center gap-1.5">
              <Sparkles size={12} className="text-rose-500" /> Accent Color Theme
            </span>

            <div className="grid grid-cols-3 gap-2">
              {(Object.keys(colorThemes) as ThemeColor[]).map((key) => {
                const item = colorThemes[key];
                const isSelected = selectedColor === key;
                return (
                  <button
                    key={key}
                    type="button"
                    onClick={() => applyThemeColor(key)}
                    className={`flex flex-col items-center justify-center p-2.5 rounded-xl border transition-all text-center group ${
                      isSelected
                        ? "border-slate-900 bg-slate-900 text-white shadow-md ring-2 ring-slate-900/20"
                        : "border-slate-200 bg-white hover:border-slate-300 text-slate-700"
                    }`}
                  >
                    <div
                      className={`h-5 w-5 rounded-full bg-gradient-to-tr ${item.gradient} shadow-sm mb-1.5 flex items-center justify-center`}
                    >
                      {isSelected && <Check size={11} className="text-white stroke-[3]" />}
                    </div>
                    <span className="text-[10px] font-bold truncate max-w-full">
                      {item.name.split(" ")[1] || item.name}
                    </span>
                  </button>
                );
              })}
            </div>
          </div>

          {/* Quick Shortcuts */}
          {setActiveTab && (
            <div>
              <span className="text-[11px] font-black text-slate-700 uppercase tracking-wider mb-2 block">
                Quick Navigation
              </span>
              <div className="grid grid-cols-2 gap-1.5">
                {[
                  { id: "workspace", label: "Workspace" },
                  { id: "scheduler", label: "Scheduler" },
                  { id: "social",    label: "Social" },
                  { id: "billing",   label: "Billing" },
                ].map(t => (
                  <button
                    key={t.id}
                    onClick={() => {
                      setActiveTab(t.id);
                      setIsOpen(false);
                    }}
                    className={`py-1.5 px-2.5 rounded-lg text-[11px] font-bold text-left transition-all truncate ${
                      activeTab === t.id
                        ? "bg-rose-50 text-rose-600 border border-rose-200"
                        : "bg-slate-50 text-slate-600 hover:bg-slate-100 border border-transparent"
                    }`}
                  >
                    • {t.label}
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* AI Settings Action Button */}
          {onOpenSettings && (
            <div className="pt-2 border-t border-slate-100">
              <button
                type="button"
                onClick={() => {
                  onOpenSettings();
                  setIsOpen(false);
                }}
                className="w-full py-2.5 px-3 bg-gradient-to-r from-rose-600 to-red-500 hover:from-rose-500 hover:to-red-400 text-white rounded-xl text-xs font-black shadow-md shadow-rose-600/20 flex items-center justify-center gap-2 transition-all"
              >
                <Settings size={14} /> Full AI & Video Settings
              </button>
            </div>
          )}
        </div>
      )}
    </aside>
  );
};

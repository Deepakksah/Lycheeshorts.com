"use client";

import React, { useState, useEffect } from "react";
import {
  Settings, X, Sparkles, Sliders, Shield, Video, Bell,
  ExternalLink, CheckCircle2, RotateCcw, Volume2, Film,
  Share2, Key, HelpCircle, Palette, ArrowLeftRight, Check,
  Moon, Sun, Laptop, User, CreditCard, Layers, CheckSquare,
  AlertCircle
} from "lucide-react";

export type ThemeColor = "rose" | "violet" | "blue" | "emerald" | "amber" | "fuchsia";
export type DockPosition = "left" | "right";

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

interface SettingsModalProps {
  isOpen: boolean;
  onClose: () => void;
  currentUser?: any;
  dockPosition?: DockPosition;
  setDockPosition?: (pos: DockPosition) => void;
  selectedColor?: ThemeColor;
  setSelectedColor?: (color: ThemeColor) => void;
  usage?: any;
}

export const SettingsModal: React.FC<SettingsModalProps> = ({
  isOpen,
  onClose,
  currentUser,
  dockPosition = "right",
  setDockPosition,
  selectedColor = "rose",
  setSelectedColor,
  usage,
}) => {
  const [activeSubTab, setActiveSubTab] = useState<"appearance" | "ai" | "publishing" | "system" | "account">("appearance");
  const [savedSuccess, setSavedSuccess] = useState(false);

  // Settings states with localStorage persistence
  const [aspectRatio, setAspectRatio] = useState("9:16");
  const [clipDuration, setClipDuration] = useState("30-60");
  const [viralityThreshold, setViralityThreshold] = useState(70);
  const [enablePitchShift, setEnablePitchShift] = useState(true);
  const [autoCaptions, setAutoCaptions] = useState(true);
  const [defaultHashtags, setDefaultHashtags] = useState("#Shorts #Viral #LycheeAI #Trending");
  const [defaultVisibility, setDefaultVisibility] = useState("public");
  const [autoPinnedComment, setAutoPinnedComment] = useState("🔥 Created with Lychee Shorts AI — subscribe for more daily clips!");
  const [apiUrl, setApiUrl] = useState("http://localhost:5000");

  useEffect(() => {
    if (typeof window !== "undefined") {
      const saved = localStorage.getItem("lychee_settings");
      if (saved) {
        try {
          const parsed = JSON.parse(saved);
          if (parsed.aspectRatio) setAspectRatio(parsed.aspectRatio);
          if (parsed.clipDuration) setClipDuration(parsed.clipDuration);
          if (parsed.viralityThreshold !== undefined) setViralityThreshold(parsed.viralityThreshold);
          if (parsed.enablePitchShift !== undefined) setEnablePitchShift(parsed.enablePitchShift);
          if (parsed.autoCaptions !== undefined) setAutoCaptions(parsed.autoCaptions);
          if (parsed.defaultHashtags) setDefaultHashtags(parsed.defaultHashtags);
          if (parsed.defaultVisibility) setDefaultVisibility(parsed.defaultVisibility);
          if (parsed.autoPinnedComment) setAutoPinnedComment(parsed.autoPinnedComment);
          if (parsed.apiUrl) setApiUrl(parsed.apiUrl);
        } catch {}
      }
    }
  }, [isOpen]);

  const handleSaveSettings = () => {
    const config = {
      aspectRatio,
      clipDuration,
      viralityThreshold,
      enablePitchShift,
      autoCaptions,
      defaultHashtags,
      defaultVisibility,
      autoPinnedComment,
      apiUrl,
    };
    localStorage.setItem("lychee_settings", JSON.stringify(config));
    setSavedSuccess(true);
    setTimeout(() => {
      setSavedSuccess(false);
      onClose();
    }, 1200);
  };

  const handleResetDefaults = () => {
    setAspectRatio("9:16");
    setClipDuration("30-60");
    setViralityThreshold(70);
    setEnablePitchShift(true);
    setAutoCaptions(true);
    setDefaultHashtags("#Shorts #Viral #LycheeAI #Trending");
    setDefaultVisibility("public");
    setAutoPinnedComment("🔥 Created with Lychee Shorts AI — subscribe for more daily clips!");
    setApiUrl("http://localhost:5000");
    if (setSelectedColor) setSelectedColor("rose");
    if (setDockPosition) setDockPosition("right");
    localStorage.removeItem("lychee_settings");
    localStorage.removeItem("lychee_theme_color");
    localStorage.removeItem("lychee_dock_pos");
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-xs p-4 animate-in fade-in duration-200">
      <div className="bg-white rounded-3xl shadow-2xl border border-slate-200 w-full max-w-3xl overflow-hidden flex flex-col max-h-[92vh]">
        {/* Modal Header */}
        <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50/70">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-2xl bg-gradient-to-tr from-rose-600 to-red-500 flex items-center justify-center shadow-md shadow-rose-500/20 text-white">
              <Settings size={20} />
            </div>
            <div>
              <h2 className="text-base font-black text-slate-900">Lychee Unified Control & Settings</h2>
              <p className="text-xs text-slate-400">Manage appearance, AI generation, social defaults, and API settings</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 text-slate-400 hover:text-slate-700 hover:bg-slate-100 rounded-xl transition-all"
          >
            <X size={18} />
          </button>
        </div>

        {/* Tab Navigation */}
        <div className="flex border-b border-slate-100 px-6 bg-white gap-1 overflow-x-auto">
          {[
            { id: "appearance", label: "Appearance & Theme", icon: Palette },
            { id: "ai",         label: "AI Video Engine",     icon: Sparkles },
            { id: "publishing", label: "Social Defaults",     icon: Share2 },
            { id: "system",     label: "System & API",        icon: Sliders },
            { id: "account",    label: "Account & Plan",      icon: User },
          ].map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              onClick={() => setActiveSubTab(id as any)}
              className={`flex items-center gap-2 py-3 px-3 text-xs font-bold border-b-2 transition-all shrink-0 ${
                activeSubTab === id
                  ? "border-rose-600 text-rose-600"
                  : "border-transparent text-slate-400 hover:text-slate-700"
              }`}
            >
              <Icon size={14} /> {label}
            </button>
          ))}
        </div>

        {/* Modal Body */}
        <div className="p-6 overflow-y-auto space-y-6 flex-1 text-slate-700">
          {/* TAB 1: APPEARANCE & THEME */}
          {activeSubTab === "appearance" && (
            <div className="space-y-5">
              {/* Floating Dock Position */}
              <div>
                <div className="flex items-center justify-between mb-2">
                  <div>
                    <h4 className="text-xs font-black text-slate-900 flex items-center gap-1.5">
                      <ArrowLeftRight size={14} className="text-rose-500" /> Floating Quick Widget Position
                    </h4>
                    <p className="text-[11px] text-slate-400">Choose which side of your screen the floating settings widget docks to</p>
                  </div>
                  <span className="text-xs font-bold text-slate-500 capitalize bg-slate-100 px-2.5 py-1 rounded-full">
                    Docked: {dockPosition}
                  </span>
                </div>

                <div className="grid grid-cols-2 gap-3 bg-slate-50 p-2 rounded-2xl border border-slate-200/80">
                  <button
                    type="button"
                    onClick={() => setDockPosition && setDockPosition("left")}
                    className={`flex items-center justify-center gap-2 py-3 rounded-xl text-xs font-bold transition-all ${
                      dockPosition === "left"
                        ? "bg-white text-rose-600 shadow-md ring-2 ring-rose-500/20 border border-rose-200"
                        : "text-slate-500 hover:text-slate-800 hover:bg-slate-100"
                    }`}
                  >
                    <span className="text-sm">⬅️</span> Float on Left Side
                  </button>
                  <button
                    type="button"
                    onClick={() => setDockPosition && setDockPosition("right")}
                    className={`flex items-center justify-center gap-2 py-3 rounded-xl text-xs font-bold transition-all ${
                      dockPosition === "right"
                        ? "bg-white text-rose-600 shadow-md ring-2 ring-rose-500/20 border border-rose-200"
                        : "text-slate-500 hover:text-slate-800 hover:bg-slate-100"
                    }`}
                  >
                    Float on Right Side <span className="text-sm">➡️</span>
                  </button>
                </div>
              </div>

              {/* Accent Color Themes */}
              <div>
                <h4 className="text-xs font-black text-slate-900 mb-1 flex items-center gap-1.5">
                  <Palette size={14} className="text-rose-500" /> Live Accent Color Themes
                </h4>
                <p className="text-[11px] text-slate-400 mb-3">Select your favorite primary color palette for highlights, buttons, and accents</p>

                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                  {(Object.keys(colorThemes) as ThemeColor[]).map((key) => {
                    const item = colorThemes[key];
                    const isSelected = selectedColor === key;
                    return (
                      <button
                        key={key}
                        type="button"
                        onClick={() => setSelectedColor && setSelectedColor(key)}
                        className={`flex items-center gap-3 p-3 rounded-2xl border transition-all text-left group ${
                          isSelected
                            ? "border-slate-900 bg-slate-900 text-white shadow-md ring-2 ring-slate-900/20"
                            : "border-slate-200 bg-white hover:border-slate-300 text-slate-700 hover:bg-slate-50"
                        }`}
                      >
                        <div
                          className={`h-7 w-7 rounded-full bg-gradient-to-tr ${item.gradient} shadow-sm shrink-0 flex items-center justify-center`}
                        >
                          {isSelected && <Check size={14} className="text-white stroke-[3]" />}
                        </div>
                        <div className="min-w-0">
                          <p className="text-xs font-black truncate">{item.name}</p>
                          <p className={`text-[10px] truncate ${isSelected ? "text-slate-300" : "text-slate-400"}`}>
                            {item.primary}
                          </p>
                        </div>
                      </button>
                    );
                  })}
                </div>
              </div>
            </div>
          )}

          {/* TAB 2: AI VIDEO ENGINE */}
          {activeSubTab === "ai" && (
            <div className="space-y-5">
              {/* Aspect Ratio */}
              <div>
                <label className="text-xs font-black text-slate-900 block mb-2">
                  Target Aspect Ratio Framing
                </label>
                <div className="grid grid-cols-3 gap-3">
                  {[
                    { id: "9:16", label: "9:16 Portrait", sub: "YouTube Shorts / Reels / TikTok" },
                    { id: "1:1",  label: "1:1 Square",    sub: "Instagram Feed / FB Posts" },
                    { id: "16:9", label: "16:9 Landscape",sub: "Standard YouTube & Web" },
                  ].map(opt => (
                    <button
                      key={opt.id}
                      type="button"
                      onClick={() => setAspectRatio(opt.id)}
                      className={`p-3 rounded-2xl border text-left transition-all ${
                        aspectRatio === opt.id
                          ? "border-rose-500 bg-rose-50/60 ring-2 ring-rose-500/20 text-rose-900"
                          : "border-slate-200 hover:border-slate-300 bg-white"
                      }`}
                    >
                      <p className="text-xs font-black">{opt.label}</p>
                      <p className="text-[10px] text-slate-400 mt-0.5">{opt.sub}</p>
                    </button>
                  ))}
                </div>
              </div>

              {/* Clip Duration Range */}
              <div>
                <label className="text-xs font-black text-slate-900 block mb-2">
                  Target Clip Duration Range
                </label>
                <div className="grid grid-cols-3 gap-3">
                  {[
                    { id: "15-30", label: "15 – 30 Seconds", sub: "Micro Viral Format" },
                    { id: "30-60", label: "30 – 60 Seconds", sub: "Optimal Engagement" },
                    { id: "60-90", label: "60 – 90 Seconds", sub: "Extended Storytelling" },
                  ].map(opt => (
                    <button
                      key={opt.id}
                      type="button"
                      onClick={() => setClipDuration(opt.id)}
                      className={`p-3 rounded-2xl border text-left transition-all ${
                        clipDuration === opt.id
                          ? "border-rose-500 bg-rose-50/60 ring-2 ring-rose-500/20 text-rose-900"
                          : "border-slate-200 hover:border-slate-300 bg-white"
                      }`}
                    >
                      <p className="text-xs font-black">{opt.label}</p>
                      <p className="text-[10px] text-slate-400 mt-0.5">{opt.sub}</p>
                    </button>
                  ))}
                </div>
              </div>

              {/* Virality Score Threshold */}
              <div className="bg-slate-50 p-4 rounded-2xl border border-slate-200/80">
                <div className="flex items-center justify-between mb-2">
                  <label className="text-xs font-black text-slate-900">
                    Minimum Virality Score Cutoff
                  </label>
                  <span className="text-xs font-black px-2.5 py-0.5 rounded-full bg-violet-100 text-violet-700 border border-violet-200">
                    🔥 {viralityThreshold}%
                  </span>
                </div>
                <input
                  type="range"
                  min={50}
                  max={95}
                  step={5}
                  value={viralityThreshold}
                  onChange={e => setViralityThreshold(Number(e.target.value))}
                  className="w-full accent-rose-600 h-1.5 bg-slate-200 rounded-lg cursor-pointer"
                />
                <p className="text-[10px] text-slate-400 mt-2">
                  Clips with estimated AI virality below this score will be excluded automatically from suggestions.
                </p>
              </div>

              {/* Anti-Copyright & Pitch Shift Toggle */}
              <div className="flex items-center justify-between p-4 rounded-2xl border border-slate-200 bg-white hover:border-slate-300 transition-all">
                <div className="flex items-start gap-3">
                  <div className="p-2 rounded-xl bg-amber-50 text-amber-600 border border-amber-200">
                    <Volume2 size={16} />
                  </div>
                  <div>
                    <h4 className="text-xs font-black text-slate-900">Anti-Copyright Pitch Shifting</h4>
                    <p className="text-[11px] text-slate-400 mt-0.5">
                      Apply micro audio pitch shift (±0.03 semi-tones) & frame micro-blur to bypass content filters
                    </p>
                  </div>
                </div>
                <input
                  type="checkbox"
                  checked={enablePitchShift}
                  onChange={e => setEnablePitchShift(e.target.checked)}
                  className="h-5 w-5 rounded-md accent-rose-600 cursor-pointer"
                />
              </div>
            </div>
          )}

          {/* TAB 3: PUBLISHING DEFAULTS */}
          {activeSubTab === "publishing" && (
            <div className="space-y-4">
              <div>
                <label className="text-xs font-black text-slate-900 block mb-1">
                  Default Hashtag Append
                </label>
                <input
                  type="text"
                  value={defaultHashtags}
                  onChange={e => setDefaultHashtags(e.target.value)}
                  placeholder="#Shorts #Viral #Trending"
                  className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                />
                <p className="text-[10px] text-slate-400 mt-1">Automatically appended to scheduled post titles</p>
              </div>

              <div>
                <label className="text-xs font-black text-slate-900 block mb-1">
                  Default Video Visibility
                </label>
                <select
                  value={defaultVisibility}
                  onChange={e => setDefaultVisibility(e.target.value)}
                  className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 font-bold"
                >
                  <option value="public">Public (Instant Discovery)</option>
                  <option value="unlisted">Unlisted (Review Before Public)</option>
                  <option value="private">Private (Restricted)</option>
                </select>
              </div>

              <div>
                <label className="text-xs font-black text-slate-900 block mb-1">
                  Auto First Pinned Comment
                </label>
                <textarea
                  rows={2}
                  value={autoPinnedComment}
                  onChange={e => setAutoPinnedComment(e.target.value)}
                  className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 resize-none"
                  placeholder="Drop a comment automatically on YouTube / Reels upon upload"
                />
              </div>
            </div>
          )}

          {/* TAB 4: SYSTEM & API */}
          {activeSubTab === "system" && (
            <div className="space-y-4">
              <div>
                <label className="text-xs font-black text-slate-900 block mb-1">
                  Backend API Host URL
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={apiUrl}
                    onChange={e => setApiUrl(e.target.value)}
                    className="flex-1 px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono text-slate-800 focus:outline-none focus:border-rose-400"
                  />
                  <a
                    href={`${apiUrl}/hangfire`}
                    target="_blank"
                    rel="noreferrer"
                    className="px-4 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl flex items-center gap-1.5 transition-all border border-slate-200 shrink-0"
                  >
                    <ExternalLink size={13} /> Hangfire Jobs
                  </a>
                </div>
              </div>

              <div className="p-4 rounded-2xl bg-emerald-50 border border-emerald-200 flex items-center justify-between">
                <div className="flex items-center gap-2.5">
                  <div className="h-3 w-3 rounded-full bg-emerald-500 animate-ping" />
                  <div>
                    <h4 className="text-xs font-black text-emerald-900">API Connection Active</h4>
                    <p className="text-[10px] text-emerald-600">Lychee Publisher Core connected on {apiUrl}</p>
                  </div>
                </div>
                <span className="text-[10px] font-bold bg-emerald-200/60 text-emerald-800 px-2.5 py-1 rounded-full">
                  Status: 200 OK
                </span>
              </div>
            </div>
          )}

          {/* TAB 5: ACCOUNT & PLAN */}
          {activeSubTab === "account" && (
            <div className="space-y-4">
              {currentUser ? (
                <div className="p-5 rounded-2xl bg-slate-50 border border-slate-200 space-y-3">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-2xl bg-rose-950 border border-rose-800/60 flex items-center justify-center text-sm font-black text-rose-300">
                      {currentUser.displayName ? currentUser.displayName.slice(0, 2).toUpperCase() : "US"}
                    </div>
                    <div>
                      <h4 className="text-sm font-black text-slate-900">{currentUser.displayName || "User"}</h4>
                      <p className="text-xs text-slate-500">{currentUser.email}</p>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-3 pt-3 border-t border-slate-200 text-xs">
                    <div>
                      <span className="text-slate-400 text-[10px] font-bold uppercase">Role</span>
                      <p className="font-bold text-slate-800 uppercase">{currentUser.role || "User"}</p>
                    </div>
                    <div>
                      <span className="text-slate-400 text-[10px] font-bold uppercase">Tier</span>
                      <p className="font-bold text-emerald-600">{currentUser.tier || "Enterprise"}</p>
                    </div>
                  </div>
                </div>
              ) : (
                <p className="text-xs text-slate-400">Not authenticated</p>
              )}

              {usage && (
                <div className="p-4 rounded-2xl bg-slate-50 border border-slate-200 text-xs">
                  <div className="flex items-center justify-between mb-1.5">
                    <span className="font-bold text-slate-700">Monthly Videos Generated</span>
                    <span className="font-bold text-rose-600">{usage.videosGeneratedThisMonth} / {usage.monthlyVideoLimit || "Unlimited"}</span>
                  </div>
                  <div className="w-full h-2 bg-slate-200 rounded-full overflow-hidden">
                    <div
                      className="h-full bg-rose-600 rounded-full transition-all"
                      style={{ width: `${Math.min(100, (usage.videosGeneratedThisMonth / (usage.monthlyVideoLimit || 100)) * 100)}%` }}
                    />
                  </div>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Modal Footer */}
        <div className="px-6 py-4 border-t border-slate-100 bg-slate-50 flex items-center justify-between">
          <button
            type="button"
            onClick={handleResetDefaults}
            className="flex items-center gap-1.5 text-xs font-bold text-slate-500 hover:text-slate-700 transition-colors"
          >
            <RotateCcw size={13} /> Reset Defaults
          </button>

          <div className="flex items-center gap-2">
            {savedSuccess && (
              <span className="text-xs font-bold text-emerald-600 flex items-center gap-1 animate-in fade-in">
                <CheckCircle2 size={14} /> Saved!
              </span>
            )}
            <button
              type="button"
              onClick={handleSaveSettings}
              className="px-5 py-2.5 bg-gradient-to-r from-rose-600 to-red-500 hover:from-rose-500 hover:to-red-400 text-white text-xs font-black rounded-xl shadow-md shadow-rose-600/20 transition-all"
            >
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

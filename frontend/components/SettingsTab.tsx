"use client";

import React, { useState, useEffect } from "react";
import {
  Settings, Sparkles, Palette, Share2, Sliders, User,
  CheckCircle2, RotateCcw, Volume2, Film, Check, ArrowLeftRight,
  ExternalLink, Shield, Cpu, RefreshCw, Zap, SlidersHorizontal,
  Layers, HardDrive, Bell, Key, Globe, Eye, EyeOff, Layout,
  Sliders as SlidersIcon, Monitor, Sun, Moon, ToggleLeft, ToggleRight
} from "lucide-react";
import { colorThemes, ThemeColor, DockPosition } from "./FloatingThemeWidget";

interface SettingsTabProps {
  currentUser?: any;
  usage?: any;
  dockPosition: DockPosition;
  setDockPosition: (pos: DockPosition) => void;
  selectedColor: ThemeColor;
  setSelectedColor: (c: ThemeColor) => void;
  showFloatingWidget?: boolean;
  setShowFloatingWidget?: (show: boolean) => void;
}

export const SettingsTab: React.FC<SettingsTabProps> = ({
  currentUser,
  usage,
  dockPosition,
  setDockPosition,
  selectedColor,
  setSelectedColor,
  showFloatingWidget = true,
  setShowFloatingWidget,
}) => {
  const [activeSection, setActiveSection] = useState<"theme" | "ai" | "publishing" | "system" | "account">("theme");
  const [savedNotice, setSavedNotice] = useState(false);
  const [apiPingStatus, setApiPingStatus] = useState<"checking" | "online" | "error">("online");

  // AI & Video Engine Preferences
  const [aspectRatio, setAspectRatio] = useState("9:16");
  const [clipDuration, setClipDuration] = useState("30-60");
  const [viralityThreshold, setViralityThreshold] = useState(70);
  const [enablePitchShift, setEnablePitchShift] = useState(true);
  const [autoCaptions, setAutoCaptions] = useState(true);
  const [captionStyle, setCaptionStyle] = useState<"karaoke" | "bold" | "minimal">("karaoke");
  const [autoMusicDucking, setAutoMusicDucking] = useState(true);

  // Social Publishing Rules
  const [defaultHashtags, setDefaultHashtags] = useState("#Shorts #Viral #LycheeAI #Trending");
  const [defaultVisibility, setDefaultVisibility] = useState("public");
  const [autoPinnedComment, setAutoPinnedComment] = useState("🔥 Created with Lychee Shorts AI — subscribe for more daily viral clips!");
  const [defaultCategory, setDefaultCategory] = useState("Entertainment");

  // System & API Settings
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
          if (parsed.captionStyle) setCaptionStyle(parsed.captionStyle);
          if (parsed.autoMusicDucking !== undefined) setAutoMusicDucking(parsed.autoMusicDucking);
          if (parsed.defaultHashtags) setDefaultHashtags(parsed.defaultHashtags);
          if (parsed.defaultVisibility) setDefaultVisibility(parsed.defaultVisibility);
          if (parsed.autoPinnedComment) setAutoPinnedComment(parsed.autoPinnedComment);
          if (parsed.defaultCategory) setDefaultCategory(parsed.defaultCategory);
          if (parsed.apiUrl) setApiUrl(parsed.apiUrl);
        } catch {}
      }
    }
  }, []);

  const handleSaveAll = () => {
    const config = {
      aspectRatio,
      clipDuration,
      viralityThreshold,
      enablePitchShift,
      autoCaptions,
      captionStyle,
      autoMusicDucking,
      defaultHashtags,
      defaultVisibility,
      autoPinnedComment,
      defaultCategory,
      apiUrl,
    };
    localStorage.setItem("lychee_settings", JSON.stringify(config));
    setSavedNotice(true);
    setTimeout(() => setSavedNotice(false), 2500);
  };

  const handleResetToDefaults = () => {
    setAspectRatio("9:16");
    setClipDuration("30-60");
    setViralityThreshold(70);
    setEnablePitchShift(true);
    setAutoCaptions(true);
    setCaptionStyle("karaoke");
    setAutoMusicDucking(true);
    setDefaultHashtags("#Shorts #Viral #LycheeAI #Trending");
    setDefaultVisibility("public");
    setAutoPinnedComment("🔥 Created with Lychee Shorts AI — subscribe for more daily viral clips!");
    setDefaultCategory("Entertainment");
    setApiUrl("http://localhost:5000");
    setSelectedColor("rose");
    setDockPosition("right");
    if (setShowFloatingWidget) setShowFloatingWidget(true);
    localStorage.removeItem("lychee_settings");
    localStorage.removeItem("lychee_theme_color");
    localStorage.removeItem("lychee_dock_pos");
    localStorage.removeItem("lychee_show_floating");
    setSavedNotice(true);
    setTimeout(() => setSavedNotice(false), 2000);
  };

  const checkApiHealth = async () => {
    setApiPingStatus("checking");
    try {
      const res = await fetch(`${apiUrl}/api/v1/health`);
      if (res.ok) {
        setApiPingStatus("online");
      } else {
        setApiPingStatus("error");
      }
    } catch {
      setApiPingStatus("error");
    }
  };

  const currentThemeObj = colorThemes[selectedColor] || colorThemes.rose;

  return (
    <div className="flex-1 overflow-y-auto bg-slate-50 p-6 md:p-8 space-y-6">
      {/* Top Banner / Title Header */}
      <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 flex flex-col md:flex-row items-start md:items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <div className="h-14 w-14 rounded-2xl bg-gradient-to-tr from-rose-600 via-rose-500 to-red-500 flex items-center justify-center text-white shadow-lg shadow-rose-500/25">
            <Cpu size={28} />
          </div>
          <div>
            <div className="flex items-center gap-2">
              <h1 className="text-xl font-black text-slate-900 tracking-tight">AI Engine & Platform Settings</h1>
              <span className="bg-rose-100 text-rose-700 text-[10px] font-black uppercase px-2 py-0.5 rounded-full border border-rose-200">
                Core Engine
              </span>
            </div>
            <p className="text-xs text-slate-400 mt-0.5">
              Live theme customizer, floating controls dock, AI virality tuning, social defaults, and API engine
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2 self-end md:self-auto">
          <button
            type="button"
            onClick={handleResetToDefaults}
            className="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 text-xs font-bold transition-all flex items-center gap-1.5"
          >
            <RotateCcw size={13} /> Reset
          </button>
          <button
            type="button"
            onClick={handleSaveAll}
            className="px-5 py-2.5 rounded-xl bg-gradient-to-r from-rose-600 to-red-500 hover:from-rose-500 hover:to-red-400 text-white text-xs font-black shadow-md shadow-rose-600/20 transition-all flex items-center gap-1.5"
          >
            {savedNotice ? <CheckCircle2 size={14} className="text-white" /> : <Zap size={14} />}
            {savedNotice ? "Settings Saved!" : "Save All Changes"}
          </button>
        </div>
      </div>

      {/* Main Settings Layout (Side Nav + Content Panels) */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {/* Navigation Sidebar */}
        <div className="md:col-span-1 space-y-2">
          <div className="bg-white rounded-2xl border border-slate-200 shadow-xs p-2 space-y-1">
            {[
              { id: "theme",      label: "UI & Theme Customizer", icon: Palette,  desc: "Colors & Left/Right dock" },
              { id: "ai",         label: "AI Video & Virality",   icon: Sparkles, desc: "Aspect, timestamps & filters" },
              { id: "publishing", label: "Publishing Defaults",   icon: Share2,   desc: "Tags, privacy & comments" },
              { id: "system",     label: "API & Background Jobs", icon: Sliders,  desc: "Host endpoints & Hangfire" },
              { id: "account",    label: "Account & Plan Limits", icon: User,     desc: "Tier, usage & credits" },
            ].map(({ id, label, icon: Icon, desc }) => (
              <button
                key={id}
                onClick={() => setActiveSection(id as any)}
                className={`w-full flex items-start gap-3 p-3 rounded-xl text-left transition-all ${
                  activeSection === id
                    ? "bg-rose-50 text-rose-900 border border-rose-200 shadow-xs"
                    : "text-slate-600 hover:bg-slate-50 hover:text-slate-900 border border-transparent"
                }`}
              >
                <div className={`p-2 rounded-lg mt-0.5 ${activeSection === id ? "bg-rose-600 text-white" : "bg-slate-100 text-slate-500"}`}>
                  <Icon size={16} />
                </div>
                <div className="min-w-0">
                  <p className="text-xs font-black truncate">{label}</p>
                  <p className="text-[10px] text-slate-400 truncate mt-0.5">{desc}</p>
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Content Area */}
        <div className="md:col-span-3 space-y-6">
          {/* SECTION 1: THEME & UI CUSTOMIZER */}
          {activeSection === "theme" && (
            <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 space-y-6">
              <div>
                <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                  <Palette size={16} className="text-rose-500" /> UI Customization & Theme Center
                </h3>
                <p className="text-xs text-slate-400 mt-0.5">Control live color themes, screen positions, and layout preferences</p>
              </div>

              {/* Floating Widget Toggle & Dock Position */}
              <div className="space-y-4 bg-slate-50 p-5 rounded-2xl border border-slate-200/80">
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="text-xs font-black text-slate-900 flex items-center gap-1.5">
                      <Layout size={14} className="text-rose-500" /> Floating Quick Customizer Widget
                    </h4>
                    <p className="text-[11px] text-slate-400">Show or hide the floating tab on the side of your screen</p>
                  </div>
                  {setShowFloatingWidget && (
                    <button
                      type="button"
                      onClick={() => {
                        const next = !showFloatingWidget;
                        setShowFloatingWidget(next);
                        localStorage.setItem("lychee_show_floating", String(next));
                      }}
                      className={`px-3 py-1.5 rounded-xl text-xs font-bold transition-all flex items-center gap-1.5 ${
                        showFloatingWidget
                          ? "bg-rose-600 text-white shadow-xs"
                          : "bg-slate-200 text-slate-600 hover:bg-slate-300"
                      }`}
                    >
                      {showFloatingWidget ? <Eye size={13} /> : <EyeOff size={13} />}
                      {showFloatingWidget ? "Visible" : "Hidden"}
                    </button>
                  )}
                </div>

                {showFloatingWidget && (
                  <div className="space-y-2 pt-3 border-t border-slate-200">
                    <div className="flex items-center justify-between">
                      <label className="text-xs font-black text-slate-800 flex items-center gap-1.5">
                        <ArrowLeftRight size={13} className="text-rose-500" /> Floating Edge Dock Side
                      </label>
                      <span className="text-[10px] font-black uppercase tracking-wider bg-rose-100 text-rose-700 px-2 py-0.5 rounded-full">
                        Docked: {dockPosition}
                      </span>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                      <button
                        type="button"
                        onClick={() => {
                          setDockPosition("left");
                          localStorage.setItem("lychee_dock_pos", "left");
                        }}
                        className={`flex items-center justify-center gap-2 py-3 rounded-xl text-xs font-black transition-all ${
                          dockPosition === "left"
                            ? "bg-white text-rose-600 shadow-md ring-2 ring-rose-500/20 border border-rose-200"
                            : "text-slate-600 hover:text-slate-900 bg-slate-100"
                        }`}
                      >
                        <span>⬅️</span> Float on Left Edge
                      </button>
                      <button
                        type="button"
                        onClick={() => {
                          setDockPosition("right");
                          localStorage.setItem("lychee_dock_pos", "right");
                        }}
                        className={`flex items-center justify-center gap-2 py-3 rounded-xl text-xs font-black transition-all ${
                          dockPosition === "right"
                            ? "bg-white text-rose-600 shadow-md ring-2 ring-rose-500/20 border border-rose-200"
                            : "text-slate-600 hover:text-slate-900 bg-slate-100"
                        }`}
                      >
                        Float on Right Edge <span>➡️</span>
                      </button>
                    </div>
                  </div>
                )}
              </div>

              {/* Accent Color Themes */}
              <div className="space-y-3">
                <h4 className="text-xs font-black text-slate-900 flex items-center gap-1.5">
                  <Sparkles size={14} className="text-rose-500" /> Live Accent Color Palettes (6 Curated Themes)
                </h4>
                <p className="text-[11px] text-slate-400">Click any palette to instantly apply colors across all badges, highlights, and buttons</p>

                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                  {(Object.keys(colorThemes) as ThemeColor[]).map((key) => {
                    const item = colorThemes[key];
                    const isSelected = selectedColor === key;
                    return (
                      <button
                        key={key}
                        type="button"
                        onClick={() => {
                          setSelectedColor(key);
                          localStorage.setItem("lychee_theme_color", key);
                          document.documentElement.style.setProperty("--theme-primary", item.primary);
                          document.documentElement.setAttribute("data-theme-color", key);
                        }}
                        className={`flex items-center gap-3 p-3.5 rounded-2xl border transition-all text-left ${
                          isSelected
                            ? "border-slate-900 bg-slate-900 text-white shadow-md ring-2 ring-slate-900/20"
                            : "border-slate-200 bg-white hover:border-slate-300 text-slate-700 hover:bg-slate-50"
                        }`}
                      >
                        <div
                          className={`h-8 w-8 rounded-full bg-gradient-to-tr ${item.gradient} shadow-sm shrink-0 flex items-center justify-center`}
                        >
                          {isSelected && <Check size={16} className="text-white stroke-[3]" />}
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

              {/* Live Preview Card */}
              <div className="p-5 rounded-2xl border border-slate-200 bg-slate-50 space-y-3">
                <span className="text-[11px] font-black text-slate-700 uppercase tracking-wider block">
                  🎨 Live Theme Preview ({currentThemeObj.name})
                </span>
                <div className="flex items-center gap-3 flex-wrap">
                  <button
                    type="button"
                    className={`px-4 py-2 rounded-xl text-white text-xs font-black bg-gradient-to-r ${currentThemeObj.gradient} shadow-md`}
                  >
                    Primary Action Button
                  </button>
                  <span className={`text-xs font-black px-3 py-1 rounded-full ${currentThemeObj.bgLight} ${currentThemeObj.accent} border ${currentThemeObj.borderLight}`}>
                    🔥 Active Badge
                  </span>
                  <span className="text-xs text-slate-500 font-bold">
                    Primary Accent: <span className="font-mono">{currentThemeObj.primary}</span>
                  </span>
                </div>
              </div>
            </div>
          )}

          {/* SECTION 2: AI VIDEO & VIRALITY */}
          {activeSection === "ai" && (
            <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 space-y-6">
              <div>
                <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                  <Sparkles size={16} className="text-rose-500" /> AI Video Clipping & Virality Parameters
                </h3>
                <p className="text-xs text-slate-400 mt-0.5">Define automated clipping rules and virality detection thresholds</p>
              </div>

              {/* Aspect Ratio Framing */}
              <div className="space-y-2">
                <label className="text-xs font-black text-slate-800 block">Default Aspect Ratio Framing</label>
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                  {[
                    { id: "9:16", title: "9:16 Portrait", sub: "YouTube Shorts / Reels / TikTok (Default)" },
                    { id: "1:1",  title: "1:1 Square",    sub: "Instagram Feed & Facebook Square" },
                    { id: "16:9", title: "16:9 Landscape",sub: "Standard Landscape YouTube" },
                  ].map(item => (
                    <button
                      key={item.id}
                      type="button"
                      onClick={() => setAspectRatio(item.id)}
                      className={`p-4 rounded-2xl border text-left transition-all ${
                        aspectRatio === item.id
                          ? "border-rose-500 bg-rose-50/60 ring-2 ring-rose-500/20 text-rose-900 shadow-xs"
                          : "border-slate-200 hover:border-slate-300 bg-white"
                      }`}
                    >
                      <p className="text-xs font-black">{item.title}</p>
                      <p className="text-[10px] text-slate-400 mt-1">{item.sub}</p>
                    </button>
                  ))}
                </div>
              </div>

              {/* Clip Duration Range */}
              <div className="space-y-2">
                <label className="text-xs font-black text-slate-800 block">AI Timestamp Duration Window</label>
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                  {[
                    { id: "15-30", title: "15 – 30 Seconds", sub: "Fast-Paced Micro Viral Scenes" },
                    { id: "30-60", title: "30 – 60 Seconds", sub: "Balanced Context & Peak Retention" },
                    { id: "60-90", title: "60 – 90 Seconds", sub: "Deep Narrative / Storytelling" },
                  ].map(item => (
                    <button
                      key={item.id}
                      type="button"
                      onClick={() => setClipDuration(item.id)}
                      className={`p-4 rounded-2xl border text-left transition-all ${
                        clipDuration === item.id
                          ? "border-rose-500 bg-rose-50/60 ring-2 ring-rose-500/20 text-rose-900 shadow-xs"
                          : "border-slate-200 hover:border-slate-300 bg-white"
                      }`}
                    >
                      <p className="text-xs font-black">{item.title}</p>
                      <p className="text-[10px] text-slate-400 mt-1">{item.sub}</p>
                    </button>
                  ))}
                </div>
              </div>

              {/* Virality Threshold Slider */}
              <div className="bg-slate-50 p-5 rounded-2xl border border-slate-200/80 space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-xs font-black text-slate-900 block">Virality Score Filter Cutoff</label>
                    <p className="text-[10px] text-slate-400">Exclude candidate scenes below this AI virality probability</p>
                  </div>
                  <span className="text-xs font-black px-3 py-1 rounded-full bg-violet-100 text-violet-700 border border-violet-200">
                    🔥 {viralityThreshold}% Minimum Score
                  </span>
                </div>
                <input
                  type="range"
                  min={50}
                  max={95}
                  step={5}
                  value={viralityThreshold}
                  onChange={e => setViralityThreshold(Number(e.target.value))}
                  className="w-full accent-rose-600 h-2 bg-slate-200 rounded-lg cursor-pointer"
                />
              </div>

              {/* Anti-Copyright & Audio Pitch Shift */}
              <div className="p-4 rounded-2xl border border-slate-200 bg-slate-50/50 flex items-center justify-between">
                <div className="flex items-start gap-3">
                  <div className="p-2.5 rounded-xl bg-amber-100 text-amber-700 border border-amber-200">
                    <Volume2 size={18} />
                  </div>
                  <div>
                    <h4 className="text-xs font-black text-slate-900">Anti-Copyright Pitch Shifting (FFmpeg)</h4>
                    <p className="text-[11px] text-slate-400 mt-0.5">
                      Slightly alters audio pitch frequency (±0.03 semitones) to bypass algorithmic duplicate detection
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

              {/* Auto Captions Style */}
              <div className="p-4 rounded-2xl border border-slate-200 bg-slate-50/50 space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="text-xs font-black text-slate-900">Auto Captions & Subtitles</h4>
                    <p className="text-[11px] text-slate-400">Generate on-screen animated word highlights</p>
                  </div>
                  <input
                    type="checkbox"
                    checked={autoCaptions}
                    onChange={e => setAutoCaptions(e.target.checked)}
                    className="h-5 w-5 rounded-md accent-rose-600 cursor-pointer"
                  />
                </div>
                {autoCaptions && (
                  <div className="grid grid-cols-3 gap-2 pt-2 border-t border-slate-200">
                    {[
                      { id: "karaoke", label: "Karaoke Highlight" },
                      { id: "bold",    label: "Bold Cinematic" },
                      { id: "minimal", label: "Minimalist Sub" },
                    ].map(style => (
                      <button
                        key={style.id}
                        type="button"
                        onClick={() => setCaptionStyle(style.id as any)}
                        className={`py-2 px-3 rounded-xl text-xs font-bold border transition-all ${
                          captionStyle === style.id
                            ? "bg-white border-rose-500 text-rose-600 ring-1 ring-rose-500/30 shadow-xs"
                            : "bg-slate-100 border-transparent text-slate-600 hover:bg-slate-200"
                        }`}
                      >
                        {style.label}
                      </button>
                    ))}
                  </div>
                )}
              </div>
            </div>
          )}

          {/* SECTION 3: PUBLISHING DEFAULTS */}
          {activeSection === "publishing" && (
            <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 space-y-5">
              <div>
                <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                  <Share2 size={16} className="text-rose-500" /> Social Publishing Automation Defaults
                </h3>
                <p className="text-xs text-slate-400 mt-0.5">Pre-fill tags, privacy states, and comments for scheduled clips</p>
              </div>

              <div>
                <label className="text-xs font-black text-slate-800 block mb-1">
                  Default Hashtag Append Template
                </label>
                <input
                  type="text"
                  value={defaultHashtags}
                  onChange={e => setDefaultHashtags(e.target.value)}
                  placeholder="#Shorts #Viral #LycheeAI #Trending"
                  className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                />
                <p className="text-[10px] text-slate-400 mt-1">Automatically added at the end of generated short descriptions and titles</p>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className="text-xs font-black text-slate-800 block mb-1">
                    Default Video Visibility
                  </label>
                  <select
                    value={defaultVisibility}
                    onChange={e => setDefaultVisibility(e.target.value)}
                    className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 font-bold"
                  >
                    <option value="public">Public (Immediate Virality)</option>
                    <option value="unlisted">Unlisted (Review First)</option>
                    <option value="private">Private (Only You)</option>
                  </select>
                </div>

                <div>
                  <label className="text-xs font-black text-slate-800 block mb-1">
                    Default Primary Category
                  </label>
                  <select
                    value={defaultCategory}
                    onChange={e => setDefaultCategory(e.target.value)}
                    className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 font-bold"
                  >
                    <option value="Entertainment">Entertainment</option>
                    <option value="Education">Education & How-To</option>
                    <option value="Gaming">Gaming & Streaming</option>
                    <option value="Tech">Science & Technology</option>
                    <option value="Comedy">Comedy & Skits</option>
                    <option value="Lifestyle">People & Blogs</option>
                  </select>
                </div>
              </div>

              <div>
                <label className="text-xs font-black text-slate-800 block mb-1">
                  Automatic First Pinned Comment
                </label>
                <textarea
                  rows={3}
                  value={autoPinnedComment}
                  onChange={e => setAutoPinnedComment(e.target.value)}
                  className="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 resize-none"
                  placeholder="Drop a call-to-action comment automatically upon publishing"
                />
              </div>
            </div>
          )}

          {/* SECTION 4: SYSTEM & API */}
          {activeSection === "system" && (
            <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 space-y-5">
              <div>
                <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                  <Sliders size={16} className="text-rose-500" /> Backend Infrastructure & Worker Pipeline
                </h3>
                <p className="text-xs text-slate-400 mt-0.5">API endpoints, Hangfire minutely publishing queue, and system health</p>
              </div>

              <div>
                <label className="text-xs font-black text-slate-800 block mb-1">
                  Backend API Host URL
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={apiUrl}
                    onChange={e => setApiUrl(e.target.value)}
                    className="flex-1 px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono text-slate-800 focus:outline-none focus:border-rose-400"
                  />
                  <button
                    type="button"
                    onClick={checkApiHealth}
                    className="px-4 py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl flex items-center gap-1.5 transition-all border border-slate-200 shrink-0"
                  >
                    <RefreshCw size={13} className={apiPingStatus === "checking" ? "animate-spin" : ""} /> Test API Ping
                  </button>
                </div>
              </div>

              <div className={`p-4 rounded-2xl border flex items-center justify-between ${
                apiPingStatus === "online" ? "bg-emerald-50 border-emerald-200" : "bg-rose-50 border-rose-200"
              }`}>
                <div className="flex items-center gap-2.5">
                  <div className={`h-3 w-3 rounded-full ${apiPingStatus === "online" ? "bg-emerald-500 animate-ping" : "bg-rose-500"}`} />
                  <div>
                    <h4 className={`text-xs font-black ${apiPingStatus === "online" ? "text-emerald-900" : "text-rose-900"}`}>
                      {apiPingStatus === "online" ? "Core API Active & Responding" : "Core API Offline / Unreachable"}
                    </h4>
                    <p className={`text-[10px] ${apiPingStatus === "online" ? "text-emerald-600" : "text-rose-600"}`}>
                      Host: {apiUrl}
                    </p>
                  </div>
                </div>
                <span className={`text-[10px] font-bold px-2.5 py-1 rounded-full ${
                  apiPingStatus === "online" ? "bg-emerald-200/60 text-emerald-800" : "bg-rose-200 text-rose-800"
                }`}>
                  HTTP 200 OK
                </span>
              </div>

              {/* Background Hangfire Worker Link */}
              <div className="p-4 rounded-2xl bg-slate-50 border border-slate-200 flex items-center justify-between">
                <div>
                  <h4 className="text-xs font-black text-slate-900">Hangfire Minutely Recurring Publisher</h4>
                  <p className="text-[11px] text-slate-400 mt-0.5">
                    Processes `PublishScheduledClipsJob` every minute for automated multi-channel posting
                  </p>
                </div>
                <a
                  href={`${apiUrl}/hangfire`}
                  target="_blank"
                  rel="noreferrer"
                  className="px-4 py-2 bg-slate-900 text-white hover:bg-slate-800 text-xs font-bold rounded-xl flex items-center gap-1.5 transition-all shadow-xs"
                >
                  <ExternalLink size={13} /> Open Hangfire
                </a>
              </div>
            </div>
          )}

          {/* SECTION 5: ACCOUNT & PLAN */}
          {activeSection === "account" && (
            <div className="bg-white rounded-3xl border border-slate-200 shadow-xs p-6 space-y-5">
              <div>
                <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                  <User size={16} className="text-rose-500" /> Account Profile & Monthly Quotas
                </h3>
                <p className="text-xs text-slate-400 mt-0.5">Your credentials, role permissions, and active video creation limits</p>
              </div>

              {currentUser && (
                <div className="p-5 rounded-2xl bg-slate-50 border border-slate-200 space-y-4">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-2xl bg-rose-950 border border-rose-800/60 flex items-center justify-center text-sm font-black text-rose-300">
                      {currentUser.displayName ? currentUser.displayName.slice(0, 2).toUpperCase() : "US"}
                    </div>
                    <div>
                      <h4 className="text-sm font-black text-slate-900">{currentUser.displayName || "Admin"}</h4>
                      <p className="text-xs text-slate-500">{currentUser.email}</p>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-3 pt-3 border-t border-slate-200 text-xs">
                    <div>
                      <span className="text-slate-400 text-[10px] font-bold uppercase">Role Elevation</span>
                      <p className="font-bold text-slate-800 uppercase mt-0.5">{currentUser.role || "Admin"}</p>
                    </div>
                    <div>
                      <span className="text-slate-400 text-[10px] font-bold uppercase">Subscription Tier</span>
                      <p className="font-bold text-emerald-600 mt-0.5">{currentUser.tier || "Enterprise Plan"}</p>
                    </div>
                  </div>
                </div>
              )}

              {usage && (
                <div className="p-5 rounded-2xl bg-slate-50 border border-slate-200 space-y-3">
                  <div className="flex items-center justify-between text-xs">
                    <span className="font-bold text-slate-700">Monthly Videos Generated</span>
                    <span className="font-bold text-rose-600">
                      {usage.videosGeneratedThisMonth} / {usage.monthlyVideoLimit || "Unlimited"}
                    </span>
                  </div>
                  <div className="w-full h-2.5 bg-slate-200 rounded-full overflow-hidden">
                    <div
                      className="h-full bg-gradient-to-r from-rose-600 to-red-500 rounded-full transition-all"
                      style={{
                        width: `${Math.min(100, (usage.videosGeneratedThisMonth / (usage.monthlyVideoLimit || 100)) * 100)}%`,
                      }}
                    />
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

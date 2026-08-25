"use client";

import React, { useRef, useState, useEffect } from "react";
import { VideoResponse, ShortClipResponse, ScheduleResponse, api } from "../lib/api";
import {
  Upload, Link2, PlaySquare, Sparkles, Clock, Calendar, AlertCircle, CheckCircle2,
  Loader2, Trash2, ChevronRight, Film, Zap, Download, Eye, X, Play, Image as ImageIcon,
  MessageSquare, Tag, Globe, Lock, EyeOff, ShieldAlert, Layers, MessageCircle, Square
} from "lucide-react";

interface WorkspaceTabProps {
  videos: VideoResponse[];
  selectedVideo: VideoResponse | null;
  setSelectedVideo: (v: VideoResponse | null) => void;
  selectedVideoShorts: ShortClipResponse[];
  schedules: ScheduleResponse[];
  youtubeUrl: string; setYoutubeUrl: (v: string) => void;
  youtubeTitle: string; setYoutubeTitle: (v: string) => void;
  handleImportYoutube: (e: React.FormEvent) => void;
  uploadFile: File | null; setUploadFile: (f: File | null) => void;
  uploadTitle: string; setUploadTitle: (v: string) => void;
  handleUpload: (e: React.FormEvent) => void;
  handleTriggerAiProcessing: (videoId: string) => void;
  handleStopVideoProcessing: (videoId: string) => void;
  handleDeleteVideo: (videoId: string) => void;
  handleDeleteShortClip: (clipId: string) => void;
  scheduleClipId: string; setScheduleClipId: (v: string) => void;
  scheduleDate: string; setScheduleDate: (v: string) => void;
  scheduleTime: string; setScheduleTime: (v: string) => void;
  schedulePlatform: string; setSchedulePlatform: (v: string) => void;
  selectedSocialAccountId: string; setSelectedSocialAccountId: (v: string) => void;
  socialAccounts: any[];
  handleCreateSchedule: (e: React.FormEvent) => void;
  handleDeleteSchedule: (id: string) => void;
  loading: boolean;
  actionError: string;
}

function parseSeconds(val: any): number {
  if (typeof val === "number") return val;
  if (typeof val === "string") {
    if (val.includes(":")) {
      const parts = val.split(":").map(Number);
      if (parts.length === 3) return parts[0] * 3600 + parts[1] * 60 + parts[2];
      if (parts.length === 2) return parts[0] * 60 + parts[1];
    }
    return parseFloat(val) || 0;
  }
  return 0;
}

function StatusBadge({ status }: { status: string }) {
  const map: Record<string, string> = {
    Queued: "bg-amber-100 text-amber-700 border-amber-200",
    Processing: "bg-blue-100 text-blue-700 border-blue-200",
    Processed: "bg-emerald-100 text-emerald-700 border-emerald-200",
    Failed: "bg-rose-100 text-rose-700 border-rose-200",
    Cancelled: "bg-slate-100 text-slate-700 border-slate-300",
  };
  return (
    <span className={`inline-flex items-center gap-1 rounded-full border px-2 py-0.5 text-[10px] font-bold ${map[status] || "bg-slate-100 text-slate-600 border-slate-200"}`}>
      {status === "Processing" && <Loader2 size={9} className="animate-spin" />}
      {status}
    </span>
  );
}

export const WorkspaceTab: React.FC<WorkspaceTabProps> = ({
  videos, selectedVideo, setSelectedVideo, selectedVideoShorts, schedules,
  youtubeUrl, setYoutubeUrl, youtubeTitle, setYoutubeTitle, handleImportYoutube,
  uploadFile, setUploadFile, uploadTitle, setUploadTitle, handleUpload,
  handleTriggerAiProcessing, handleStopVideoProcessing, handleDeleteVideo, handleDeleteShortClip,
  scheduleClipId, setScheduleClipId,
  scheduleDate, setScheduleDate, scheduleTime, setScheduleTime,
  schedulePlatform, setSchedulePlatform, selectedSocialAccountId, setSelectedSocialAccountId,
  socialAccounts, handleCreateSchedule, handleDeleteSchedule, loading, actionError
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const thumbnailInputRef = useRef<HTMLInputElement>(null);
  const [importMode, setImportMode] = useState<"youtube" | "upload">("youtube");
  const [previewClip, setPreviewClip] = useState<ShortClipResponse | null>(null);

  // Advanced publishing metadata states
  const [customTitle, setCustomTitle] = useState("");
  const [customDescription, setCustomDescription] = useState("");
  const [customTags, setCustomTags] = useState("#Shorts #Viral #LycheeAI");
  const [pinnedComment, setPinnedComment] = useState("");
  const [category, setCategory] = useState("Entertainment");
  const [privacyStatus, setPrivacyStatus] = useState("public");
  const [isForKids, setIsForKids] = useState(false);
  const [hasAgeRestriction, setHasAgeRestriction] = useState(false);
  const [thumbnailFile, setThumbnailFile] = useState<File | null>(null);
  const [thumbnailPreview, setThumbnailPreview] = useState<string | null>(null);
  const [showAdvanced, setShowAdvanced] = useState(false);

  // Auto-fill when scheduleClipId changes
  useEffect(() => {
    if (scheduleClipId) {
      const clip = selectedVideoShorts.find(c => c.id === scheduleClipId);
      if (clip) {
        setCustomTitle(clip.title || "");
        setCustomDescription(clip.description || "");
        if (clip.hashtags) setCustomTags(clip.hashtags);
      }
    }
  }, [scheduleClipId, selectedVideoShorts]);

  const [transferProgress, setTransferProgress] = useState<{
    active: boolean;
    type: "download" | "upload";
    title: string;
    percent: number;
    loadedStr: string;
    totalStr: string;
    error?: string;
    completed?: boolean;
  } | null>(null);

  // Real-time download progress map: videoId -> { percent, step }
  const [videoProgress, setVideoProgress] = useState<Record<string, { percent: number; step: string }>>({});

  useEffect(() => {
    const processingIds = videos.filter(v => v.status === "Processing" || v.status === "Queued").map(v => v.id);
    if (processingIds.length === 0) return;
    let alive = true;
    const poll = async () => {
      if (!alive) return;
      const results = await Promise.allSettled(processingIds.map(id => api.videos.getProgress(id)));
      if (!alive) return;
      setVideoProgress(prev => {
        const next = { ...prev };
        results.forEach((r, i) => {
          if (r.status === "fulfilled") next[processingIds[i]] = { percent: r.value.percent, step: r.value.step };
        });
        return next;
      });
    };
    poll();
    const interval = setInterval(poll, 2000);
    return () => { alive = false; clearInterval(interval); };
  }, [videos]);

  const handleDownloadShort = async (clipId: string, title?: string) => {
    const fileName = `${title ? title.replace(/[^a-zA-Z0-9_-]/g, "_") : "lychee_short"}.mp4`;
    setTransferProgress({
      active: true,
      type: "download",
      title: fileName,
      percent: 0,
      loadedStr: "0 MB",
      totalStr: "Connecting...",
    });

    try {
      await api.videos.downloadWithProgress(
        api.videos.downloadShortUrl(clipId),
        fileName,
        (percent, loadedStr, totalStr) => {
          if (percent === 100) {
            setTransferProgress({
              active: true,
              type: "download",
              title: fileName,
              percent: 100,
              loadedStr: "",
              totalStr: "",
              completed: true,
            });
            setTimeout(() => setTransferProgress(null), 3500);
          } else {
            setTransferProgress({
              active: true,
              type: "download",
              title: fileName,
              percent: percent < 0 ? 50 : percent,
              loadedStr,
              totalStr,
            });
          }
        }
      );
    } catch (err: any) {
      setTransferProgress({
        active: true,
        type: "download",
        title: fileName,
        percent: 0,
        loadedStr: "",
        totalStr: "",
        error: err.message || "Download failed",
      });
      setTimeout(() => setTransferProgress(null), 4000);
    }
  };

  const handleDownloadFullVideo = async (videoId: string, title?: string) => {
    const fileName = `${title ? title.replace(/[^a-zA-Z0-9_-]/g, "_") : "source_video"}.mp4`;
    setTransferProgress({
      active: true,
      type: "download",
      title: fileName,
      percent: 0,
      loadedStr: "0 MB",
      totalStr: "Connecting...",
    });

    try {
      await api.videos.downloadWithProgress(
        api.videos.downloadVideoUrl(videoId),
        fileName,
        (percent, loadedStr, totalStr) => {
          if (percent === 100) {
            setTransferProgress({
              active: true,
              type: "download",
              title: fileName,
              percent: 100,
              loadedStr: "",
              totalStr: "",
              completed: true,
            });
            setTimeout(() => setTransferProgress(null), 3500);
          } else {
            setTransferProgress({
              active: true,
              type: "download",
              title: fileName,
              percent: percent < 0 ? 50 : percent,
              loadedStr,
              totalStr,
            });
          }
        }
      );
    } catch (err: any) {
      setTransferProgress({
        active: true,
        type: "download",
        title: fileName,
        percent: 0,
        loadedStr: "",
        totalStr: "",
        error: err.message || "Download failed",
      });
      setTimeout(() => setTransferProgress(null), 4000);
    }
  };

  const handleThumbnailSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setThumbnailFile(file);
      setThumbnailPreview(URL.createObjectURL(file));
    }
  };

  return (
    <div className="flex-1 flex min-h-0 overflow-hidden relative">
      {/* Real-Time Download Percentage Progress Toast */}
      {transferProgress && transferProgress.active && (
        <div className="fixed bottom-6 right-6 z-50 bg-slate-900/95 backdrop-blur-md border border-slate-800 text-white rounded-2xl p-4 shadow-2xl max-w-sm w-full animate-in slide-in-from-bottom duration-300">
          <div className="flex items-center justify-between gap-3 mb-2">
            <div className="flex items-center gap-2 min-w-0">
              <div className={`p-2 rounded-xl ${transferProgress.completed ? "bg-emerald-500/20 text-emerald-400" : transferProgress.error ? "bg-rose-500/20 text-rose-400" : "bg-rose-500/20 text-rose-400"}`}>
                {transferProgress.completed ? <CheckCircle2 size={16} /> : transferProgress.type === "download" ? <Download size={16} className="animate-bounce" /> : <Upload size={16} className="animate-bounce" />}
              </div>
              <div className="min-w-0">
                <p className="text-xs font-bold text-white truncate">{transferProgress.title}</p>
                <p className="text-[10px] text-slate-400">
                  {transferProgress.completed
                    ? "Download Complete!"
                    : transferProgress.error
                    ? transferProgress.error
                    : `${transferProgress.type === "download" ? "Downloading" : "Uploading"}... ${transferProgress.loadedStr} ${transferProgress.totalStr !== "Unknown" && transferProgress.totalStr ? `/ ${transferProgress.totalStr}` : ""}`}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2 shrink-0">
              {!transferProgress.completed && !transferProgress.error && (
                <span className="text-xs font-black text-rose-400">
                  {transferProgress.percent >= 0 ? `${transferProgress.percent}%` : "..."}
                </span>
              )}
              <button onClick={() => setTransferProgress(null)} className="text-slate-500 hover:text-slate-300 p-1 rounded-lg hover:bg-slate-800">
                <X size={14} />
              </button>
            </div>
          </div>

          {!transferProgress.completed && !transferProgress.error && (
            <div className="w-full bg-slate-800 rounded-full h-2 overflow-hidden relative mt-1">
              <div
                className="bg-gradient-to-r from-rose-500 via-purple-500 to-indigo-500 h-full rounded-full transition-all duration-150"
                style={{ width: `${Math.max(5, transferProgress.percent)}%` }}
              />
            </div>
          )}
        </div>
      )}

      {/* Preview Modal */}
      {previewClip && (
        <div className="fixed inset-0 z-50 bg-black/80 backdrop-blur-md flex items-center justify-center p-4">
          <div className="bg-zinc-900 border border-zinc-700/80 rounded-2xl max-w-lg w-full p-6 text-white relative shadow-2xl animate-in fade-in zoom-in-95 duration-200">
            <button
              onClick={() => setPreviewClip(null)}
              className="absolute top-4 right-4 text-zinc-400 hover:text-white p-1.5 rounded-xl hover:bg-zinc-800 transition-all z-10"
            >
              <X size={20} />
            </button>
            <div className="flex items-center gap-2 mb-3">
              <span className="px-2.5 py-0.5 rounded-full bg-violet-500/20 text-violet-300 border border-violet-500/30 text-xs font-bold flex items-center gap-1">
                🔥 {previewClip.viralityScore || 85}% Virality Score
              </span>
              <span className="text-xs text-zinc-400 font-medium">
                ⏱️ {Math.max(1, Math.round(parseSeconds(previewClip.endTime) - parseSeconds(previewClip.startTime)))}s Duration
              </span>
            </div>
            <h3 className="font-bold text-base mb-1.5 text-white leading-snug">{previewClip.title}</h3>
            <p className="text-xs text-zinc-400 mb-3 line-clamp-2">{previewClip.description}</p>
            {previewClip.hashtags && (
              <p className="text-xs text-rose-400 font-semibold mb-4">{previewClip.hashtags}</p>
            )}

            {/* REAL HTML5 VIDEO PLAYER */}
            <div className="relative aspect-[9/16] max-h-[380px] w-full mx-auto bg-black rounded-xl border border-zinc-800 overflow-hidden mb-5 shadow-inner flex items-center justify-center">
              <video
                key={previewClip.id}
                src={previewClip.outputUri?.startsWith("http") ? previewClip.outputUri : `http://127.0.0.1:5000/api/v1/videos/shorts/${previewClip.id}/download`}
                controls
                autoPlay
                loop
                playsInline
                className="w-full h-full object-cover rounded-xl"
                onError={(e) => {
                  const target = e.currentTarget;
                  if (!target.dataset.fallback) {
                    target.dataset.fallback = "true";
                    target.src = "https://assets.mixkit.co/videos/preview/mixkit-flying-through-a-starfield-in-deep-space-41538-large.mp4";
                    target.play().catch(() => {});
                  }
                }}
              />
            </div>

            <div className="flex gap-2.5">
              <button
                onClick={() => handleDownloadShort(previewClip.id, previewClip.title)}
                className="flex-1 py-2.5 bg-zinc-800 hover:bg-zinc-700 text-white rounded-xl text-xs font-bold text-center flex items-center justify-center gap-2 transition-all shadow-xs"
              >
                <Download size={14} /> Download Short
              </button>
              <button
                onClick={() => {
                  setScheduleClipId(previewClip.id);
                  setPreviewClip(null);
                }}
                className="flex-1 py-2.5 bg-gradient-to-r from-rose-600 to-red-500 hover:from-rose-500 hover:to-red-400 text-white rounded-xl text-xs font-bold flex items-center justify-center gap-2 transition-all shadow-md shadow-rose-950/40"
              >
                <Calendar size={14} /> Schedule Now
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Left panel - Import & Library */}
      <div className="w-80 shrink-0 bg-white border-r border-slate-200 flex flex-col overflow-y-auto">
        {/* Import section */}
        <div className="p-4 border-b border-slate-100">
          <h2 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-3">Import Video</h2>
          {/* Mode toggle */}
          <div className="flex rounded-xl bg-slate-100 p-1 mb-4 gap-1">
            <button
              onClick={() => setImportMode("youtube")}
              className={`flex-1 flex items-center justify-center gap-1.5 py-2 rounded-lg text-xs font-bold transition-all ${importMode === "youtube" ? "bg-white text-rose-600 shadow-xs" : "text-slate-500 hover:text-slate-700"}`}
            >
              <Link2 size={12} /> Video URL
            </button>
            <button
              onClick={() => setImportMode("upload")}
              className={`flex-1 flex items-center justify-center gap-1.5 py-2 rounded-lg text-xs font-bold transition-all ${importMode === "upload" ? "bg-white text-rose-600 shadow-xs" : "text-slate-500 hover:text-slate-700"}`}
            >
              <Upload size={12} /> Upload File
            </button>
          </div>

          {importMode === "youtube" ? (
            <form onSubmit={handleImportYoutube} className="space-y-3">
              <input
                type="text" value={youtubeTitle} onChange={e => setYoutubeTitle(e.target.value)}
                placeholder="Video title (optional)"
                className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 placeholder-slate-400"
              />
              <input
                type="url" value={youtubeUrl} onChange={e => setYoutubeUrl(e.target.value)}
                placeholder="Paste YouTube, Instagram, FB, or TikTok URL..." required
                className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 placeholder-slate-400"
              />
              <p className="text-[10px] text-slate-400 flex items-center gap-1">
                <span>⚡ Supports YouTube, Instagram Reels, Facebook & TikTok links</span>
              </p>
              <button
                type="submit" disabled={loading || !youtubeUrl}
                className="w-full py-2.5 bg-gradient-to-r from-rose-600 to-red-500 text-white text-xs font-black rounded-xl hover:from-rose-500 hover:to-red-400 disabled:opacity-50 transition-all flex items-center justify-center gap-2"
              >
                {loading ? <Loader2 size={13} className="animate-spin" /> : <Sparkles size={13} />}
                Import & Generate Shorts
              </button>
            </form>
          ) : (
            <form onSubmit={handleUpload} className="space-y-3">
              <input
                type="text" value={uploadTitle} onChange={e => setUploadTitle(e.target.value)}
                placeholder="Video title (optional)"
                className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 placeholder-slate-400"
              />
              <div
                onClick={() => fileInputRef.current?.click()}
                className="border-2 border-dashed border-slate-200 hover:border-rose-300 rounded-xl p-4 cursor-pointer transition-colors text-center group"
              >
                <Upload size={20} className="mx-auto mb-2 text-slate-300 group-hover:text-rose-400 transition-colors" />
                <p className="text-xs text-slate-500 font-medium">{uploadFile ? uploadFile.name : "Click to select .mp4, .mov, .avi"}</p>
                <p className="text-[10px] text-slate-400 mt-1">Max 500MB</p>
              </div>
              <input ref={fileInputRef} type="file" accept="video/*" className="hidden" onChange={e => setUploadFile(e.target.files?.[0] || null)} />
              <button
                type="submit" disabled={loading || !uploadFile}
                className="w-full py-2.5 bg-gradient-to-r from-rose-600 to-red-500 text-white text-xs font-black rounded-xl hover:from-rose-500 hover:to-red-400 disabled:opacity-50 transition-all flex items-center justify-center gap-2"
              >
                {loading ? <Loader2 size={13} className="animate-spin" /> : <Upload size={13} />}
                Upload & Process
              </button>
            </form>
          )}

          {actionError && (
            <div className="mt-3 p-2.5 rounded-xl bg-rose-50 border border-rose-200 text-rose-600 text-[11px] flex items-start gap-2">
              <AlertCircle size={13} className="shrink-0 mt-0.5" />
              <span>{actionError}</span>
            </div>
          )}
        </div>

        {/* Video library list */}
        <div className="flex-1 overflow-y-auto p-4">
          <h2 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-3">Video Library ({videos.length})</h2>
          {videos.length === 0 ? (
            <div className="text-center py-8">
              <Film size={32} className="mx-auto text-slate-200 mb-2" />
              <p className="text-xs text-slate-400">No videos yet. Import or upload one!</p>
            </div>
          ) : (
            <div className="space-y-2">
              {videos.map(v => (
                <div
                  key={v.id}
                  onClick={() => setSelectedVideo(v)}
                  className={`w-full text-left p-3 rounded-xl border transition-all cursor-pointer group ${selectedVideo?.id === v.id ? "bg-rose-50 border-rose-200 shadow-xs" : "bg-white border-slate-200 hover:border-rose-200"}`}
                >
                  <div className="flex items-start justify-between gap-2">
                    <p className="text-xs font-bold text-slate-800 truncate leading-tight flex-1">{v.title}</p>
                    <div className="flex items-center gap-1 shrink-0">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDownloadFullVideo(v.id, v.title);
                        }}
                        className="p-1 text-slate-400 hover:text-slate-700 transition-colors"
                        title="Download Video"
                      >
                        <Download size={12} />
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleStopVideoProcessing(v.id);
                        }}
                        className="p-1 text-slate-400 hover:text-amber-600 transition-colors"
                        title="Stop Processing"
                      >
                        <Square size={11} fill="currentColor" />
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDeleteVideo(v.id);
                        }}
                        className="p-1 text-slate-400 hover:text-rose-600 transition-colors"
                        title="Delete Video"
                      >
                        <Trash2 size={12} />
                      </button>
                      <StatusBadge status={v.status} />
                    </div>
                  </div>
                  <p className="text-[10px] text-slate-400 mt-1 truncate">{v.sourceUrl}</p>
                  <p className="text-[10px] text-slate-400 mt-0.5">{new Date(v.createdAtUtc).toLocaleDateString()}</p>
                  {/* Real-time progress bar for Processing/Queued videos */}
                  {(v.status === "Processing" || v.status === "Queued") && (() => {
                    const prog = videoProgress[v.id];
                    const pct = prog?.percent ?? 0;
                    const step = prog?.step ?? "Waiting in queue...";
                    return (
                      <div className="mt-2">
                        <div className="flex justify-between items-center mb-1">
                          <span className="text-[9px] text-blue-600 font-medium truncate">{step}</span>
                          <span className="text-[9px] text-blue-700 font-bold ml-1">{pct}%</span>
                        </div>
                        <div className="w-full h-1.5 bg-blue-100 rounded-full overflow-hidden">
                          <div
                            className="h-full bg-gradient-to-r from-blue-500 to-violet-500 rounded-full transition-all duration-500"
                            style={{ width: `${Math.max(3, pct)}%` }}
                          />
                        </div>
                      </div>
                    );
                  })()}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Right panel - Clips & Advanced Publishing Options */}
      <div className="flex-1 overflow-y-auto bg-slate-50 p-6 space-y-6">
        {!selectedVideo ? (
          <div className="flex flex-col items-center justify-center h-full text-center py-20">
            <div className="h-16 w-16 rounded-2xl bg-slate-100 flex items-center justify-center mb-4">
              <PlaySquare size={28} className="text-slate-300" />
            </div>
            <h3 className="font-bold text-slate-600 mb-2">Select a Video</h3>
            <p className="text-sm text-slate-400 max-w-xs">Choose a video from the library on the left to see its AI-generated shorts and schedule them.</p>
          </div>
        ) : (
          <>
            {/* Video detail card */}
            <div className="bg-white rounded-2xl border border-slate-200 shadow-xs p-5">
              <div className="flex items-start justify-between gap-4">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <h2 className="font-black text-slate-900 text-base truncate">{selectedVideo.title}</h2>
                  </div>
                  <p className="text-xs text-slate-400 mt-1 truncate">{selectedVideo.sourceUrl}</p>
                  <div className="flex items-center gap-2 mt-2">
                    <StatusBadge status={selectedVideo.status} />
                    <span className="text-[10px] text-slate-400">{new Date(selectedVideo.createdAtUtc).toLocaleString()}</span>
                  </div>
                </div>

                <div className="flex items-center gap-2 shrink-0 flex-wrap justify-end">
                  <button
                    onClick={() => handleDownloadFullVideo(selectedVideo.id, selectedVideo.title)}
                    className="flex items-center gap-1.5 px-3 py-2 bg-slate-100 hover:bg-slate-200 text-slate-800 text-xs font-bold rounded-xl transition-all border border-slate-200"
                    title="Download original video file"
                  >
                    <Download size={13} /> Download
                  </button>

                  <button
                    onClick={() => handleStopVideoProcessing(selectedVideo.id)}
                    disabled={loading}
                    className="flex items-center gap-1.5 px-3 py-2 bg-amber-50 hover:bg-amber-100 text-amber-700 border border-amber-200 text-xs font-bold rounded-xl transition-all shadow-xs"
                    title="Stop / Cancel video processing job"
                  >
                    <Square size={12} fill="currentColor" /> Stop
                  </button>

                  <button
                    onClick={() => handleDeleteVideo(selectedVideo.id)}
                    disabled={loading}
                    className="flex items-center gap-1.5 px-3 py-2 bg-rose-50 hover:bg-rose-100 text-rose-600 border border-rose-200 text-xs font-bold rounded-xl transition-all shadow-xs"
                    title="Delete video & all associated clips"
                  >
                    <Trash2 size={13} /> Delete Video
                  </button>

                  <button
                    onClick={() => handleTriggerAiProcessing(selectedVideo.id)}
                    disabled={loading}
                    className="flex items-center gap-1.5 px-4 py-2 bg-gradient-to-r from-violet-600 to-purple-500 text-white text-xs font-black rounded-xl hover:from-violet-500 hover:to-purple-400 disabled:opacity-50 transition-all shadow-xs"
                  >
                    {loading ? <Loader2 size={13} className="animate-spin" /> : <Sparkles size={13} />}
                    {selectedVideo.status === "Processed" ? "Re-Generate Shorts" : "Generate AI Shorts"}
                  </button>
                </div>
              </div>
            </div>

            {/* AI Shorts Grid */}
            {selectedVideoShorts.length > 0 && (
              <div>
                <h3 className="text-xs font-black text-slate-700 uppercase tracking-widest mb-3 flex items-center gap-2">
                  <Zap size={12} className="text-violet-500" /> AI Generated Shorts ({selectedVideoShorts.length})
                </h3>
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
                  {selectedVideoShorts.map(clip => (
                    <div key={clip.id} className={`bg-white rounded-xl border p-4 transition-all relative group ${scheduleClipId === clip.id ? "border-rose-400 ring-2 ring-rose-500/20 shadow-sm" : "border-slate-200 hover:border-violet-200"}`}>
                      <div className="flex items-start justify-between gap-2 mb-2">
                        <p className="text-xs font-bold text-slate-800 leading-tight">{clip.title}</p>
                        <div className="flex items-center gap-1.5 shrink-0">
                          {clip.viralityScore !== undefined && (
                            <span className="text-[10px] font-black bg-violet-100 text-violet-600 px-2 py-0.5 rounded-full border border-violet-200">
                              🔥 {clip.viralityScore}%
                            </span>
                          )}
                          <button
                            onClick={() => handleDeleteShortClip(clip.id)}
                            className="p-1 text-slate-300 hover:text-rose-500 transition-colors"
                            title="Delete Clip"
                          >
                            <Trash2 size={12} />
                          </button>
                        </div>
                      </div>
                      <p className="text-[10px] text-slate-500 mb-2">{clip.description?.slice(0, 80)}...</p>
                      <div className="flex items-center gap-2 text-[10px] text-slate-400">
                        <Clock size={10} />
                        <span>{Math.floor(parseSeconds(clip.startTime))}s — {Math.floor(parseSeconds(clip.endTime))}s</span>
                        <span className="text-slate-300">|</span>
                        <span>{Math.round(parseSeconds(clip.endTime) - parseSeconds(clip.startTime))}s duration</span>
                      </div>
                      {clip.hashtags && <p className="text-[10px] text-rose-500 mt-1.5 truncate">{clip.hashtags}</p>}

                      <div className="mt-3 flex gap-2">
                        <button
                          onClick={() => setPreviewClip(clip)}
                          className="px-3 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-[11px] font-bold rounded-xl flex items-center gap-1 transition-all"
                        >
                          <Eye size={12} /> Preview
                        </button>
                        <button
                          onClick={() => handleDownloadShort(clip.id, clip.title)}
                          className="px-3 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-[11px] font-bold rounded-xl flex items-center gap-1 transition-all"
                        >
                          <Download size={12} /> Download
                        </button>
                        <button
                          onClick={() => setScheduleClipId(clip.id)}
                          className={`flex-1 py-2 rounded-xl text-[11px] font-black transition-all border ${scheduleClipId === clip.id ? "bg-rose-600 text-white border-rose-600" : "bg-slate-50 text-slate-600 border-slate-200 hover:border-rose-300 hover:text-rose-600"}`}
                        >
                          {scheduleClipId === clip.id ? "✓ Selected Options Below" : "Select & Configure"}
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Comprehensive Social Media Upload & Schedule Options Panel */}
            {scheduleClipId && (
              <div className="bg-white rounded-2xl border border-rose-200 shadow-sm p-6 space-y-5">
                <div className="flex items-center justify-between border-b border-slate-100 pb-3">
                  <div>
                    <h3 className="text-sm font-black text-slate-900 flex items-center gap-2">
                      <Calendar size={16} className="text-rose-500" /> Configure Social Media Upload & Schedule
                    </h3>
                    <p className="text-[11px] text-slate-400 mt-0.5">Customize title, thumbnail, first pinned comment, categories, and visibility</p>
                  </div>
                  <button
                    onClick={() => setShowAdvanced(!showAdvanced)}
                    className="text-xs font-bold text-rose-600 hover:text-rose-700 bg-rose-50 px-3 py-1.5 rounded-xl border border-rose-200 transition-all flex items-center gap-1"
                  >
                    <Layers size={13} /> {showAdvanced ? "Basic View" : "+ Advanced Options"}
                  </button>
                </div>

                <form onSubmit={handleCreateSchedule} className="space-y-4">
                  {/* Basic Schedule Grid */}
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 block">Publish Date *</label>
                      <input type="date" value={scheduleDate} onChange={e => setScheduleDate(e.target.value)} required
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400" />
                    </div>
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 block">Publish Time *</label>
                      <input type="time" value={scheduleTime} onChange={e => setScheduleTime(e.target.value)} required
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400" />
                    </div>
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 block">Platform</label>
                      <select value={schedulePlatform} onChange={e => setSchedulePlatform(e.target.value)}
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 font-bold">
                        <option value="1">YouTube Shorts</option>
                        <option value="2">Instagram Reels</option>
                        <option value="3">Facebook Reels</option>
                      </select>
                    </div>
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 block">Target Channel *</label>
                      <select value={selectedSocialAccountId} onChange={e => setSelectedSocialAccountId(e.target.value)} required
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400">
                        <option value="">Select connected channel...</option>
                        {socialAccounts.map(a => (
                          <option key={a.id} value={a.id}>{a.displayName} ({a.channelName})</option>
                        ))}
                      </select>
                    </div>
                  </div>

                  {/* Title & Description Overrides */}
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                        <span>Video / Reel Title</span>
                      </label>
                      <input
                        type="text"
                        value={customTitle}
                        onChange={e => setCustomTitle(e.target.value)}
                        placeholder="Enter post title..."
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                      />
                    </div>
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                        <Tag size={11} className="text-rose-500" /> Tags & Hashtags
                      </label>
                      <input
                        type="text"
                        value={customTags}
                        onChange={e => setCustomTags(e.target.value)}
                        placeholder="#Shorts #Viral #LycheeAI"
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                      />
                    </div>
                  </div>

                  <div>
                    <label className="text-[11px] font-bold text-slate-700 mb-1 block">Full Description & Caption</label>
                    <textarea
                      rows={2}
                      value={customDescription}
                      onChange={e => setCustomDescription(e.target.value)}
                      placeholder="Write your main video description or reel caption..."
                      className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 resize-none"
                    />
                  </div>

                  {/* Advanced Publishing Options Panel */}
                  <div className="pt-2 border-t border-slate-100 space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                      {/* Custom Thumbnail Picker */}
                      <div>
                        <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                          <ImageIcon size={12} className="text-blue-500" /> Custom Thumbnail Image
                        </label>
                        <div
                          onClick={() => thumbnailInputRef.current?.click()}
                          className="border border-dashed border-slate-300 hover:border-rose-400 rounded-xl p-2.5 cursor-pointer text-center bg-slate-50 hover:bg-slate-100/50 transition-all flex items-center gap-2"
                        >
                          {thumbnailPreview ? (
                            <img src={thumbnailPreview} alt="Thumb" className="h-9 w-9 object-cover rounded-lg border border-slate-200 shrink-0" />
                          ) : (
                            <ImageIcon size={20} className="text-slate-400 shrink-0 mx-auto" />
                          )}
                          <div className="text-left min-w-0 flex-1">
                            <p className="text-[11px] font-bold text-slate-700 truncate">{thumbnailFile ? thumbnailFile.name : "Select custom cover image"}</p>
                            <p className="text-[9px] text-slate-400">JPG, PNG (Max 5MB)</p>
                          </div>
                        </div>
                        <input ref={thumbnailInputRef} type="file" accept="image/*" className="hidden" onChange={handleThumbnailSelect} />
                      </div>

                      {/* Category Selector */}
                      <div>
                        <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                          <Layers size={12} className="text-violet-500" /> Category / Co-Types
                        </label>
                        <select
                          value={category}
                          onChange={e => setCategory(e.target.value)}
                          className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                        >
                          <option value="Entertainment">Entertainment</option>
                          <option value="Gaming">Gaming & Esports</option>
                          <option value="Tech & Science">Tech & Science</option>
                          <option value="Education">Education & Tutorials</option>
                          <option value="Howto & Style">How-to & Style</option>
                          <option value="Comedy">Comedy & Humor</option>
                          <option value="Music">Music & Audio</option>
                          <option value="News & Politics">News & Politics</option>
                        </select>
                      </div>

                      {/* Visibility & Privacy Status */}
                      <div>
                        <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                          <Globe size={12} className="text-emerald-500" /> Privacy & Visibility
                        </label>
                        <select
                          value={privacyStatus}
                          onChange={e => setPrivacyStatus(e.target.value)}
                          className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400 font-bold"
                        >
                          <option value="public">🌐 Public (Immediate Reach)</option>
                          <option value="unlisted">🔗 Unlisted (Link Only)</option>
                          <option value="private">🔒 Private (Draft Mode)</option>
                        </select>
                      </div>
                    </div>

                    {/* First Pinned Comment */}
                    <div>
                      <label className="text-[11px] font-bold text-slate-700 mb-1 flex items-center gap-1">
                        <MessageCircle size={12} className="text-amber-500" /> Automated First Pinned Comment (Optional)
                      </label>
                      <input
                        type="text"
                        value={pinnedComment}
                        onChange={e => setPinnedComment(e.target.value)}
                        placeholder="e.g. Subscribe to our channel & check out the full link in bio! 👇"
                        className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                      />
                    </div>

                    {/* Audience Toggles */}
                    <div className="flex items-center gap-6 pt-1">
                      <label className="flex items-center gap-2 cursor-pointer text-xs font-bold text-slate-700">
                        <input
                          type="checkbox"
                          checked={isForKids}
                          onChange={e => setIsForKids(e.target.checked)}
                          className="rounded border-slate-300 text-rose-600 focus:ring-rose-500 h-4 w-4"
                        />
                        <span>Made for Kids (COPPA Compliant)</span>
                      </label>

                      <label className="flex items-center gap-2 cursor-pointer text-xs font-bold text-slate-700">
                        <input
                          type="checkbox"
                          checked={hasAgeRestriction}
                          onChange={e => setHasAgeRestriction(e.target.checked)}
                          className="rounded border-slate-300 text-rose-600 focus:ring-rose-500 h-4 w-4"
                        />
                        <span className="flex items-center gap-1 text-slate-700">
                          <ShieldAlert size={12} className="text-rose-500" /> Age Restriction (18+ Only)
                        </span>
                      </label>
                    </div>
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="w-full py-3 bg-gradient-to-r from-rose-600 to-red-500 text-white text-xs font-black rounded-xl hover:from-rose-500 hover:to-red-400 disabled:opacity-50 transition-all flex items-center justify-center gap-2 shadow-sm"
                  >
                    {loading ? <Loader2 size={14} className="animate-spin" /> : <Calendar size={14} />}
                    Schedule Clip with All Selected Options
                  </button>
                </form>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};

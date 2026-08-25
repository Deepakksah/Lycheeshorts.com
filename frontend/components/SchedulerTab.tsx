"use client";

import React, { useState } from "react";
import { ScheduleResponse, ShortClipResponse } from "../lib/api";
import {
  Calendar as CalendarIcon, Trash2, Edit3, Check, X, Clock, AlertCircle, Loader2,
  ChevronLeft, ChevronRight, LayoutGrid, List, PlaySquare, Zap, Plus, Video
} from "lucide-react";

interface SchedulerTabProps {
  schedules: ScheduleResponse[];
  shortsCache: Record<string, ShortClipResponse>;
  editingScheduleId: string | null;
  setEditingScheduleId: (id: string | null) => void;
  rescheduleDateTime: string;
  setRescheduleDateTime: (v: string) => void;
  handleReschedule: (id: string) => void;
  handleDeleteSchedule: (id: string) => void;
  loading: boolean;
  getPlatformLabel: (p: string) => string;
  getStatusBadge: (s: string) => React.ReactNode;
}

export const SchedulerTab: React.FC<SchedulerTabProps> = ({
  schedules, shortsCache, editingScheduleId, setEditingScheduleId,
  rescheduleDateTime, setRescheduleDateTime, handleReschedule, handleDeleteSchedule,
  loading, getPlatformLabel, getStatusBadge
}) => {
  const [viewMode, setViewMode] = useState<"calendar" | "list">("calendar");
  const [currentDate, setCurrentDate] = useState<Date>(new Date());
  const [selectedDay, setSelectedDay] = useState<number | null>(new Date().getDate());

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  // Calendar calculations
  const firstDayOfMonth = new Date(year, month, 1).getDay();
  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const monthName = currentDate.toLocaleString("default", { month: "long" });

  const sortedSchedules = [...schedules].sort((a, b) => new Date(a.publishAtUtc).getTime() - new Date(b.publishAtUtc).getTime());

  // Group schedules by day of current month
  const schedulesByDay: Record<number, ScheduleResponse[]> = {};
  schedules.forEach(s => {
    const d = new Date(s.publishAtUtc);
    if (d.getFullYear() === year && d.getMonth() === month) {
      const dayNum = d.getDate();
      if (!schedulesByDay[dayNum]) schedulesByDay[dayNum] = [];
      schedulesByDay[dayNum].push(s);
    }
  });

  const prevMonth = () => setCurrentDate(new Date(year, month - 1, 1));
  const nextMonth = () => setCurrentDate(new Date(year, month + 1, 1));
  const today = () => { setCurrentDate(new Date()); setSelectedDay(new Date().getDate()); };

  return (
    <div className="flex-1 flex flex-col min-h-0 overflow-hidden w-full bg-slate-50">
      {/* Calendar Header Bar */}
      <div className="bg-white border-b border-slate-200 px-8 py-3.5 flex items-center justify-between shrink-0 shadow-sm">
        <div className="flex items-center gap-3">
          <div className="h-9 w-9 rounded-xl bg-gradient-to-r from-rose-600 to-red-500 flex items-center justify-center text-white shadow-md">
            <CalendarIcon size={18} />
          </div>
          <div>
            <h1 className="text-base font-black text-slate-900 tracking-tight">Content Publishing Calendar</h1>
            <p className="text-xs text-slate-500">{schedules.length} clips scheduled across YouTube, Instagram & Facebook</p>
          </div>
        </div>

        {/* Controls */}
        <div className="flex items-center gap-3">
          {/* Month navigation */}
          <div className="flex items-center gap-2 bg-slate-100 p-1 rounded-xl border border-slate-200">
            <button onClick={prevMonth} className="p-1.5 hover:bg-white rounded-lg text-slate-600 transition-all">
              <ChevronLeft size={16} />
            </button>
            <span className="text-xs font-black text-slate-800 min-w-[120px] text-center">
              {monthName} {year}
            </span>
            <button onClick={nextMonth} className="p-1.5 hover:bg-white rounded-lg text-slate-600 transition-all">
              <ChevronRight size={16} />
            </button>
            <button onClick={today} className="px-2.5 py-1 text-[11px] font-black bg-white text-slate-800 rounded-lg shadow-xs hover:bg-slate-50">
              Today
            </button>
          </div>

          {/* View mode toggle */}
          <div className="flex items-center gap-1 bg-slate-100 p-1 rounded-xl border border-slate-200">
            <button
              onClick={() => setViewMode("calendar")}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-black transition-all ${
                viewMode === "calendar" ? "bg-white text-rose-600 shadow-xs" : "text-slate-500 hover:text-slate-800"
              }`}
            >
              <LayoutGrid size={13} /> Calendar
            </button>
            <button
              onClick={() => setViewMode("list")}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-black transition-all ${
                viewMode === "list" ? "bg-white text-rose-600 shadow-xs" : "text-slate-500 hover:text-slate-800"
              }`}
            >
              <List size={13} /> Timeline
            </button>
          </div>
        </div>
      </div>

      {/* Main View Area */}
      <div className="flex-1 overflow-y-auto p-6 w-full">
        {viewMode === "calendar" ? (
          <div className="w-full h-full flex flex-col space-y-4">
            {/* Calendar Grid */}
            <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden flex-1 flex flex-col">
              {/* Day names header */}
              <div className="grid grid-cols-7 bg-slate-100/70 border-b border-slate-200 text-center py-2.5 text-[11px] font-black text-slate-600 uppercase tracking-wider">
                <div>Sun</div><div>Mon</div><div>Tue</div><div>Wed</div><div>Thu</div><div>Fri</div><div>Sat</div>
              </div>

              {/* Grid cells */}
              <div className="grid grid-cols-7 flex-1 auto-rows-fr divide-x divide-y divide-slate-100 min-h-[500px]">
                {/* Empty cells before month start */}
                {Array.from({ length: firstDayOfMonth }).map((_, i) => (
                  <div key={`empty-${i}`} className="bg-slate-50/40 p-2 min-h-[100px]" />
                ))}

                {/* Days of month */}
                {Array.from({ length: daysInMonth }).map((_, i) => {
                  const dayNum = i + 1;
                  const daySchedules = schedulesByDay[dayNum] || [];
                  const isToday =
                    dayNum === new Date().getDate() &&
                    month === new Date().getMonth() &&
                    year === new Date().getFullYear();
                  const isSelected = selectedDay === dayNum;

                  return (
                    <div
                      key={dayNum}
                      onClick={() => setSelectedDay(dayNum)}
                      className={`p-2 transition-all min-h-[100px] flex flex-col justify-between cursor-pointer group ${
                        isToday ? "bg-rose-50/30" : isSelected ? "bg-slate-50/80" : "bg-white hover:bg-slate-50/50"
                      }`}
                    >
                      <div className="flex items-center justify-between">
                        <span
                          className={`h-6 w-6 rounded-full flex items-center justify-center text-xs font-black ${
                            isToday
                              ? "bg-rose-600 text-white shadow-sm"
                              : isSelected
                              ? "bg-slate-800 text-white"
                              : "text-slate-700 group-hover:text-rose-600"
                          }`}
                        >
                          {dayNum}
                        </span>
                        {daySchedules.length > 0 && (
                          <span className="text-[10px] font-bold text-slate-400 bg-slate-100 px-1.5 py-0.5 rounded-full">
                            {daySchedules.length}
                          </span>
                        )}
                      </div>

                      {/* Scheduled badges preview */}
                      <div className="space-y-1 my-1 flex-1 overflow-y-auto max-h-[80px]">
                        {daySchedules.map(sch => {
                          const clip = shortsCache[sch.shortClipId];
                          const pLabel = getPlatformLabel(sch.platform);
                          const pColor = sch.platform === "1" ? "bg-red-50 text-red-700 border-red-200" : sch.platform === "2" ? "bg-purple-50 text-purple-700 border-purple-200" : "bg-blue-50 text-blue-700 border-blue-200";

                          return (
                            <div
                              key={sch.id}
                              className={`p-1 rounded-lg border text-[10px] font-bold truncate leading-tight flex items-center gap-1 shadow-2xs ${pColor}`}
                              title={`${clip?.title || "Scheduled Short"} — ${new Date(sch.publishAtUtc).toLocaleTimeString()}`}
                            >
                              <PlaySquare size={10} className="shrink-0" />
                              <span className="truncate">{clip?.title || pLabel}</span>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Day Detail drawer */}
            {selectedDay !== null && (
              <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
                <div className="flex items-center justify-between mb-4">
                  <h3 className="text-xs font-black text-slate-800 uppercase tracking-widest flex items-center gap-2">
                    <Clock size={14} className="text-rose-500" /> Schedules for {monthName} {selectedDay}, {year}
                  </h3>
                  <span className="text-xs text-slate-400">
                    {(schedulesByDay[selectedDay] || []).length} scheduled items
                  </span>
                </div>

                {(schedulesByDay[selectedDay] || []).length === 0 ? (
                  <p className="text-xs text-slate-400 text-center py-4">No scheduled clips for this day. Click on any clip in Workspace to schedule it.</p>
                ) : (
                  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                    {(schedulesByDay[selectedDay] || []).map(schedule => {
                      const clip = shortsCache[schedule.shortClipId];
                      const isEditing = editingScheduleId === schedule.id;

                      return (
                        <div key={schedule.id} className="bg-slate-50 rounded-xl border border-slate-200 p-3.5 space-y-2">
                          <div className="flex items-center justify-between">
                            <span className="text-[10px] font-bold text-slate-500">{getPlatformLabel(schedule.platform)}</span>
                            {getStatusBadge(schedule.status)}
                          </div>
                          <p className="text-xs font-bold text-slate-800 truncate">{clip?.title || `Clip ${schedule.shortClipId.slice(0, 8)}`}</p>
                          <p className="text-[10px] text-rose-600 font-bold">{new Date(schedule.publishAtUtc).toLocaleTimeString()}</p>
                          <div className="flex justify-end gap-1 pt-1">
                            <button
                              onClick={() => handleDeleteSchedule(schedule.id)}
                              className="p-1 text-slate-400 hover:text-rose-500"
                              title="Delete"
                            >
                              <Trash2 size={12} />
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            )}
          </div>
        ) : (
          /* List / Timeline View */
          <div className="w-full space-y-4">
            <div className="flex items-center justify-between mb-2">
              <h2 className="text-lg font-black text-slate-900">All Scheduled Dispatches ({sortedSchedules.length})</h2>
            </div>

            {sortedSchedules.length === 0 ? (
              <div className="text-center py-20 bg-white rounded-2xl border border-slate-200">
                <CalendarIcon size={40} className="mx-auto text-slate-200 mb-3" />
                <h3 className="font-bold text-slate-600 mb-1">No Scheduled Clips</h3>
                <p className="text-sm text-slate-400">Go to Workspace to select a clip and schedule it.</p>
              </div>
            ) : (
              <div className="space-y-3">
                {sortedSchedules.map(schedule => {
                  const clip = shortsCache[schedule.shortClipId];
                  const isEditing = editingScheduleId === schedule.id;
                  const publishDate = new Date(schedule.publishAtUtc);
                  const isPast = publishDate < new Date();

                  return (
                    <div key={schedule.id} className={`bg-white rounded-2xl border shadow-sm p-4 transition-all ${isEditing ? "border-rose-300 shadow-rose-100" : "border-slate-200"}`}>
                      <div className="flex items-start justify-between gap-4">
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 mb-1">
                            {getStatusBadge(schedule.status)}
                            <span className="text-xs font-bold text-slate-600">{getPlatformLabel(schedule.platform)}</span>
                          </div>
                          <p className="text-sm font-bold text-slate-900 truncate">{clip?.title || `Clip ${schedule.shortClipId.slice(0, 8)}...`}</p>
                          <div className="flex items-center gap-2 mt-1.5 text-xs text-slate-500">
                            <Clock size={12} />
                            <span className={isPast ? "text-slate-400" : "text-rose-600 font-bold"}>
                              {publishDate.toLocaleString()}
                            </span>
                            {isPast && schedule.status === "Scheduled" && <span className="text-amber-500 font-bold">(Overdue)</span>}
                          </div>
                          {clip && <p className="text-xs text-slate-400 mt-1 truncate">{clip.description?.slice(0, 80)}...</p>}
                        </div>

                        <div className="flex items-center gap-2 shrink-0">
                          {schedule.status === "Scheduled" && (
                            <button
                              onClick={() => setEditingScheduleId(isEditing ? null : schedule.id)}
                              className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-xl transition-all"
                              title="Reschedule"
                            >
                              <Edit3 size={14} />
                            </button>
                          )}
                          <button
                            onClick={() => handleDeleteSchedule(schedule.id)}
                            disabled={loading}
                            className="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all"
                            title="Delete"
                          >
                            <Trash2 size={14} />
                          </button>
                        </div>
                      </div>

                      {isEditing && (
                        <div className="mt-4 pt-4 border-t border-slate-100 flex items-center gap-3">
                          <input
                            type="datetime-local"
                            value={rescheduleDateTime}
                            onChange={e => setRescheduleDateTime(e.target.value)}
                            className="flex-1 px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-800 focus:outline-none focus:border-rose-400"
                          />
                          <button
                            onClick={() => handleReschedule(schedule.id)}
                            disabled={loading || !rescheduleDateTime}
                            className="flex items-center gap-1.5 px-4 py-2 bg-rose-600 text-white text-xs font-bold rounded-xl hover:bg-rose-500 disabled:opacity-50 transition-all"
                          >
                            {loading ? <Loader2 size={12} className="animate-spin" /> : <Check size={12} />}
                            Update
                          </button>
                          <button
                            onClick={() => setEditingScheduleId(null)}
                            className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-xl transition-all"
                          >
                            <X size={14} />
                          </button>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

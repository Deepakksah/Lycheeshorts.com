"use client";

import React from "react";
import { PlaySquare, Lock, Mail, Eye, EyeOff, UserPlus, KeyRound, ArrowLeft } from "lucide-react";

interface AuthModalProps {
  authMode: "login" | "register" | "verify" | "forgot" | "reset";
  setAuthMode: (m: "login" | "register" | "verify" | "forgot" | "reset") => void;
  authEmail: string; setAuthEmail: (v: string) => void;
  authPassword: string; setAuthPassword: (v: string) => void;
  authDisplayName: string; setAuthDisplayName: (v: string) => void;
  verificationToken: string; setVerificationToken: (v: string) => void;
  resetToken: string; setResetToken: (v: string) => void;
  newPassword: string; setNewPassword: (v: string) => void;
  authError: string; setAuthError: (v: string) => void;
  loading: boolean;
  handleAuthSubmit: (e: React.FormEvent) => void;
}

export const AuthModal: React.FC<AuthModalProps> = ({
  authMode, setAuthMode, authEmail, setAuthEmail, authPassword, setAuthPassword,
  authDisplayName, setAuthDisplayName, verificationToken, setVerificationToken,
  resetToken, setResetToken, newPassword, setNewPassword,
  authError, setAuthError, loading, handleAuthSubmit
}) => {
  const [showPw, setShowPw] = React.useState(false);

  return (
    <div className="min-h-screen bg-gradient-to-br from-zinc-950 via-zinc-900 to-rose-950 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Brand */}
        <div className="flex flex-col items-center mb-8 gap-4">
          <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-gradient-to-tr from-rose-700 via-rose-600 to-red-500 shadow-2xl shadow-rose-950">
            <PlaySquare className="text-white fill-white/20" size={32} />
          </div>
          <div className="text-center">
            <h1 className="text-3xl font-black text-white tracking-tight">Lychee</h1>
            <p className="text-rose-400 text-xs uppercase tracking-widest font-semibold mt-1">AI Shorts Publisher</p>
          </div>
        </div>

        {/* Card */}
        <div className="bg-zinc-900/80 backdrop-blur border border-rose-950/40 rounded-2xl p-8 shadow-2xl">
          <h2 className="text-lg font-black text-white mb-1">
            {authMode === "login" ? "Sign In" : authMode === "register" ? "Create Account" : authMode === "verify" ? "Verify Email" : authMode === "forgot" ? "Forgot Password" : "Reset Password"}
          </h2>
          <p className="text-xs text-zinc-500 mb-6">
            {authMode === "login" ? "Welcome back to your dashboard" : authMode === "register" ? "Start publishing viral shorts today" : authMode === "verify" ? "Enter the code sent to your email" : authMode === "forgot" ? "We'll send you a reset link" : "Enter your new password"}
          </p>

          {authError && (
            <div className="mb-4 p-3 rounded-xl bg-rose-950/50 border border-rose-800/40 text-rose-300 text-xs font-medium">
              {authError}
            </div>
          )}

          <form onSubmit={handleAuthSubmit} className="space-y-4">
            {authMode === "register" && (
              <div>
                <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">Display Name</label>
                <div className="relative">
                  <UserPlus size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500" />
                  <input
                    type="text" value={authDisplayName} onChange={e => setAuthDisplayName(e.target.value)}
                    placeholder="Your name" required
                    className="w-full pl-9 pr-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                  />
                </div>
              </div>
            )}

            {(authMode === "login" || authMode === "register" || authMode === "forgot") && (
              <div>
                <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">Email</label>
                <div className="relative">
                  <Mail size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500" />
                  <input
                    type="email" value={authEmail} onChange={e => setAuthEmail(e.target.value)}
                    placeholder="you@example.com" required
                    className="w-full pl-9 pr-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                  />
                </div>
              </div>
            )}

            {(authMode === "login" || authMode === "register") && (
              <div>
                <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">Password</label>
                <div className="relative">
                  <Lock size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500" />
                  <input
                    type={showPw ? "text" : "password"} value={authPassword} onChange={e => setAuthPassword(e.target.value)}
                    placeholder="••••••••" required minLength={6}
                    className="w-full pl-9 pr-10 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                  />
                  <button type="button" onClick={() => setShowPw(!showPw)} className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-500 hover:text-zinc-300">
                    {showPw ? <EyeOff size={14} /> : <Eye size={14} />}
                  </button>
                </div>
              </div>
            )}

            {authMode === "verify" && (
              <div>
                <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">Verification Code</label>
                <input
                  type="text" value={verificationToken} onChange={e => setVerificationToken(e.target.value)}
                  placeholder="Enter code from email" required
                  className="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                />
              </div>
            )}

            {authMode === "reset" && (
              <>
                <div>
                  <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">Reset Token</label>
                  <input
                    type="text" value={resetToken} onChange={e => setResetToken(e.target.value)}
                    placeholder="Token from email" required
                    className="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                  />
                </div>
                <div>
                  <label className="text-[11px] font-bold text-zinc-400 uppercase tracking-wider mb-1.5 block">New Password</label>
                  <input
                    type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)}
                    placeholder="••••••••" required minLength={6}
                    className="w-full px-4 py-2.5 bg-zinc-800 border border-zinc-700 rounded-xl text-white text-sm focus:outline-none focus:border-rose-500 transition-colors placeholder-zinc-600"
                  />
                </div>
              </>
            )}

            <button
              type="submit" disabled={loading}
              className="w-full py-3 rounded-xl bg-gradient-to-r from-rose-600 to-red-500 text-white text-sm font-black tracking-wide hover:from-rose-500 hover:to-red-400 disabled:opacity-50 transition-all shadow-lg shadow-rose-950/50 mt-2"
            >
              {loading ? "Please wait..." : authMode === "login" ? "Sign In" : authMode === "register" ? "Create Account" : authMode === "verify" ? "Verify Email" : authMode === "forgot" ? "Send Reset Link" : "Reset Password"}
            </button>
          </form>

          {/* Footer links */}
          <div className="mt-6 flex flex-col gap-2 text-center">
            {authMode === "login" && (
              <>
                <button onClick={() => { setAuthMode("register"); setAuthError(""); }} className="text-xs text-zinc-500 hover:text-rose-400 transition-colors">
                  No account? <span className="text-rose-400 font-bold">Create one</span>
                </button>
                <button onClick={() => { setAuthMode("forgot"); setAuthError(""); }} className="text-xs text-zinc-600 hover:text-zinc-400 transition-colors">
                  Forgot password?
                </button>
              </>
            )}
            {authMode !== "login" && (
              <button onClick={() => { setAuthMode("login"); setAuthError(""); }} className="text-xs text-zinc-500 hover:text-rose-400 transition-colors flex items-center justify-center gap-1">
                <ArrowLeft size={12} /> Back to Sign In
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

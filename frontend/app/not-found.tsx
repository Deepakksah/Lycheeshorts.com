"use client";

import React from "react";
import Link from "next/link";

export default function NotFound() {
  return (
    <div className="min-h-screen bg-slate-950 text-white flex flex-col items-center justify-center p-6 text-center">
      <h1 className="text-6xl font-black text-rose-500 mb-2">404</h1>
      <h2 className="text-xl font-bold mb-4">Page Not Found</h2>
      <p className="text-xs text-slate-400 max-w-sm mb-6">
        The page you are looking for does not exist within Lychee Platform.
      </p>
      <Link
        href="/"
        className="px-5 py-2.5 bg-rose-600 hover:bg-rose-500 text-white font-bold text-xs rounded-xl transition-all"
      >
        Return to Workspace
      </Link>
    </div>
  );
}

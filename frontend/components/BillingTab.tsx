"use client";

import React from "react";
import { SubscriptionPlanResponse, PaymentResponse, UserUsageResponse } from "../lib/api";
import { CreditCard, Zap, CheckCircle2, Clock, BarChart3, Loader2, Star, ShieldCheck } from "lucide-react";

interface BillingTabProps {
  plans: SubscriptionPlanResponse[];
  usage: UserUsageResponse | null;
  paymentHistory: PaymentResponse[];
  billingCycle: "monthly" | "yearly";
  setBillingCycle: (v: "monthly" | "yearly") => void;
  handleCheckout: (planId: string, provider: "Stripe" | "Razorpay") => void;
  loading: boolean;
}

const DEFAULT_PLANS: SubscriptionPlanResponse[] = [
  {
    id: "20000000-0000-0000-0000-000000000001",
    tier: 1,
    name: "Free",
    description: "For testing AI video automation.",
    monthlyPrice: 0,
    yearlyPrice: 0,
    monthlyVideoLimit: 5,
    features: "5 Videos/mo, 3 Shorts/video, Standard Queue"
  },
  {
    id: "20000000-0000-0000-0000-000000000002",
    tier: 2,
    name: "Starter",
    description: "For active YouTubers & TikTokers.",
    monthlyPrice: 19,
    yearlyPrice: 190,
    monthlyVideoLimit: 20,
    features: "20 Videos/mo, 10 Shorts/video, Anti-Copyright"
  },
  {
    id: "20000000-0000-0000-0000-000000000003",
    tier: 3,
    name: "Pro",
    description: "For power creators & agencies.",
    monthlyPrice: 49,
    yearlyPrice: 490,
    monthlyVideoLimit: 100,
    features: "100 Videos/mo, Unlimited Shorts, Auto-Schedule"
  }
];

export const BillingTab: React.FC<BillingTabProps> = ({
  plans, usage, paymentHistory, billingCycle, setBillingCycle, handleCheckout, loading
}) => {
  const displayPlans = plans.length > 0 ? plans : DEFAULT_PLANS;
  const currentTierName = usage?.currentPlanName || usage?.subscriptionTier || "Free Tier";
  const videosUsed = usage?.videosUsedThisMonth ?? usage?.videosThisMonth ?? 0;
  const videoLimit = usage?.monthlyVideoLimit ?? usage?.maxVideosPerMonth ?? 5;

  return (
    <div className="flex-1 flex flex-col min-h-0 overflow-hidden w-full bg-slate-50">
      {/* Top Compact Bar */}
      <div className="bg-white border-b border-slate-200 px-6 py-2.5 flex items-center justify-between shrink-0 shadow-xs">
        <div className="flex items-center gap-2.5">
          <div className="h-7 w-7 rounded-lg bg-gradient-to-r from-rose-600 to-red-500 flex items-center justify-center text-white shadow-xs">
            <CreditCard size={14} />
          </div>
          <div>
            <h1 className="text-xs font-black text-slate-900 tracking-tight">Billing & Plans</h1>
            <p className="text-[10px] text-slate-400 font-bold uppercase">Quotas & Subscriptions</p>
          </div>
        </div>

        {/* Toggle Billing Cycle */}
        <div className="flex items-center gap-1 bg-slate-100 p-0.5 rounded-lg border border-slate-200">
          <button
            onClick={() => setBillingCycle("monthly")}
            className={`px-3 py-1 rounded-md text-[11px] font-black transition-all ${
              billingCycle === "monthly" ? "bg-white text-slate-900 shadow-xs" : "text-slate-500 hover:text-slate-800"
            }`}
          >
            Monthly
          </button>
          <button
            onClick={() => setBillingCycle("yearly")}
            className={`px-3 py-1 rounded-md text-[11px] font-black transition-all ${
              billingCycle === "yearly" ? "bg-white text-slate-900 shadow-xs" : "text-slate-500 hover:text-slate-800"
            }`}
          >
            Yearly <span className="text-emerald-600 font-bold ml-1">−20%</span>
          </button>
        </div>
      </div>

      {/* Main Full Page Scrollable Container */}
      <div className="flex-1 overflow-y-auto p-5 w-full space-y-5">
        {/* Compact Usage Card */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-xs p-4 w-full">
          <div className="flex items-center justify-between mb-3">
            <h3 className="text-[11px] font-black text-slate-700 uppercase tracking-wider flex items-center gap-1.5">
              <BarChart3 size={13} className="text-rose-500" /> Usage Telemetry
            </h3>
            <span className="text-[10px] font-bold text-slate-400">
              {Math.round((videosUsed / (videoLimit || 1)) * 100)}% Quota Used
            </span>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-3">
            {[
              { label: "Plan", value: currentTierName, color: "text-violet-600", bg: "bg-violet-50/60" },
              { label: "Videos Used", value: `${videosUsed} / ${videoLimit}`, color: "text-rose-600", bg: "bg-rose-50/60" },
              { label: "Shorts Built", value: `${videosUsed * 4}`, color: "text-blue-600", bg: "bg-blue-50/60" },
              { label: "Dispatch", value: "Active", color: "text-emerald-600", bg: "bg-emerald-50/60" },
            ].map(({ label, value, color, bg }) => (
              <div key={label} className={`p-2.5 rounded-lg border border-slate-200/80 ${bg} text-center`}>
                <p className={`text-base font-black ${color}`}>{value}</p>
                <p className="text-[9px] text-slate-500 font-bold uppercase tracking-wider mt-0.5">{label}</p>
              </div>
            ))}
          </div>

          {/* Progress Bar */}
          <div className="h-2 bg-slate-100 rounded-full overflow-hidden border border-slate-200/60">
            <div
              className="h-full bg-gradient-to-r from-rose-600 to-red-500 rounded-full transition-all"
              style={{ width: `${Math.min(100, (videosUsed / (videoLimit || 1)) * 100)}%` }}
            />
          </div>
        </div>

        {/* Compact Pricing Grid */}
        <div>
          <h2 className="text-sm font-black text-slate-900 mb-3">Select Subscription Tier</h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 w-full">
            {displayPlans.map((plan, i) => {
              const mPrice = plan.monthlyPrice ?? plan.monthlyPriceUsd ?? 0;
              const yPrice = plan.yearlyPrice ?? plan.yearlyPriceUsd ?? 0;
              const price = billingCycle === "monthly" ? mPrice : Math.round(yPrice / 12);
              const isPopular = plan.name.includes("Starter") || plan.name.includes("Pro") || i === 1;

              return (
                <div
                  key={plan.id}
                  className={`bg-white rounded-xl border shadow-xs p-4 flex flex-col justify-between relative transition-all hover:border-slate-300 ${
                    isPopular ? "border-rose-300 ring-1 ring-rose-500/20" : "border-slate-200"
                  }`}
                >
                  {isPopular && (
                    <div className="absolute -top-2.5 left-1/2 -translate-x-1/2 px-2.5 py-0.5 bg-rose-600 text-white text-[9px] font-black rounded-full shadow-xs flex items-center gap-1 uppercase tracking-wider">
                      <Star size={8} fill="white" /> Popular
                    </div>
                  )}

                  <div>
                    <div className="flex items-start justify-between mb-2">
                      <div>
                        <h3 className="font-black text-slate-900 text-sm">{plan.name}</h3>
                        <p className="text-[10px] text-slate-400 leading-tight mt-0.5">{plan.description}</p>
                      </div>
                    </div>

                    <div className="mb-3 pb-2.5 border-b border-slate-100 flex items-baseline gap-1">
                      <span className="text-2xl font-black text-slate-900">${price}</span>
                      <span className="text-[10px] text-slate-400 font-bold">/mo</span>
                      {billingCycle === "yearly" && yPrice > 0 && (
                        <span className="text-[9px] text-emerald-600 font-bold ml-auto">(${yPrice}/yr)</span>
                      )}
                    </div>

                    <div className="space-y-1.5 mb-4">
                      <div className="flex items-center gap-1.5 text-[11px] text-slate-700 font-medium">
                        <CheckCircle2 size={12} className="text-emerald-500 shrink-0" />
                        <span><strong>{plan.monthlyVideoLimit ?? plan.maxVideosPerMonth ?? 20}</strong> Videos/mo</span>
                      </div>
                      <div className="flex items-center gap-1.5 text-[11px] text-slate-700 font-medium">
                        <CheckCircle2 size={12} className="text-emerald-500 shrink-0" />
                        <span>AI Short Extraction</span>
                      </div>
                      <div className="flex items-center gap-1.5 text-[11px] text-slate-700 font-medium">
                        <ShieldCheck size={12} className="text-violet-500 shrink-0" />
                        <span>Anti-Copyright Filter</span>
                      </div>
                      {plan.features && plan.features.split(",").map(f => (
                        <div key={f} className="flex items-center gap-1.5 text-[11px] text-slate-700 font-medium">
                          <CheckCircle2 size={12} className="text-emerald-500 shrink-0" />
                          <span className="truncate">{f.trim()}</span>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="space-y-1.5 pt-3 border-t border-slate-100">
                    <button
                      onClick={() => handleCheckout(plan.id, "Stripe")}
                      disabled={loading || mPrice === 0}
                      className={`w-full py-2 rounded-lg text-[11px] font-black transition-all flex items-center justify-center gap-1.5 ${
                        mPrice === 0
                          ? "bg-slate-100 text-slate-400 cursor-not-allowed"
                          : isPopular
                          ? "bg-rose-600 hover:bg-rose-500 text-white shadow-xs"
                          : "bg-slate-900 text-white hover:bg-slate-800"
                      }`}
                    >
                      {loading ? <Loader2 size={11} className="animate-spin" /> : <CreditCard size={11} />}
                      {mPrice === 0 ? "Free Active" : "Pay Stripe"}
                    </button>
                    {mPrice > 0 && (
                      <button
                        onClick={() => handleCheckout(plan.id, "Razorpay")}
                        disabled={loading}
                        className="w-full py-2 rounded-lg text-[11px] font-black bg-indigo-600 text-white hover:bg-indigo-500 transition-all flex items-center justify-center gap-1.5"
                      >
                        <Zap size={11} /> Pay Razorpay
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* Compact Payment History */}
        {paymentHistory.length > 0 && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-xs p-4 w-full">
            <h3 className="text-[11px] font-black text-slate-700 uppercase tracking-wider mb-3 flex items-center gap-1.5">
              <Clock size={13} className="text-slate-400" /> Transaction History
            </h3>

            <div className="overflow-x-auto w-full">
              <table className="w-full text-[11px]">
                <thead className="bg-slate-50 border-b border-slate-200 text-slate-500">
                  <tr>
                    {["Provider", "Ref ID", "Status", "Amount", "Date"].map(h => (
                      <th key={h} className="text-left px-3 py-2 font-bold uppercase text-[9px]">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {paymentHistory.map(p => (
                    <tr key={p.id} className="hover:bg-slate-50/60">
                      <td className="px-3 py-2 font-bold text-slate-800">{p.provider}</td>
                      <td className="px-3 py-2 font-mono text-slate-400 text-[10px]">{p.providerPaymentId || "N/A"}</td>
                      <td className="px-3 py-2">
                        <span className={`px-2 py-0.5 rounded-full text-[9px] font-bold ${p.status === "Completed" ? "bg-emerald-50 text-emerald-600" : "bg-slate-100 text-slate-500"}`}>
                          {p.status}
                        </span>
                      </td>
                      <td className="px-3 py-2 font-black text-slate-900">{p.currency} {p.amount.toFixed(2)}</td>
                      <td className="px-3 py-2 text-slate-400 text-[10px]">{new Date(p.createdAtUtc).toLocaleDateString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

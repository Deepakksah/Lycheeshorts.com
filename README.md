# 🍒 Lychee — AI Short-Form Video Automation & Multi-Platform Publishing System

**Lychee** (formerly ShortsAI) is an enterprise-grade, end-to-end AI-powered video automation platform. It turns long-form content (YouTube videos, podcasts, streams, local video files) into high-virality short-form videos optimized for **YouTube Shorts**, **Instagram Reels**, and **Facebook Reels**.

---

## 📑 Table of Contents

1. [Architectural Overview](#-architectural-overview)
2. [Folder & Workspace Structure](#-folder--workspace-structure)
3. [Key Platform Features](#-key-platform-features)
4. [Technology Stack](#-technology-stack)
5. [Database & Entity Schema](#-database--entity-schema)
6. [API Endpoints Reference](#-api-endpoints-reference)
7. [Getting Started & Installation](#-getting-started--installation)
8. [Background Processing & Hangfire](#-background-processing--hangfire)
9. [Social Media Integrations & OAuth](#-social-media-integrations--oauth)
10. [Troubleshooting & Maintenance](#-troubleshooting--maintenance)

---

## 🏗️ Architectural Overview

```
 ┌───────────────────────────────────────────────────────────┐
 │                   Lychee Frontend (Next.js 15)            │
 │  - Workspace & Library    - Scheduler & Calendar          │
 │  - Social Accounts        - Billing & Subscriptions       │
 │  - Admin Suite            - Stateful Collapsible Sidebar  │
 └──────────────────────────────┬────────────────────────────┘
                                │ REST API (HTTP / JSON / FormData)
 ┌──────────────────────────────▼────────────────────────────┐
 │               Lychee.Publisher.Api (.NET 9)                │
 │  - Minimal API Endpoints  - JWT Bearer Authentication     │
 │  - Global Rate Limiting   - Exception Interceptor & Logger│
 └──────────────────────┬────────────────────────────────────┘
                        │
 ┌──────────────────────┼────────────────────────────────────┐
 │ Application Layer    │ Infrastructure Layer               │
 │ - Core Interfaces    │ - EF Core & SQLite/Postgres DB     │
 │ - DTOs & Validation  │ - Hangfire Minutely Scheduler      │
 │ - Video Workflows    │ - FFmpeg Processing Pipeline       │
 └──────────────────────┴────────────────────────────────────┘
```

---

## 📁 Folder & Workspace Structure

```
LycheeWeb/
├── README.md                                # Full Project Documentation
├── backend/
│   ├── Lychee.Publisher.Api/                # Entry point API & Endpoints
│   │   ├── Lychee.Publisher.Api.Endpoints/  # Feature-grouped endpoint definitions
│   │   │   ├── AdminEndpoints.cs
│   │   │   ├── AuthEndpoints.cs
│   │   │   ├── PaymentEndpoints.cs
│   │   │   ├── ScheduleEndpoints.cs
│   │   │   ├── SocialAccountEndpoints.cs
│   │   │   └── VideoEndpoints.cs
│   │   ├── Program.cs                       # Services DI & App Pipeline
│   │   ├── appsettings.json                 # Configuration settings
│   │   └── FileLogger.cs                    # File-based logging utility
│   ├── Lychee.Publisher.Application/        # Application abstractions & logic
│   ├── Lychee.Publisher.Domain/             # Core entities & Enums
│   └── Lychee.Publisher.Infrastructure/     # EF DbContext, Seeder, Jobs & Services
└── frontend/                                # Next.js 15 Frontend
    ├── app/
    │   ├── globals.css                      # Global styles & Tailwind directive
    │   ├── layout.tsx                       # Root layout
    │   └── page.tsx                         # Main dashboard container & state
    ├── components/
    │   ├── AdminTab.tsx                     # Admin management & analytics
    │   ├── AuthModal.tsx                    # Login, Register, Verify & Reset modal
    │   ├── BillingTab.tsx                   # Pricing plans & payment history
    │   ├── Header.tsx                       # Top navigation bar
    │   ├── SchedulerTab.tsx                 # Interactive scheduling calendar & list
    │   ├── Sidebar.tsx                      # Stateful collapsible sidebar (Menu icon)
    │   ├── SocialTab.tsx                    # Social account connector (YouTube, IG, FB)
    │   └── WorkspaceTab.tsx                 # Video import, upload & AI clip viewer
    └── lib/
        └── api.ts                           # TypeScript fetch API client & types
```

---

## ⚡ Key Platform Features

1. **Video Import & Upload**:
   - Direct YouTube URL import pipeline.
   - Multipart file upload for high-bitrate video formats (`.mp4`, `.mov`, `.avi`).
2. **AI Clipping & Virality Engine**:
   - Automated timestamp extraction for high-viral probability scenes.
   - Dynamic virality score calculation (`0-100%`).
   - Automated title, description, and hashtag generation.
3. **Automated Scheduler**:
   - Schedule clips for future dates and times across multiple platforms.
   - Background minutely worker via Hangfire to trigger automated posting.
4. **Anti-Copyright & Optimization**:
   - FFmpeg audio pitch micro-shifting and video aspect framing to pass automated content matching filters.
5. **Multi-Channel Management**:
   - Link multiple YouTube Channels, Instagram Profiles, and Facebook Pages.
6. **Monetization & Billing**:
   - Multi-tier subscription plans (Free, Starter, Pro, Business).
   - Dual payment provider support (Stripe & Razorpay).
7. **Admin Control Suite**:
   - Live system health metrics and aggregate counts.
   - User table with role elevation (`User`, `Admin`, `Moderator`) and tier overrides.
   - Subscriptions log and revenue breakdowns by provider.
   - Audit trail for admin actions.

---

## 🧰 Technology Stack

| Layer | Technology |
|---|---|
| **Frontend Framework** | Next.js 15 (App Router), React 19, TypeScript |
| **UI Components & Icons**| TailwindCSS, Lucide Icons |
| **Backend Framework** | .NET 9 Minimal API |
| **ORM & Database** | Entity Framework Core 9, SQLite / PostgreSQL |
| **Task Scheduler** | Hangfire |
| **Authentication** | JWT Bearer Authentication, ASP.NET Core Identity Hasher |
| **Media Processing** | FFmpeg CLI / Media Toolkit |

---

## 🗄️ Database & Entity Schema

### Core Domain Entities:
- **`User`**: System account, credentials, email verification token, password reset tokens, tier, role.
- **`Video`**: Source video metadata, YouTube URL or local file path, overall processing status.
- **`ShortClip`**: Generated short clip with start/end timestamps, virality score, hashtags, and title.
- **`Schedule`**: Target clip ID, platform ID, scheduled date/time, publish status (`Scheduled`, `Publishing`, `Published`, `Failed`), failure reason.
- **`SocialAccount`**: Platform identifier, channel/page display name, handle, encrypted OAuth access token.
- **`SubscriptionPlan`**: Plan tier name, monthly/yearly pricing, video/shorts monthly limits.
- **`Payment`**: Transaction history, provider (`Stripe`/`Razorpay`), amount, currency, status.
- **`AuditLog`**: System action logs for compliance and debugging.

---

## 📡 API Endpoints Reference

### 🔐 Authentication (`/api/v1/auth`)
- `POST /login` — Authenticate and receive JWT token.
- `POST /register` — Register a new account.
- `GET /me` — Fetch current user context.
- `POST /verify-email` — Verify email token.
- `POST /forgot-password` — Request password reset email.
- `POST /reset-password` — Reset password using token.

### 🎬 Videos (`/api/v1/videos`)
- `GET /` — List all uploaded/imported videos for user.
- `POST /import-youtube` — Import video from YouTube link.
- `POST /upload` — Upload video file (`multipart/form-data`).
- `POST /{id}/process` — Trigger AI clip generation pipeline.
- `GET /{id}/shorts` — Get generated short clips for a video.

### 📅 Schedules (`/api/v1/schedules`)
- `GET /` — Get user's publishing schedule list.
- `POST /` — Schedule a short clip for publishing.
- `PUT /{id}` — Reschedule an existing item.
- `DELETE /{id}` — Cancel/delete a scheduled item.

### 🌐 Social Accounts (`/api/v1/social-accounts`)
- `GET /` — List linked social accounts.
- `POST /connect` — Connect a new social channel.
- `DELETE /{id}` — Unlink a social channel.

### 💳 Payments & Billing (`/api/v1/payments`)
- `GET /plans` — List available subscription tiers.
- `GET /usage` — Fetch user's current monthly usage quota.
- `POST /checkout` — Initiate Stripe or Razorpay checkout session.
- `GET /history` — View payment history.

### 🛡️ Admin Suite (`/api/v1/admin`)
- `GET /system-stats` — Get platform-wide overview metrics.
- `GET /users` — Paginated user management list with search.
- `PUT /users/{id}/role` — Change user role.
- `PUT /users/{id}/tier` — Override user subscription tier.
- `DELETE /users/{id}` — Permanently delete user.
- `GET /subscriptions` — View platform subscriptions log.
- `GET /revenue` — Revenue statistics and provider breakdown.
- `GET /audit-logs` — System audit trails.
- `GET /analytics` — Conversion funnel and active user analytics.

---

## 🚀 Getting Started & Installation

### 1. Prerequisites
- **.NET 9.0 SDK**
- **Node.js 18+** & **npm**
- **FFmpeg** installed locally and added to PATH.

### 2. Backend Setup
```bash
# Navigate to the API directory
cd backend/Lychee.Publisher.Api

# Restore dependencies
dotnet restore

# Run the API
dotnet run
```
> The API starts at **`http://localhost:5031`**.

### 3. Frontend Setup
```bash
# Navigate to the frontend directory
cd frontend

# Install dependencies
npm install

# Run dev server
npm run dev
```
> Access the application at **`http://localhost:3000`**.

---

## ⏱️ Background Processing & Hangfire

The system relies on **Hangfire** to manage background tasks and automated publishing.
- **Hangfire Dashboard**: Accessible in development mode at `http://localhost:5031/hangfire`.
- **Minutely Publisher**: `PublishScheduledClipsJob` runs every minute (`Cron.Minutely()`) to pick up pending clips marked for current time, process anti-copyright transformations, and dispatch them to connected social channels.

---

## 🔑 Social Media Integrations & OAuth

To configure YouTube Shorts publishing in production:
1. Open Google Cloud Console and select your project (`quantum-enigma-501118-i9`).
2. Enable **YouTube Data API v3**.
3. Configure OAuth Consent Screen & credentials.
4. Update `backend/Lychee.Publisher.Api/appsettings.json`:
   ```json
   "GoogleOAuth": {
     "ClientId": "YOUR_CLIENT_ID",
     "ClientSecret": "YOUR_CLIENT_SECRET"
   }
   ```

---

## 🔧 Troubleshooting & Maintenance

- **DB Reset / Reseed**: Delete `publisher.db` in `backend/Lychee.Publisher.Api` and restart the backend. Migration and seeders will run automatically.
- **Frontend Type Check**: Run `npx tsc --noEmit` in `frontend` to verify TypeScript definitions.
- **Build Verification**: Run `npm run build` in `frontend` and `dotnet build` in `backend/Lychee.Publisher.Api`.

---

© **Lychee Platform** — Automated AI Short-Form Content Suite.

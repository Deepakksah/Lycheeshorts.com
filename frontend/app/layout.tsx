import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Lychee Web - AI Short-Form Video Platform",
  description: "AI-powered short-form video generation and multi-platform publishing platform",
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body className="antialiased">{children}</body>
    </html>
  );
}

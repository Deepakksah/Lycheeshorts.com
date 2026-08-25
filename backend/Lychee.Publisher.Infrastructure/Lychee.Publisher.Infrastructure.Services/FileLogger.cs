using System;
using System.IO;

namespace Lychee.Publisher.Infrastructure.Services;

public static class FileLogger
{
	private static readonly object LogLock;

	private static readonly string LogsDir;

	static FileLogger()
	{
		LogLock = new object();
		_003C_003Ey__InlineArray7<string> buffer = default(_003C_003Ey__InlineArray7<string>);
		buffer[0] = AppDomain.CurrentDomain.BaseDirectory;
		buffer[1] = "..";
		buffer[2] = "..";
		buffer[3] = "..";
		buffer[4] = "..";
		buffer[5] = "..";
		buffer[6] = "logs";
		LogsDir = Path.Combine(buffer);
		try
		{
			string fullPath = Path.GetFullPath(LogsDir);
			if (!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}
		}
		catch
		{
		}
	}

	public static void LogBackendError(string source, string message, Exception? ex = null)
	{
		WriteLog("backend-error.log", source, message, ex);
	}

	public static void LogFrontendError(string message, string? stack = null, string? route = null)
	{
		string message2 = $"[ROUTE: {route ?? "unknown"}]\n{message}{(string.IsNullOrEmpty(stack) ? "" : ("\nStack:\n" + stack))}";
		WriteLog("frontend-error.log", "FrontendClient", message2);
	}

	private static void WriteLog(string fileName, string source, string message, Exception? ex = null)
	{
		lock (LogLock)
		{
			try
			{
				string fullPath = Path.GetFullPath(LogsDir);
				if (!Directory.Exists(fullPath))
				{
					Directory.CreateDirectory(fullPath);
				}
				string path = Path.Combine(fullPath, fileName);
				string value = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
				string text = $"[{value}] [{source.ToUpper()}]\n{message}\n";
				if (ex != null)
				{
					text += $"Exception: {ex.GetType().FullName}: {ex.Message}\nStackTrace:\n{ex.StackTrace}\n";
				}
				text = text + new string('-', 80) + "\n";
				File.AppendAllText(path, text);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[LOGGED TO " + fileName + "] " + message);
				Console.ResetColor();
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Failed to write log file: " + ex2.Message);
			}
		}
	}
}

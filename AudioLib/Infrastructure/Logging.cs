using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AudioLib.Infrastructure
{
	public static class Logging
	{
		public enum LogLevel
		{
			NoLogging,
			Fatal,
			Exception,
			Error,
			Warn,
			Info
		}

		static object lockOjbect = new object();
		static volatile List<string> Data;
		static Thread BackgroundThread;
		static bool Running;
		static FileStream Stream;
		static StreamWriter Writer;

		public static LogLevel Level { get; private set; }

		public static void Start(LogLevel logLevel, string outputDirectory, string fileTemplate)
		{
			if (Running && BackgroundThread != null)
			{
				Stop();
			}

			Data = new List<string>();
			Level = logLevel;
			var file = Path.Combine(outputDirectory, fileTemplate + "-" + DateTime.Now.ToString("yyyyMMDD-HHmmss") + ".log");
			Directory.CreateDirectory(outputDirectory);
			if(File.Exists(file))
				Stream = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Read);
			else
				Stream = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);

			Writer = new StreamWriter(Stream);

			BackgroundThread = new Thread(FlushLoop) { IsBackground = true, Priority = ThreadPriority.Lowest };
			BackgroundThread.Start();
			Running = true;
		}

		public static void Stop()
		{
			Running = false;
			BackgroundThread.Join();
			Stream.Close();
		}

		static void FlushLoop()
		{
			while(Running)
			{
				Thread.Sleep(1000);
				FlushBuffer();
			}
		}

		public static void FlushBuffer()
		{
			List<string> toWrite = null;
			if (Data.Count == 0)
				return;

			lock (lockOjbect)
			{
				toWrite = Data;
				Data = new List<string>();
			}

			foreach (var message in toWrite)
				Writer.WriteLine(message);

			Writer.Flush();
		}

		public static void LogInfo<T>(string message)
		{
			PushMessage(message, typeof(T).FullName, LogLevel.Info);
		}

		public static void LogInfo(string message, string location)
		{
			PushMessage(message, location, LogLevel.Info);
		}


		public static void LogWarning<T>(string message)
		{
			PushMessage(message, typeof(T).FullName, LogLevel.Warn);
		}

		public static void LogWarning(string message, string location)
		{
			PushMessage(message, location, LogLevel.Warn);
		}


		public static void LogError<T>(string message)
		{
			PushMessage(message, typeof(T).FullName, LogLevel.Error);
		}

		public static void LogError(string message, string location)
		{
			PushMessage(message, location, LogLevel.Error);
		}


		public static void LogException<T>(Exception ex)
		{
			LogException(ex, typeof(T).FullName);
		}

		public static void LogException(Exception ex, string location)
		{
			if (Level < LogLevel.Exception)
				return;

			var sb = new StringBuilder();
			while(ex != null)
			{
				sb.AppendLine(ex.Message);
				sb.AppendLine(ex.StackTrace);
				ex = ex.InnerException;
			}

			sb.AppendLine();
			PushMessage(sb.ToString(), location, LogLevel.Exception);
		}

		static void PushMessage(string message, string location, LogLevel type)
		{
			if (Level < type)
				return;

			var entry = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff} : {1} : {2}] > {3}", DateTime.Now, location, type.ToString(), message);
			lock (lockOjbect)
			{
				Data.Add(entry);
			}
		}
	}
}

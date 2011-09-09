using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace LogNet
{
	/// <remarks>
	/// A static class which can be used to log debug information.
	/// </remarks>
	public static class DebugLogger
	{
		#region Variables and enumerations

		private static String appName = "MyApp";
		private static String fileName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\MyApp.log";
		private static LoggingLevels threshold = LoggingLevels.Critical;
		private static bool enabled = true;
		private static StringBuilder buffer = new StringBuilder();

		#endregion

		#region Public Properties

		/// <summary>
		///   Sets or returns the name of the application that we are logging for.
		///   Errors will be logged in "My Documents\(AppName).log".
		/// </summary>
		public static String AppName {
			get {
				return appName;
			}
			set {
				if (value.IndexOfAny("\\.".ToCharArray()) >= 0)
					throw new ArgumentException(ResStrings.GetString("InvalidAppNameCharacters"));
				appName = value;
				fileName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
					"\\" + appName + ".log";
				buffer.Length = 0;
				buffer.Append("\r\n");
				buffer.Append("------------------------------------------------------------\r\n");
				buffer.Append("\r\n");
			}
		}

		/// <summary>
		/// Get or set the threshold logging level. Note that we will log all events at this level or
		/// higher. This defaults to "Critical".
		/// </summary>
		public static LoggingLevels Threshold {
			get { return threshold; }
			set { threshold = value; }
		}
		
		/// <summary>
		/// Get or set whether logging is enabled. This defaults to "True".
		/// </summary>
		public static bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// This static method logs a single action to the output file. This will
		/// appear in the file looking like:
		/// <code>2006-12-31 12:34:56 : The debug message.</code>
		/// </summary>
		/// <param name="source">The source object or method.</param>
		/// <param name="what">The debug message we wish to record.</param>
		/// <param name="priority">What priority should this be recorded at.</param>
		public static void LogAction(string source, 
                                     string what, 
                                     LoggingLevels priority) {
			if (!enabled) return;
			if (priority < threshold) return;
			buffer.AppendLine(GetReport(source, what, ""));
			WriteBufferToFile();
		}

		/// <summary>
		/// This static method logs a single action to the output file. This will
		/// appear in the file looking like:
		/// <code>2006-12-31 12:34:56 : The debug message.</code>
		/// </summary>
		/// <param name="source">The source object or method.</param>
		/// <param name="what">The debug message we wish to record.</param>
		/// <param name="data">Additional data to serialise and record.</param>
		/// <param name="priority">What priority should this be recorded at.</param>
		public static void LogAction(string source, 
                                     string what,
                                     object data, 
                                     LoggingLevels priority) {
			if (!enabled) return;
			if (priority < threshold) return;
			buffer.AppendLine(GetReport(source, what, Formatters.ASCIIfy(data)));
			WriteBufferToFile();
		}

		/// <summary>
		/// Log the current call stack to the log file.
		/// </summary>
		public static void LogCallStack(string info) {
			LogThreadState(Thread.CurrentThread, info);
		}
		
		/// <summary>
		/// Log a summary of the state of the current System.Threading.Thread.
		/// </summary>
		/// <param name="info">Informational string to be pre-pended to output.</param>
		public static void LogThreadState(string info) {
			LogThreadState(Thread.CurrentThread, info);
		}
		
		/// <summary>
		/// Log a summary of the state of the specified System.Threading.Thread.
		/// </summary>
		/// <param name="t">The thread to log.</param>
		/// <param name="info">Informational string to be pre-pended to output.</param>
		public static void LogThreadState(Thread t, 
                                          string info) {
			if (!enabled || t == null)
				return;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GetReport(t, info, "", ""));
			sb.AppendLine("   System.Threading.ThreadState = " + t.ThreadState.ToString());
			sb.AppendLine("   Call Stack:");
			sb.Append(ListCallStack(t));
			buffer.AppendLine(sb.ToString());
			WriteBufferToFile();
		}
		
		/// <summary>
		/// Log a summary of the state of all threads.
		/// </summary>
		/// <param name="info">Informational string to be pre-pended to output.</param>
		public static void LogThreadStates(string info) {
			if (!enabled) return;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GetReport(info, "", ""));
			foreach (ProcessThread pt in Process.GetCurrentProcess().Threads) {
				sb.AppendLine(GetThreadState(pt));
			}
			buffer.AppendLine(sb.ToString());
			WriteBufferToFile();
		}
		
		#endregion
		
		#region Private methods
		
		private static string GetReport(string source, 
                                        string what, 
                                        string extra) {
			return GetReport(Thread.CurrentThread, source, what, extra);
		}
		
		private static string GetReport(Thread t, 
                                        string source, 
                                        string what, 
                                        string extra) {
			string s = DateTime.Now.ToString("s") + " \t" +
				t.Name + "(" + t.ManagedThreadId.ToString() + ")\t" +
				source + "\t" + what + "\t" + extra;
			return s;
		}
		
		private static string ListCallStack(Thread t) {
			try {
				StackTrace callStack = new StackTrace(t, true);
				StringBuilder sb = new StringBuilder();
				string s = "";
				bool lastLineWasDots = false;
				bool haveLoggedLocalFunc = false;
				foreach (StackFrame frame in callStack.GetFrames()) {
					s = GetMethodNameToLog(frame, ref haveLoggedLocalFunc);
					if (string.IsNullOrEmpty(s)) {
						//Ignore
					} else if (s == "...") {
						if (!lastLineWasDots)
							sb.AppendLine("      ...");
						lastLineWasDots = true;
					} else {
						sb.AppendLine("      " + s);
						lastLineWasDots = false;
					}
				}
				return sb.ToString();
			} catch (Exception ex) {
				return "      No call stack available. " + ex.Message;
			}
		}
		
		private static string GetMethodNameToLog(StackFrame f, 
                                                 ref bool stripSysMethods) {
			MethodBase method = f.GetMethod();
			//Don't log stuff that's in this assembly.
			if (method.DeclaringType.Assembly == Assembly.GetExecutingAssembly())
				return "";
			//Stuff in system assemblies we summarise after any local method.
			if (IsSystemMethod(method)) {
				if (stripSysMethods)
					return "...";
			} else {
				stripSysMethods = true;
			}
			return GetMethodDescription(method) + GetShortFileName(f) + GetLineNumber(f);
		}
		
		private static bool IsSystemMethod(MethodBase mb) {
			return mb.DeclaringType.FullName.StartsWith("System", StringComparison.CurrentCultureIgnoreCase);
		}
		
		private static string GetMethodDescription(MethodBase method) {
			StringBuilder sb = new StringBuilder();
			sb.Append(method.DeclaringType.FullName + "." + method.Name + "(");
			ParameterInfo[] pInfos = method.GetParameters();
			if (pInfos.Length > 0) {
				foreach (ParameterInfo pi in pInfos) {
					sb.Append(pi.ParameterType.Name + ", ");
				}
				sb.Remove(sb.Length - 2, 2);
			}
			sb.Append(")");
			return sb.ToString();
		}
		
		private static string GetShortFileName(StackFrame f) {
			string file = f.GetFileName();
			if (string.IsNullOrEmpty(file))
				return string.Empty;
			string here = Path.GetFileName(Path.GetDirectoryName(file));
			return " in '" + Path.Combine(here, Path.GetFileName(file)) + "'";
		}
		
		private static string GetLineNumber(StackFrame f) {
			int i = f.GetFileLineNumber();
			if (i == 0)
				return string.Empty;
			return " at line " + i.ToString();
		}
		
		private static void WriteBufferToFile() {
			try {
				using (StreamWriter w = File.AppendText(fileName)) {
					w.Write(buffer.ToString());
					buffer.Length = 0;
					w.Flush();
					w.Close();
				}
			} catch {
				//Do nothing.
			}
		}
		
		private static string GetThreadState(ProcessThread pt) {
			if (pt == null)
				return "";
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("   Thread ID='" + pt.Id + "'  Start=" + pt.StartTime.ToString("s"));
			sb.Append("      System.Diagnostics.ThreadState = " + pt.ThreadState.ToString());
			if (pt.ThreadState == System.Diagnostics.ThreadState.Wait)
				sb.Append(" (ThreadWaitReason=" + pt.WaitReason.ToString() + ")");
			return sb.ToString();
		}

        #endregion
	}
}

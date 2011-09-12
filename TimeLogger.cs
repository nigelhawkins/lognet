// Part of the LogNet logging library.
// Copyright (c) 2011 Nigel Hawkins
//
// Licensed under the Gnu LGPL version 3.
// See http://www.gnu.org/licenses/lgpl.html

using System;

namespace LogNet
{
	/// <summary>
	/// The TimeLogger class is a wrapper around the TimeMonitor class that logs the
	/// elapsed time every few events.
	/// </summary>
	public class TimeLogger
	{
		
		#region Variables
		
		private int logInterval = 100;
		private TimeMonitor monitor = new TimeMonitor();
		private string logName;
		private LoggingLevels logLevel = LoggingLevels.Informational;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Get or set the number of events between each log. The default is 100.
		/// </summary>
		public int LogInterval {
			get { return logInterval; }
			set { 
				if (value <= 0)
					value = 100;
				logInterval = value; 
			}
		}
		
		/// <summary>
		/// Get or set a flag indicating if logging is enabled. The default is true.
		/// </summary>
		public bool Enabled {
			get { return monitor.Enabled; }
			set { monitor.Enabled = value; }
		}
		
		/// <summary>
		/// Get the name of this logger to be recorced in the log file.
		/// </summary>
		public string LogName {
			get { return logName; }
			set { logName = value; }
		}
		
		/// <summary>
		/// Get or set the level at which we should log. The default is "Informational".
		/// </summary>
		public LoggingLevels LogLevel {
			get { return logLevel; }
			set { logLevel = value; }
		}
		
		#endregion
		
		/// <summary>
		/// Default constuctor.
		/// </summary>
		public TimeLogger() {
		}
		
		#region Public methods
		
		/// <summary>
		/// Start timing a new event.
		/// </summary>
		public void Start() {
			monitor.Start();
		}
		
		/// <summary>
		/// Finish timing the current event, and log if necessary.
		/// </summary>
		public void Finish() {
			monitor.Finish();
			if (monitor.EventCount < logInterval)
				return;
			DebugLogger.LogAction("TimeLogger(" + logName + ")",
			                      "#Events=" + monitor.EventCount.ToString() + " Avg.Time=" + monitor.AverageMilliSecs.ToString(), 
			                      logLevel);
			monitor.Clear();
		}
		
		#endregion
		
	}
}

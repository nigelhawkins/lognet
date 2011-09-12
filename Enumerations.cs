// Part of the LogNet logging library.
// Copyright (c) 2011 Nigel Hawkins
//
// Licensed under the Gnu LGPL version 3.
// See http://www.gnu.org/licenses/lgpl.html

namespace LogNet
{
	/// <summary>
	/// Definition of the levels of debug information we may want to log.
	/// </summary>
	public enum LoggingLevels : int {
		/// <summary>Not very important. For information only.</summary>
		Informational = 0,
		/// <summary>The warning level is appropriate for events that are 
        /// important but which are unlikely to cause problems.</summary>
		Warning,
		/// <summary>The Alarm level is appropriate for events that may cause 
        /// problems.</summary>
		Alarm,
		/// <summary>Items at the Critical level should almost always be 
        /// logged.</summary>
		Critical
	}
	
	/// <summary>
	/// This enumeration indicates when an error message should be displayed.
	/// </summary>
	public enum DisplayOption {
		/// <summary>
		/// The error message should always be displayed on the screen.
		/// </summary>
		Always,
		/// <summary>
		/// The error message should never be displayed on the screen.
		/// </summary>
		Never,
		/// <summary>
		/// The error message should only be displayed on the screen if it is
		/// recorded in the log. By default, repeated messages are only logged
		/// once per day.
		/// </summary>
		OnlyIfLogged
	}

}

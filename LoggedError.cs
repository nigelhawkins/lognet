// Part of the LogNet logging library.
// Copyright (c) 2011 Nigel Hawkins
//
// Licensed under the Gnu LGPL version 3.
// See http://www.gnu.org/licenses/lgpl.html

using System;

namespace LogNet 
{

	/// <remarks>
	/// This class simply remembers the last time we saw a specific error
	/// message. It is not visible from outside the assembly.
	/// </remarks>
	internal sealed class LoggedError {
		private DateTime timeRecorded = System.DateTime.Now;
		private string errorMessage;
			
		/// <summary>
		/// The default constructor.
		/// </summary>
		public LoggedError() {
		}
			
		/// <summary>
		/// This property returns the age of this instance.
		/// </summary>
		public TimeSpan Age {
			get { return DateTime.Now - timeRecorded; }
		}
		
		/// <summary>
		/// This property gets or sets the error message.
		/// </summary>
		public string ErrorMessage {
			get { return errorMessage; }
			set { errorMessage = value; }
		}
	}
}


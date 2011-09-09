using System;
using System.Collections.Generic;
using System.Threading;

namespace LogNet
{
	/// <remarks>
	/// This class implements a generic error logging framework. It remembers all
	/// of the error messages it has seen, and ensures that repeated errors are not
	/// logged more than once per day.
	/// </remarks>
	public static  class ErrorLogger
	{
		#region Variables and Enumerations

		private static List<LoggedError> loggedErrors = new List<LoggedError>();
		private static string appName = "";
		private static string fileName = "";
		private static TimeSpan ageThreshold = TimeSpan.FromDays(1);
		
		#endregion

		#region Public Properties

		/// <summary>
		///   Sets or returns the name of the application that we are logging for.
		///   Errors will be logged in "My Documents\(AppName).err".
		/// </summary>
		public static String AppName {
			get {
				return appName;
			}
			set {
				if (value.IndexOfAny("\\.".ToCharArray()) >= 0) {
					throw new ArgumentException(ResStrings.GetString("InvalidAppNameCharacters"));
				}
				appName = value;
				fileName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
					"\\" + appName + ".err";
			}
		}
		
		#endregion

		/// <summary>
		/// Default static constructor.
		/// </summary>
		static ErrorLogger() {
			AppName = "MyApp";
		}

		#region Public Functions

		/// <summary>
		/// Determines whether the passed exception needs to be logged and logs 
        /// it if necessary.
		/// </summary>
		/// <param name="e">The exception to be logged.</param>
		/// <param name="display">A value that determines when the error message
        /// should be displayed in a messagebox.</param>
		/// <returns>False.</returns>
        /// <remarks>
        /// <para>Errors will be logged in "My Documents\(AppName).err" and will
        /// also be sent to the debug logger with severity = alarm.</para>
		/// <para>The message that gets logged is identical to what you would 
        /// get if you had called:</para>
		/// <para><c>LogError(e.ToString(), display);</c></para>
		/// <para>Note that this method only returns a value to facilitate its 
        /// use in the VB.Net <c>Catch ... When (condition)</c> construct.</para>
        ///</remarks>
		public static bool LogError(Exception e, DisplayOption display) {
			LogError(e.ToString(), display);
			return false;
		}
		
		/// <summary>
		/// Determines whether the passed error text needs to be logged and logs
        /// it if necessary.
		/// </summary>
		/// <param name="errText">The text of the error to be logged.</param>
		/// <param name="display">A value that determines when the error message
        /// should be displayed in a messagebox.</param>
		/// <returns>False.</returns>
        /// <remarks>
        /// <para>Errors will be logged in "My Documents\(AppName).err" and will
        /// also be sent to the debug logger with severity = alarm.</para>
        /// <para>Note that this method only returns a value to facilitate its 
        /// use in the VB.Net <c>Catch ... When (condition)</c> construct.</para>
        /// </remarks>
		public static bool LogError(string errText, DisplayOption display) {
			errText = string.Format(ResStrings.GetString("Thread"),
			                        Thread.CurrentThread.Name,
			                        Thread.CurrentThread.ManagedThreadId.ToString()) + "\r\n" + errText;
			PurgeOldErrors();
			if (IsLogDuplicate(errText)) {
				// Not logging: only display error if user selected.
				if (display == DisplayOption.Always)
					DisplayToScreen(errText);
			} else {
				// Log the error and add to remembered errors collection
				LogToFile(errText);
				DebugLogger.LogAction(ResStrings.GetString("UnhandledError"), errText, LoggingLevels.Alarm);
				LoggedError errNew = new LoggedError();
				errNew.ErrorMessage = errText;
				loggedErrors.Add(errNew);
				// Now display the error if required
				if (display != DisplayOption.Never)
					DisplayToScreen(errText);
			}
			return false;
		}
		
		#endregion

		#region Private Functions

		/// <summary>
		/// Checks whether the specified error message already exists in the
		/// collection of remembered errors.
		/// </summary>
		/// <param name="errText">The error text to be looked for.</param>
		/// <returns>True if the message exists, false if not.</returns>
		private static bool IsLogDuplicate(string errText)
		{
			for (int i=0; i<loggedErrors.Count; i++) {
				if (loggedErrors[i].ErrorMessage == errText)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Removes any remembered errors which are older than the threshold.
		/// </summary>
		private static void PurgeOldErrors() {
			// Since the entries will always be in time order, we can bug out as
			// soon as we see a recent log.
			while ((loggedErrors.Count > 0)) {
				if (loggedErrors[0].Age > ageThreshold) {
					loggedErrors.RemoveAt(0);
				} else {
					return;
				}
			}
		}
		
		/// <summary>
		/// Saves the passed error text to the error file.
		/// </summary>
		private static void LogToFile(String errText)
		{
			using (System.IO.StreamWriter w = System.IO.File.AppendText(fileName)) {
				w.WriteLine("--------------------------------------------------------------------------------");
				w.WriteLine(System.DateTime.Now.ToString("s"));
				w.WriteLine(errText);
				w.WriteLine("");
				w.Flush();
				w.Close();
			}
		}
		
		/// <summary>
		/// Displays the passed error message in a message box on screen.
		/// </summary>
		private static void DisplayToScreen(String errText)
		{
			System.Windows.Forms.MessageBox.Show(
				String.Format(ResStrings.GetString("ErrorHeader"), fileName, errText),
				String.Format(ResStrings.GetString("ErrorCaption"), appName));
		}

		#endregion

	}
}

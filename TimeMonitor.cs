using System;

namespace LogNet
{
	/// <summary>
	/// The TimeMonitor class tracks how much time a given action takes. It can be set to log its
	/// results every few actions.
	/// </summary>
	public class TimeMonitor
	{
		
		#region Variables

		private bool enabled = true;
		private DateTime started = DateTime.MinValue;
		private double totalMilliSecs = 0;
		private int eventCount = 0;
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Get or set a flag indicating if logging is enabled.
		/// </summary>
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		
		/// <summary>
		/// Get the total elapsed miliseconds since our last log.
		/// </summary>
		public double TotalMilliSecs {
			get { return totalMilliSecs; }
		}
		
		/// <summary>
		/// Get the total number of events sonce our last log.
		/// </summary>
		public int EventCount {
			get { return eventCount; }
		}
		
		/// <summary>
		/// Get the average time per event.
		/// </summary>
		public double AverageMilliSecs {
			get {
				if (eventCount == 0)
					return 0.0;
				return totalMilliSecs / eventCount;
			}
		}
			
		#endregion
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TimeMonitor() {
		}
		
		#region Public methods
		
		/// <summary>
		/// Start timing a new event. Note that any old unfinished event will be discarded when this
		/// method gets called.
		/// </summary>
		public void Start() {
			if (!enabled)
				return;
			started = DateTime.Now;
		}
		
		/// <summary>
		/// Finish timing the current event.
		/// </summary>
		public void Finish() {
			if (!enabled || started == DateTime.MinValue)
				return;
			TimeSpan elapsed = DateTime.Now - started;
			totalMilliSecs += elapsed.TotalMilliseconds;
			eventCount++;
			started = DateTime.MinValue;
		}
		
		/// <summary>
		/// Reset all tracking.
		/// </summary>
		public void Clear() {
			totalMilliSecs = 0;
			eventCount = 0;
			started = DateTime.MinValue;
		}
		
		#endregion
		
	}
}

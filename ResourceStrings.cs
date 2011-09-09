using System;
using System.Resources;  //For ResourceManager.xxx
using System.Reflection; //For Assembly.xxx

namespace LogNet
{
	/// <remarks>
	/// This class is used to hold a singleton reference to the <c>Strings.resx</c>
	/// resource file.
	/// </remarks>
	internal sealed class ResStrings
	{
		private static ResourceManager resMan = new ResourceManager("LogNet.Strings", Assembly.GetExecutingAssembly());

		/// <summary>
		/// Default constructor. We never want this class instantiated, so this
		/// is private.
		/// </summary>
		private ResStrings() {
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetString(string name) {
			return resMan.GetString(name);
		}
	}
}

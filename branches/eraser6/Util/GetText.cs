using System;
using System.Collections.Generic;
using System.Text;

using GNU.Gettext;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Eraser.Util
{
	/// <summary>
	/// GetText shortcut class. Instead of calling GetString on all strings, just
	/// call S._(string) or S._(string, object) for plurals
	/// </summary>
	public static class S
	{
		/// <summary>
		/// Translates the localizable string to the set localized string.
		/// </summary>
		/// <param name="str">The string to localize.</param>
		/// <returns>A localized string, or str if no localization exists.</returns>
		public static string _(string str)
		{
			Assembly caller = Assembly.GetCallingAssembly();
			GettextResourceManager res = null;
			if (!managers.ContainsKey(caller))
			{
				res = new GettextResourceManager(
					Path.GetFileNameWithoutExtension(caller.Location), caller);
				managers[caller] = res;
			}
			else
				res = managers[caller];

			return res.GetString(str, Language);
		}

		/// <summary>
		/// The current culture to use when looking up for localizations.
		/// </summary>
		public static CultureInfo Language = CultureInfo.CurrentUICulture;
		private static Dictionary<Assembly, GettextResourceManager> managers =
			new Dictionary<Assembly, GettextResourceManager>();
	}
}

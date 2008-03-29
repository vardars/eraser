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

		private static Dictionary<Assembly, GettextResourceManager> managers =
			new Dictionary<Assembly, GettextResourceManager>();
		public static CultureInfo Language = CultureInfo.CurrentUICulture;
	}
}

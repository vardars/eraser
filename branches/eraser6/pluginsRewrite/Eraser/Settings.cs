/* 
 * $Id$
 * Copyright 2008-2010 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Linq;

using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

using Eraser.Util;
using Eraser.Plugins;

namespace Eraser
{
	internal class Settings : PersistentStore
	{
		/// <summary>
		/// Registry-based storage backing for the Settings class.
		/// </summary>
		private sealed class RegistrySettings : PersistentStore, IDisposable
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="key">The registry key to look for the settings in.</param>
			public RegistrySettings(RegistryKey key)
			{
				if (key == null)
					throw new ArgumentNullException("key");

				Key = key;
			}

			#region IDisposable Members

			~RegistrySettings()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (Key == null)
					return;

				if (disposing)
					Key.Close();
				Key = null;
			}

			#endregion

			public override T GetValue<T>(string name, T defaultValue)
			{
				//Get the raw registry value
				object rawResult = Key.GetValue(name, null);
				if (rawResult == null)
					return defaultValue;

				//Check if it is a serialised object
				byte[] resultArray = rawResult as byte[];
				if (resultArray != null)
				{
					using (MemoryStream stream = new MemoryStream(resultArray))
						try
						{
							BinaryFormatter formatter = new BinaryFormatter();
							if (typeof(T) != typeof(object))
								formatter.Binder = new TypeNameSerializationBinder(typeof(T));
							return (T)formatter.Deserialize(stream);
						}
						catch (InvalidCastException)
						{
							Key.DeleteValue(name);
							MessageBox.Show(S._("Could not load the setting {0}\\{1}. The " +
								"setting has been lost.", Key, name),
								S._("Eraser"), MessageBoxButtons.OK, MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								Localisation.IsRightToLeft(null) ? MessageBoxOptions.RtlReading : 0);
						}
				}
				else if (typeof(T) == typeof(Guid))
				{
					return (T)(object)new Guid((string)rawResult);
				}
				else if (typeof(T).GetInterfaces().Any(x => x == typeof(IConvertible)))
				{
					return (T)Convert.ChangeType(rawResult, typeof(T));
				}
				else
				{
					return (T)rawResult;
				}

				return defaultValue;
			}

			public override void SetValue(string name, object value)
			{
				if (value == null)
				{
					Key.DeleteValue(name);
				}
				else
				{
					if (value is bool)
						Key.SetValue(name, value, RegistryValueKind.DWord);
					else if ((value is int) || (value is uint))
						Key.SetValue(name, value, RegistryValueKind.DWord);
					else if ((value is long) || (value is ulong))
						Key.SetValue(name, value, RegistryValueKind.QWord);
					else if ((value is string) || (value is Guid))
						Key.SetValue(name, value, RegistryValueKind.String);
					else if (value is ICollection<string>)
					{
						ICollection<string> collection = (ICollection<string>)value;
						string[] temp = new string[collection.Count];
						collection.CopyTo(temp, 0);
						Key.SetValue(name, temp, RegistryValueKind.MultiString);
					}
					else
						using (MemoryStream stream = new MemoryStream())
						{
							new BinaryFormatter().Serialize(stream, value);
							Key.SetValue(name, stream.ToArray(), RegistryValueKind.Binary);
						}
				}
			}

			public override PersistentStore GetSubsection(string subsectionName)
			{
				RegistryKey subKey = null;

				try
				{
					//Open the registry key containing the settings
					subKey = Key.OpenSubKey(subsectionName, true);
					if (subKey == null)
						subKey = Key.CreateSubKey(subsectionName);

					return new RegistrySettings(subKey);
				}
				finally
				{
					if (subKey != null)
						subKey.Close();
				}
			}

			/// <summary>
			/// The registry key where the data is stored.
			/// </summary>
			private RegistryKey Key;
		}

		public Settings()
		{
			RegistryKey eraserKey = null;

			try
			{
				//Open the registry key containing the settings
				eraserKey = Registry.CurrentUser.OpenSubKey(Program.SettingsPath, true);
				if (eraserKey == null)
					eraserKey = Registry.CurrentUser.CreateSubKey(Program.SettingsPath);

				//Return the Settings object.
				registry = new RegistrySettings(eraserKey);
			}
			finally
			{
				if (eraserKey != null)
					eraserKey.Close();
			}
		}

		public override PersistentStore GetSubsection(string subsectionName)
		{
			return registry.GetSubsection(subsectionName);
		}

		public override T GetValue<T>(string name, T defaultValue)
		{
			return registry.GetValue<T>(name, defaultValue);
		}

		public override void SetValue(string name, object value)
		{
			registry.SetValue(name, value);
		}

		private RegistrySettings registry;
	}

	/// <summary>
	/// Encapsulates an abstract list that is used to store settings.
	/// </summary>
	/// <typeparam name="T">The type of the list element.</typeparam>
	class SettingsList<T> : IList<T>
	{
		public SettingsList(Settings settings, string settingName)
		{
			Settings = settings;
			SettingName = settingName;
			List = new List<T>();

			T[] values = settings.GetValue<T[]>(settingName);
			if (values != null)
				List.AddRange(values);
		}

		~SettingsList()
		{
			Save();
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			List.Insert(index, item);
			Save();
		}

		public void RemoveAt(int index)
		{
			List.RemoveAt(index);
			Save();
		}

		public T this[int index]
		{
			get
			{
				return List[index];
			}
			set
			{
				List[index] = value;
				Save();
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			List.Add(item);
			Save();
		}

		public void Clear()
		{
			List.Clear();
			Save();
		}

		public bool Contains(T item)
		{
			return List.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return List.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			bool result = List.Remove(item);
			Save();
			return result;
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Saves changes made to the list to the settings manager.
		/// </summary>
		private void Save()
		{
			Settings.SetValue(SettingName, List);
		}

		/// <summary>
		/// The settings object storing the settings.
		/// </summary>
		private Settings Settings;

		/// <summary>
		/// The name of the setting we are encapsulating.
		/// </summary>
		private string SettingName;

		/// <summary>
		/// The list we are using as scratch.
		/// </summary>
		private List<T> List;
	}

	/// <summary>
	/// Encapsulates an abstract dictionary that is used to store settings.
	/// </summary>
	/// <typeparam name="TKey">The key type of the dictionary.</typeparam>
	/// <typeparam name="TValue">The value type of the dictionary.</typeparam>
	class SettingsDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		public SettingsDictionary(Settings settings, string settingName)
		{
			Settings = settings;
			SettingName = settingName;
			Dictionary = settings.GetValue<Dictionary<TKey, TValue>>(settingName);
			if (Dictionary == null)
				Dictionary = new Dictionary<TKey, TValue>();
		}

		~SettingsDictionary()
		{
			Save();
		}

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			Dictionary.Add(key, value);
			Save();
		}

		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { return Dictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			bool result = Dictionary.Remove(key);
			Save();
			return result;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get { return Dictionary.Values; }
		}

		public TValue this[TKey key]
		{
			get
			{
				return Dictionary[key];
			}
			set
			{
				Dictionary[key] = value;
				Save();
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Dictionary.Add(item.Key, item.Value);
			Save();
		}

		public void Clear()
		{
			Dictionary.Clear();
			Save();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Dictionary.ContainsKey(item.Key) && Dictionary[item.Key].Equals(item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return Dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (Dictionary.ContainsKey(item.Key) && Dictionary[item.Key].Equals(item.Value))
			{
				bool result = Dictionary.Remove(item.Key);
				Save();
				return result;
			}

			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Dictionary.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Saves changes made to the list to the settings manager.
		/// </summary>
		private void Save()
		{
			Settings.SetValue(SettingName, Dictionary);
		}

		/// <summary>
		/// The settings object storing the settings.
		/// </summary>
		private Settings Settings;

		/// <summary>
		/// The name of the setting we are encapsulating.
		/// </summary>
		private string SettingName;

		/// <summary>
		/// The list we are using as scratch.
		/// </summary>
		private Dictionary<TKey, TValue> Dictionary;
	}

	internal class EraserSettings
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private EraserSettings()
		{
			settings = Host.Instance.PersistentStore.GetValue<PersistentStore>(
				new Guid(((GuidAttribute)Assembly.GetCallingAssembly().
					GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value).ToString());
		}

		/// <summary>
		/// Gets the singleton instance of the Eraser UI Settings.
		/// </summary>
		/// <returns>The global instance of the Eraser UI settings.</returns>
		public static EraserSettings Get()
		{
			if (instance == null)
				instance = new EraserSettings();
			return instance;
		}

		/// <summary>
		/// Gets or sets the LCID of the language which the UI should be displayed in.
		/// </summary>
		public string Language
		{
			get
			{
				return settings.GetValue("Language", GetCurrentCulture().Name);
			}
			set
			{
				settings.SetValue("Language", value);
			}
		}

		/// <summary>
		/// Gets or sets whether the Shell Extension should be loaded into Explorer.
		/// </summary>
		public bool IntegrateWithShell
		{
			get
			{
				return settings.GetValue("IntegrateWithShell", true);
			}
			set
			{
				settings.SetValue("IntegrateWithShell", value);
			}
		}

		/// <summary>
		/// Gets or sets a value on whether the main frame should be minimised to the
		/// system notification area.
		/// </summary>
		public bool HideWhenMinimised
		{
			get
			{
				return settings.GetValue("HideWhenMinimised", true);
			}
			set
			{
				settings.SetValue("HideWhenMinimised", value);
			}
		}

		/// <summary>
		/// Gets ot setts a value whether tasks which were completed successfully
		/// should be removed by the Eraser client.
		/// </summary>
		public bool ClearCompletedTasks
		{
			get
			{
				return settings.GetValue("ClearCompletedTasks", true);
			}
			set
			{
				settings.SetValue("ClearCompletedTasks", value);
			}
		}

		/// <summary>
		/// Gets the most specific UI culture with a localisation available, defaulting to English
		/// if none exist.
		/// </summary>
		/// <returns>The CultureInfo of the current UI culture, correct to the top level.</returns>
		private static CultureInfo GetCurrentCulture()
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			CultureInfo culture = CultureInfo.CurrentUICulture;
			while (culture.Parent != CultureInfo.InvariantCulture &&
				!Localisation.LocalisationExists(culture, entryAssembly))
			{
				culture = culture.Parent;
			}

			//Default to English if any of our cultures don't exist.
			if (!Localisation.LocalisationExists(culture, entryAssembly))
				culture = new CultureInfo("en");

			return culture;
		}

		/// <summary>
		/// The data store behind the object.
		/// </summary>
		private PersistentStore settings;

		/// <summary>
		/// The global instance of the settings class.
		/// </summary>
		private static EraserSettings instance;
	}
}

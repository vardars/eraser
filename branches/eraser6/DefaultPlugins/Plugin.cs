/* 
 * $Id$
 * Copyright 2008 The Eraser Project
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
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Manager.Plugin;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	public class DefaultPlugin : IPlugin
	{
		public void Initialize(Host host)
		{
			//Get the settings dictionary
			Settings = Manager.ManagerLibrary.Instance.Settings.PluginSettings;

			//Then register the erasure methods et al.
			ErasureMethodManager.Register(new Gutmann());				//35 passes
			ErasureMethodManager.Register(new GutmannLite());			//10 passes
			ErasureMethodManager.Register(new DoD_EcE());				//7 passes
			ErasureMethodManager.Register(new RCMP_TSSIT_OPS_II());	//7 passes
			ErasureMethodManager.Register(new Schneier());				//7 passes
			ErasureMethodManager.Register(new VSITR());				//7 passes
			ErasureMethodManager.Register(new DoD_E());				//3 passes
			ErasureMethodManager.Register(new HMGIS5Enhanced());		//3 passes
			ErasureMethodManager.Register(new USAF5020());				//3 passes
			ErasureMethodManager.Register(new USArmyAR380_19());		//3 passes
			ErasureMethodManager.Register(new GOSTP50739());			//2 passes
			ErasureMethodManager.Register(new HMGIS5Baseline());		//1 pass
			ErasureMethodManager.Register(new Pseudorandom());			//1 pass
			EraseCustom.RegisterAll();

			PRNGManager.Register(new ISAAC());
			PRNGManager.Register(new RNGCrypto());

			//Done last
			try
			{
				ErasureMethodManager.Register(new FirstLast16KB());
			}
			catch (Exception)
			{
			}
		}

		public void Dispose()
		{
			Manager.ManagerLibrary.Instance.Settings.SetSettings(Settings);
		}

		public string Name
		{
			get { return S._("Default Erasure Methods and PRNGs"); }
		}

		public string Author
		{
			get { return S._("The Eraser Project <eraser-development@lists.sourceforge.net>"); }
		}

		public bool Configurable
		{
			get { return true; }
		}

		public void DisplaySettings(Control parent)
		{
			SettingsForm form = new SettingsForm();
			form.ShowDialog();
		}

		/// <summary>
		/// The dictionary holding settings for this plugin.
		/// </summary>
		public static Dictionary<string, object> Settings = null;
	}
}

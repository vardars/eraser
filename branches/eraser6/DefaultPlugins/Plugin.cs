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
			Settings = Manager.ManagerLibrary.Instance.Settings.GetSettings();

			//Then register the erasure methods et al.
			host.RegisterErasureMethod(new Gutmann());
			host.RegisterErasureMethod(new Schneier());
			host.RegisterErasureMethod(new DoD_EcE());
			host.RegisterErasureMethod(new DoD_E());
			host.RegisterErasureMethod(new Pseudorandom());

			host.RegisterErasureMethod(new HMGIS5Baseline());
			host.RegisterErasureMethod(new GOSTP50739());
			host.RegisterErasureMethod(new USAF5020());
			host.RegisterErasureMethod(new HMGIS5Enhanced());
			host.RegisterErasureMethod(new USArmyAR380_19());
			host.RegisterErasureMethod(new VSITR());
			host.RegisterErasureMethod(new RCMP_TSSIT_OPS_II());
			host.RegisterErasureMethod(new GutmannLite());
			host.RegisterPRNG(new ISAAC());
			host.RegisterPRNG(new RNGCrypto());

			//Done last
			try
			{
				host.RegisterErasureMethod(new FirstLast16KB());
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

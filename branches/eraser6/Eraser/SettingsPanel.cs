using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eraser
{
	public partial class SettingsPanel : Eraser.BasePanel
	{
		public SettingsPanel()
		{
			InitializeComponent();
			Dock = DockStyle.None;
		}
	}
}


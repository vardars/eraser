using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eraser.Util;

namespace Eraser
{
	public partial class BlackBoxUploadForm : Form
	{
		public BlackBoxUploadForm()
		{
			InitializeComponent();
			UXThemeApi.UpdateControlTheme(this);
		}
	}
}

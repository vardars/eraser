using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Eraser.Manager;

namespace Eraser
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (eraserClient = new DirectExecutor())
				Application.Run(new MainForm());
		}

		public static DirectExecutor eraserClient;
	}
}

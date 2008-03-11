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
			Application.Run(new MainForm());

			(eraserClient as IDisposable).Dispose();
		}

		public static Executor eraserClient = new DirectExecutor();
	}
}

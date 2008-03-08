using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	public class EraseMethod
	{
		public string Name
		{
			get { return "Unknown"; }
		}
		public readonly EraseMethod Default = new EraseMethod();
	}
}

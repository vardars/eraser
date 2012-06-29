using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Eraser.Manager;

namespace Eraser.Service
{
	class Program
	{
		static void Main(string[] args)
		{
			RemoteExecutorServer eraserClient = new RemoteExecutorServer();

			//Run the eraser client.
			eraserClient.Run();

			Console.ReadKey();
		}
	}
}

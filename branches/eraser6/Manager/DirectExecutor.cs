using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace Eraser.Manager
{
	/// <summary>
	/// The DirectExecutor class is used by the Eraser GUI directly when the program
	/// is run without the help of a Service.
	/// </summary>
	class DirectExecutor : Executor
	{
		public DirectExecutor()
		{
			thread = new Thread(delegate()
			{
				this.Main();
			});
		}

		public override void AddTask(ref Task task)
		{
			lock (unusedIdsLock)
			{
				if (unusedIds.Count != 0)
				{
					task.ID = unusedIds[0];
					unusedIds.RemoveAt(0);
				}
				else
					task.ID = ++nextId;
			}
			tasks.Add(task.ID, task);
		}

		public override bool DeleteTask(uint taskId)
		{
			if (!tasks.ContainsKey(taskId))
				return false;

			lock (unusedIdsLock)
				unusedIds.Add(taskId);
			tasks.Remove(taskId);
			return true;
		}

		public override Task GetTask(uint taskId)
		{
			if (!tasks.ContainsKey(taskId))
				return null;
			return tasks[taskId];
		}

		public override Dictionary<uint, Task>.Enumerator GetIterator()
		{
			return tasks.GetEnumerator();
		}

		private void Main()
		{
		}

		private Thread thread;

		private Dictionary<uint, Task> tasks = new Dictionary<uint, Task>();
		private SortedList<DateTime, Task> scheduledTasks =
			new SortedList<DateTime, Task>();

		private List<uint> unusedIds = new List<uint>();
		private object unusedIdsLock = new object();
		private uint nextId = 0;
	}
}

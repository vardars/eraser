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
	public class DirectExecutor : Executor
	{
		public DirectExecutor()
		{
			thread = new Thread(delegate()
			{
				this.Main();
			});

			thread.Start();
			Thread.Sleep(0);
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

			//Add the task to the set of tasks
			lock (tasksLock)
			{
				tasks.Add(task.ID, task);

				//If the task is scheduled to run now, break the waiting thread and
				//run it immediately
				if (task.Schedule == Schedule.RunNow)
				{
					scheduledTasks.Add(DateTime.Now, task);
					schedulerInterrupt.Set();
				}
				//If the task is scheduled, add the next execution time to the list
				//of schduled tasks.
				else if (task.Schedule != Schedule.RunOnRestart)
				{
					scheduledTasks.Add((task.Schedule as RecurringSchedule).NextRun, task);
				}
			}
		}

		public override bool DeleteTask(uint taskId)
		{
			lock (tasksLock)
			{
				if (!tasks.ContainsKey(taskId))
					return false;

				lock (unusedIdsLock)
					unusedIds.Add(taskId);
				tasks.Remove(taskId);
			}
			return true;
		}

		public override Task GetTask(uint taskId)
		{
			lock (tasksLock)
			{
				if (!tasks.ContainsKey(taskId))
					return null;
				return tasks[taskId];
			}
		}

		public override Dictionary<uint, Task>.Enumerator GetIterator()
		{
			return tasks.GetEnumerator();
		}

		private void Main()
		{
			//The waiting thread will utilize a polling loop to check for new
			//scheduled tasks. This will be checked every 30 seconds. However,
			//when the thread is waiting for a new task, it can be interrupted.
			while (thread.ThreadState != ThreadState.AbortRequested)
			{
				//Check for a new task
				Task task = null;
				lock (tasksLock)
				{
					if (scheduledTasks.Count != 0 &&
						(scheduledTasks.Values[0].Schedule == Schedule.RunNow ||
						 scheduledTasks.Keys[0] <= DateTime.Now))
					{
						task = scheduledTasks.Values[0];
						scheduledTasks.RemoveAt(0);
					}
				}

				if (task != null)
				{
					//Run the task
					;
				}

				//Wait for half a minute to check for the next scheduled task.
				schedulerInterrupt.WaitOne(30000, false);
			}
		}

		private Thread thread;

		private object tasksLock = new object();
		private Dictionary<uint, Task> tasks = new Dictionary<uint, Task>();
		private SortedList<DateTime, Task> scheduledTasks =
			new SortedList<DateTime, Task>();

		private List<uint> unusedIds = new List<uint>();
		private object unusedIdsLock = new object();
		private uint nextId = 0;

		AutoResetEvent schedulerInterrupt = new AutoResetEvent(true);
	}
}

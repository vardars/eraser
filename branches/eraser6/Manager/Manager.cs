using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	public abstract class Executor
	{
		/// <summary>
		/// Adds a new task to be executed in the future.
		/// </summary>
		/// <param name="task">The Task object describing the details of the task.
		/// The task object's ID member will be updated to allow unique identification.</param>
		public virtual void AddTask(ref Task task)
		{
			if (unusedIds.Count != 0)
			{
				task.ID = unusedIds[0];
				unusedIds.RemoveAt(0);
			}
			else
				task.ID = ++nextId;
			tasks.Add(task.ID, task);
		}

		/// <summary>
		/// Deletes a task currently pending execution.
		/// </summary>
		/// <param name="taskId">The unique task ID returned when AddTask was called.</param>
		/// <returns>True if the task was found and deleted.</returns>
		public virtual bool DeleteTask(uint taskId)
		{
			if (!tasks.ContainsKey(taskId))
				return false;
			unusedIds.Add(taskId);
			tasks.Remove(taskId);
			return true;
		}

		/// <summary>
		/// Retrieves the task object represented by the task ID given.
		/// </summary>
		/// <param name="taskId">The Task ID of the task in question.</param>
		/// <returns>The task object represented by the ID, or null if no object is found</returns>
		public virtual Task GetTask(uint taskId)
		{
			if (!tasks.ContainsKey(taskId))
				return null;
			return tasks[taskId];
		}

		/// <summary>
		/// Retrieves an enumerator to the list of tasks.
		/// </summary>
		/// <returns>An enumerator to the list of tasks</returns>
		public Dictionary<uint, Task>.Enumerator GetIterator()
		{
			return tasks.GetEnumerator();
		}

		private Dictionary<uint, Task> tasks = new Dictionary<uint, Task>();
		private List<uint> unusedIds = new List<uint>();
		private uint nextId = 0;
	}
}

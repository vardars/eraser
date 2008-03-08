using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// Executor base class. This class will manage the tasks currently scheduled
	/// to be run and will run them when they are set to be run. This class is
	/// abstract as they each will have their own ways of dealing with tasks.
	/// </summary>
	public abstract class Executor
	{
		/// <summary>
		/// Adds a new task to be executed in the future.
		/// </summary>
		/// <param name="task">The Task object describing the details of the task.
		/// The task object's ID member will be updated to allow unique identification.</param>
		public abstract void AddTask(ref Task task);

		/// <summary>
		/// Deletes a task currently pending execution.
		/// </summary>
		/// <param name="taskId">The unique task ID returned when AddTask was called.</param>
		/// <returns>True if the task was found and deleted.</returns>
		public abstract bool DeleteTask(uint taskId);

		/// <summary>
		/// Retrieves the task object represented by the task ID given.
		/// </summary>
		/// <param name="taskId">The Task ID of the task in question.</param>
		/// <returns>The task object represented by the ID, or null if no object is found</returns>
		public abstract Task GetTask(uint taskId);

		/// <summary>
		/// Retrieves an enumerator to the list of tasks.
		/// </summary>
		/// <returns>An enumerator to the list of tasks</returns>
		public abstract Dictionary<uint, Task>.Enumerator GetIterator();
	}
}

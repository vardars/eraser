using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	public interface Client
	{
		/// <summary>
		/// Adds a new task to be executed in the future.
		/// </summary>
		/// <param name="task">The Task object describing the details of the task.
		/// The task object's ID member will be updated to allow unique identification.</param>
		/// <exception cref=""
		void AddTask(ref Task task);

		/// <summary>
		/// Deletes a task currently pending execution.
		/// </summary>
		/// <param name="taskId">The unique task ID returned when AddTask was called.</param>
		/// <returns>True if the task was found and deleted.</returns>
		bool DeleteTask(uint taskId);

		/// <summary>
		/// Retrieves the task object represented by the task ID given.
		/// </summary>
		/// <param name="taskId">The Task ID of the task in question.</param>
		/// <returns>The task object represented by the ID, or null if no object is found</returns>
		Task GetTask(uint taskId);
	}
}

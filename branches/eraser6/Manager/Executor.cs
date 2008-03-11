using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
		/// Replaces the current task in the executor with the new task, loading
		/// new parameters. This maintains the taks ID.
		/// </summary>
		/// <param name="task">The new task details.</param>
		public abstract void ReplaceTask(Task task);
		
		/// <summary>
		/// Queues the task for execution.
		/// </summary>
		/// <param name="task">The task to queue.</param>
		public abstract void QueueTask(Task task);

		/// <summary>
		/// Cancels the given task, if it is being executed or queued for execution.
		/// </summary>
		/// <param name="task">The task to cancel.</param>
		public abstract void CancelTask(Task task);

		/// <summary>
		/// Retrieves the task object represented by the task ID given.
		/// </summary>
		/// <param name="taskId">The Task ID of the task in question.</param>
		/// <returns>The task object represented by the ID, or null if no object
		/// is found.</returns>
		public abstract Task GetTask(uint taskId);

		/// <summary>
		/// Retrieves the current task list for the executor.
		/// </summary>
		/// <returns>A list of tasks which the executor has registered.</returns>
		public abstract List<Task> GetTasks();

		/// <summary>
		/// Saves the task list to the given stream.
		/// </summary>
		/// <param name="stream">The stream to save to.</param>
		public abstract void SaveTaskList(Stream stream);

		/// <summary>
		/// Loads the task list from the given stream.
		/// </summary>
		/// <param name="stream">The stream to save to.</param>
		public abstract void LoadTaskList(Stream stream);

		/// <summary>
		/// The number of times file names are renamed to erase the file name from
		/// the MFT.
		/// </summary>
		protected const int FilenameErasePasses = 7;
	}
}

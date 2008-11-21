/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

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
	public abstract class Executor : IDisposable
	{
		#region IDisposable members
		public abstract void Dispose();
		#endregion

		/// <summary>
		/// Starts the execution of tasks queued.
		/// </summary>
		public abstract void Run();

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
		/// Schedules the given task for execution.
		/// </summary>
		/// <param name="task">The task to schedule</param>
		public abstract void ScheduleTask(Task task);

		/// <summary>
		/// Queues all tasks in the task list which are meant for restart execution.
		/// This is a separate function rather than just running them by default on
		/// task load because creating a new instance and loading the task list
		/// may just be a program restart and may not necessarily be a system
		/// restart. Therefore this fuction has to be explicitly called by clients.
		/// </summary>
		public abstract void QueueRestartTasks();

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
		/// The delegate for handling the task processing event from the executor.
		/// </summary>
		/// <param name="task">The currently processing task.</param>
		public delegate void TaskProcessingEvent(Task task);

		/// <summary>
		/// The task processing event object.
		/// </summary>
		public event TaskProcessingEvent TaskProcessing;

		/// <summary>
		/// Helper function for the Task processing event.
		/// </summary>
		protected void OnTaskProcessing(Task task)
		{
			if (TaskProcessing != null)
				TaskProcessing(task);
		}

		/// <summary>
		/// The delegate for handling the task processed event from the executor.
		/// </summary>
		/// <param name="task">The processed task.</param>
		public delegate void TaskProcessedEvent(Task task);

		/// <summary>
		/// The task processed event object.
		/// </summary>
		public event TaskProcessedEvent TaskProcessed;

		/// <summary>
		/// Helper function for the Task processed event.
		/// </summary>
		protected void OnTaskProcessed(Task task)
		{
			if (TaskProcessed != null)
				TaskProcessed(task);
		}

		/// <summary>
		/// The number of times file names are renamed to erase the file name from
		/// the MFT.
		/// </summary>
		protected const int FilenameErasePasses = 7;
	}
}

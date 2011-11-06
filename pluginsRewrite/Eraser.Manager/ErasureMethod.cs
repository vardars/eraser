/* 
 * $Id$
 * Copyright 2008-2010 The Eraser Project
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

using System.Reflection;
using System.IO;

using Eraser.Util;
using Eraser.Plugins;
using Eraser.Plugins.ExtensionPoints;

namespace Eraser.Manager
{
	/// <summary>
	/// Class managing all the erasure methods. This class pairs GUIDs with constructor
	/// prototypes, and when an instance of the erasure method is required, a new
	/// instance is created. This is unique to erasure methods since the other managers
	/// do not have run-time equivalents; they all are compile-time.
	/// </summary>
	public class ErasureMethodRegistrar : Registrar<ErasureMethod>
	{
		#region Default Erasure method
		private class DefaultMethod : ErasureMethod
		{
			public DefaultMethod()
			{
			}

			public override string Name
			{
				get { return S._("(default)"); }
			}

			public override int Passes
			{
				get { return 0; }
			}

			public override Guid Guid
			{
				get { return Guid.Empty; }
			}

			public override long CalculateEraseDataSize(ICollection<StreamInfo> paths, long targetSize)
			{
				throw new InvalidOperationException("The DefaultMethod class should never " +
					"be used and should instead be replaced before execution!");
			}

			public override void Erase(Stream strm, long erasureLength, Prng prng,
				ErasureMethod.ErasureMethodProgressFunction callback)
			{
				throw new InvalidOperationException("The DefaultMethod class should never " +
					"be used and should instead be replaced before execution!");
			}
		}

		/// <summary>
		/// A dummy method placeholder used for representing the default erase
		/// method. Do not use this variable when trying to call the erase function,
		/// this is just a placeholder and will throw a InvalidOperationException.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly ErasureMethod Default = new DefaultMethod();
		#endregion
	}

	public class ErasureMethodRegistrationEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The GUID of the newly registered/unregistered
		/// erasure method.</param>
		public ErasureMethodRegistrationEventArgs(Guid value)
		{
			Guid = value;
		}

		/// <summary>
		/// The GUID of the newly registsered/unregistered erasure method.
		/// </summary>
		public Guid Guid { get; private set; }
	}
}

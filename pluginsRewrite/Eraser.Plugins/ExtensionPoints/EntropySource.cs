/* 
 * $Id: EntropySource.cs 2055 2010-05-04 05:51:04Z lowjoel $
 * Copyright 2008-2010 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: Kasra Nassiri <cjax@users.sourceforge.net>
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
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Plugins.ExtensionPoints
{
	/// <summary>
	/// Provides an abstract interface to allow multiple sources of entropy into
	/// the EntropyPoller class.
	/// </summary>
	public abstract class EntropySource : IRegisterable
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected EntropySource()
		{
		}

		/// <summary>
		/// The name of the entropy source
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// The guid representing this entropy source
		/// </summary>
		public abstract Guid Guid
		{
			get;
		}

		/// <summary>
		/// Gets a primer to add to the pool when this source is first initialised, to
		/// further add entropy to the pool.
		/// </summary>
		/// <returns>A byte array containing the entropy.</returns>
		public abstract byte[] GetPrimer();

		/// <summary>
		/// Retrieve entropy from a source which will have slow rate of
		/// entropy polling.
		/// </summary>
		/// <returns></returns>
		public abstract byte[] GetSlowEntropy();

		/// <summary>
		/// Retrieve entropy from a soruce which will have a fast rate of 
		/// entropy polling.
		/// </summary>
		/// <returns></returns>
		public abstract byte[] GetFastEntropy();

		/// <summary>
		/// Gets entropy from the entropy source. This will be called repetitively.
		/// </summary>
		/// <returns>A byte array containing the entropy, both slow rate and fast rate.</returns>
		public abstract byte[] GetEntropy();

		/// <summary>
		/// Converts value types into a byte array. This is a helper function to allow
		/// inherited classes to convert value types into byte arrays which can be
		/// returned to the EntropyPoller class.
		/// </summary>
		/// <typeparam name="T">Any value type</typeparam>
		/// <param name="entropy">A value which will be XORed with pool contents.</param>
		protected static byte[] StructToBuffer<T>(T entropy) where T : struct
		{
			int sizeofObject = Marshal.SizeOf(entropy);
			IntPtr memory = Marshal.AllocHGlobal(sizeofObject);
			try
			{
				Marshal.StructureToPtr(entropy, memory, false);
				byte[] dest = new byte[sizeofObject];

				//Copy the memory
				Marshal.Copy(memory, dest, 0, sizeofObject);
				return dest;
			}
			finally
			{
				Marshal.FreeHGlobal(memory);
			}
		}
	}
}

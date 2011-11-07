﻿/* 
 * $Id$
 * Copyright 2008-2011 The Eraser Project
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
using System.Linq;
using System.Text;

using Eraser.Plugins.ExtensionPoints;

namespace Eraser.Plugins.Registrars
{
	/// <summary>
	/// Class managing all the PRNG algorithms.
	/// </summary>
	public class PrngRegistrar : Registrar<Prng>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PrngRegistrar()
		{
		}

		/// <summary>
		/// Gets the PRNG that is active.
		/// </summary>
		/// <remarks>Client code should set the <see cref="ActivePrngGuid"/>
		/// member.</remarks>
		public Prng ActivePrng
		{
			get
			{
				return this[ActivePrngGuid];
			}
		}

		/// <summary>
		/// Sets the GUID of the active PRNG.
		/// </summary>
		private Guid ActivePrngGuid
		{
			get;
			internal set;
		}

		/// <summary>
		/// Allows the EntropyThread to get entropy to the PRNG functions as seeds.
		/// </summary>
		/// <param name="entropy">An array of bytes, being entropy for the PRNG.</param>
		internal void AddEntropy(byte[] entropy)
		{
			lock (Host.Instance.Prngs)
				foreach (Prng prng in Host.Instance.Prngs)
					prng.Reseed(entropy);
		}
	}
}

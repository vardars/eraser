/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * The algorithm in this file is implemented using the description in EMIShredder
 * (http://www.codeplex.com/EMISecurityShredder)
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
using Eraser.Manager;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	sealed class USArmyAR380_19 : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return S._("US Army AR380-19"); }
		}

		public override Guid Guid
		{
			get { return new Guid("{0FE620EA-8055-4861-B5BB-BD8BDC3FD4AC}"); }
		}

		protected override bool RandomizePasses
		{
			get { return false; }
		}

		protected override ErasureMethodPass[] PassesSet
		{
			get
			{
				Prng prng = PrngManager.GetInstance(ManagerLibrary.Settings.ActivePrng);
				int rand = prng.Next();

				return new ErasureMethodPass[]
				{
					new ErasureMethodPass(WriteRandom, null),
					new ErasureMethodPass(WriteConstant, new byte[] { (byte)(rand & 0xFF) }),
					new ErasureMethodPass(WriteConstant, new byte[] { (byte)~(rand & 0xFF) })
				};
			}
		}
	}
}

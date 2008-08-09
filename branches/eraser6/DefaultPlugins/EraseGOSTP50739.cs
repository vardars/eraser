/* 
 * $Id: EraseDoD.cs 348 2008-04-02 13:05:06Z lowjoel $
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
	class GOSTP50739 : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return S._("Russian GOST P50739-95"); }
		}

		public override Guid GUID
		{
			get { return new Guid("{92681583-F484-415f-A66C-CC210222EDC5}"); }
		}

		protected override bool RandomizePasses
		{
			get { return false; }
		}

		protected override Pass[] PassesSet
		{
			get
			{
				return new Pass[]
				{
					new Pass(WriteConstant, new byte[] { (byte)0 }),
					new Pass(WriteRandom, null)
				};
			}
		}
	}
}

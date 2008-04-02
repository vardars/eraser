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
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY
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
	class Gutmann : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return S._("Gutmann"); }
		}

		public override Guid GUID
		{
			get { return new Guid("{1407FC4E-FEFF-4375-B4FB-D7EFBB7E9922}"); }
		}

		protected override bool RandomizePasses
		{
			get { return true; }
		}

		protected override Pass[] PassesSet
		{
			get
			{
				return new Pass[]
				{
					new Pass(WriteRandom, null),                                   // 1
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteConstant, new byte[] {0x55}),                    // 5
					new Pass(WriteConstant, new byte[] {0xAA}),
					new Pass(WriteConstant, new byte[] {0x92, 0x49, 0x24}),
					new Pass(WriteConstant, new byte[] {0x49, 0x24, 0x92}),
					new Pass(WriteConstant, new byte[] {0x24, 0x92, 0x49}),
					new Pass(WriteConstant, new byte[] {0x00}),                    // 10
					new Pass(WriteConstant, new byte[] {0x11}),
					new Pass(WriteConstant, new byte[] {0x22}),
					new Pass(WriteConstant, new byte[] {0x33}),
					new Pass(WriteConstant, new byte[] {0x44}),
					new Pass(WriteConstant, new byte[] {0x55}),                    // 15
					new Pass(WriteConstant, new byte[] {0x66}),
					new Pass(WriteConstant, new byte[] {0x77}),
					new Pass(WriteConstant, new byte[] {0x88}),
					new Pass(WriteConstant, new byte[] {0x99}),
					new Pass(WriteConstant, new byte[] {0xAA}),                    // 20
					new Pass(WriteConstant, new byte[] {0xBB}),
					new Pass(WriteConstant, new byte[] {0xCC}),
					new Pass(WriteConstant, new byte[] {0xDD}),
					new Pass(WriteConstant, new byte[] {0xEE}),
					new Pass(WriteConstant, new byte[] {0xFF}),                    // 25
					new Pass(WriteConstant, new byte[] {0x92, 0x49, 0x24}),
					new Pass(WriteConstant, new byte[] {0x49, 0x24, 0x92}),
					new Pass(WriteConstant, new byte[] {0x24, 0x92, 0x49}),
					new Pass(WriteConstant, new byte[] {0x6D, 0xB6, 0xDB}),
					new Pass(WriteConstant, new byte[] {0xB6, 0xDB, 0x6D}),        // 30
					new Pass(WriteConstant, new byte[] {0xDB, 0x6D, 0xB6}),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null)                                    // 35
				};
			}
		}
	}
}

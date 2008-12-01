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
using Eraser.Manager;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	class DoD_EcE : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return S._("US DoD 5220.22-M (8-306./E, C & E)"); }
		}

		public override Guid GUID
		{
			get { return new Guid("{D1583631-702E-4dbf-A0E9-C35DBA481702}"); }
		}

		protected override bool RandomizePasses
		{
			get { return false; }
		}

		protected override Pass[] PassesSet
		{
			get
			{
				//Set passes 1, 4 and 5 to be a random value
				PRNG prng = PRNGManager.GetInstance(ManagerLibrary.Instance.Settings.ActivePRNG);
				int rand = prng.Next();

				Pass[] result = new Pass[]
				{
					new Pass(WriteConstant, new byte[] { (byte)(rand & 0xFF) }),
					new Pass(WriteConstant, new byte[] { 0 }),
					new Pass(WriteRandom, null),
					new Pass(WriteConstant, new byte[] { (byte)((rand >> 8) & 0xFF) }),
					new Pass(WriteConstant, new byte[] { (byte)((rand >> 16) & 0xFF) }),
					new Pass(WriteConstant, new byte[] { 0 }),
					new Pass(WriteRandom, null)
				};

				//Set passes 2 and 6 to be complements of 1 and 5
				result[1] = new Pass(WriteConstant, new byte[] {
					(byte)(~((byte[])result[0].OpaqueValue)[0]) });
				result[5] = new Pass(WriteConstant, new byte[] {
					(byte)(~((byte[])result[4].OpaqueValue)[0]) });
				return result;
			}
		}
	}

	class DoD_E : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return S._("US DoD 5220.22-M (8-306./E)"); }
		}

		public override Guid GUID
		{
			get { return new Guid("{ECBF4998-0B4F-445c-9A06-23627659E419}"); }
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
					new Pass(WriteConstant, new byte[] { 0 }),
					new Pass(WriteConstant, new byte[] { 0xFF }),
					new Pass(WriteRandom, null)
				};
			}
		}
	}
}

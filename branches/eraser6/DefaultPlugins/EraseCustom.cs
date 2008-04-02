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

namespace Eraser.DefaultPlugins
{
	class EraseCustom : PassBasedErasureMethod
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="method">The erasure method definition for the custom method.</param>
		public EraseCustom(CustomErasureMethod method)
		{
			this.method = method;
		}

		public override string Name
		{
			get { return method.Name; }
		}

		public override Guid GUID
		{
			get { return method.GUID; }
		}

		protected override bool RandomizePasses
		{
			get { return method.RandomizePasses; }
		}

		protected override Pass[] PassesSet
		{
			get { return method.Passes; }
		}

		CustomErasureMethod method;
	}

	/// <summary>
	/// Contains information necessary to create user-defined erasure methods.
	/// </summary>
	public class CustomErasureMethod
	{
		public string Name;
		public Guid GUID;
		public bool RandomizePasses;
		public ErasureMethod.Pass[] Passes;
	}
}

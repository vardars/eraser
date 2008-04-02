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

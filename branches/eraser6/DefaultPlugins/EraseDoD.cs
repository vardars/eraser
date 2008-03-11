using System;
using System.Collections.Generic;
using System.Text;
using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class DoD_EcE : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return "US DoD 5220.22-M (8-306. / E, C and E)"; }
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
				PRNG prng = PRNGManager.GetInstance(Globals.Settings.ActivePRNG);
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
			get { return "US DoD 5220.22-M (8-306. / E)"; }
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

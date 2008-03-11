using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return "Gutmann"; }
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

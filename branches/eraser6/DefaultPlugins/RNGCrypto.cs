using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using System.Security.Cryptography;

namespace Eraser.DefaultPlugins
{
	public class RNGCrypto : PRNG
	{
		public override string Name
		{
			get { return "RNGCryptoServiceProvider"; }
		}

		public override void NextBytes(byte[] buffer)
		{
			lock (rand)
				rand.GetBytes(buffer);
		}

		RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
	}
}

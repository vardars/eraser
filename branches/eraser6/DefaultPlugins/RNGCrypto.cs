using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using System.Security.Cryptography;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	public class RNGCrypto : PRNG
	{
		public override string Name
		{
			get { return S._("RNGCryptoServiceProvider"); }
		}

		public override Guid GUID
		{
			get { return new Guid("{6BF35B8E-F37F-476e-B6B2-9994A92C3B0C}"); }
		}

		public override void NextBytes(byte[] buffer)
		{
			lock (rand)
				rand.GetBytes(buffer);
		}

		protected override void Reseed(byte[] seed)
		{
			//No-op. RNGCryptoServiceProviders can't be reseeded.
		}

		RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
	}
}

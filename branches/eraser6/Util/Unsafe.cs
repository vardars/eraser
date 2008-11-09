using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Util
{
	public class Unsafe
	{		/// <summary>
		/// Optomised unmanaged circular memory xor
		/// </summary>
		/// <param name="dest">Destination Pointer</param>
		/// <param name="source">Source Pointer</param>
		/// <param name="size">Size in bytes</param>
		public unsafe static int CircularMemoryXor(IntPtr destination, IntPtr source,
			int destOffset, int destLength, int size)
		{
			uint* dest = (uint*)destination.ToPointer();
			uint* src = (uint*)source.ToPointer();
			while (size > 0)
			{
				if (size + destOffset < destLength)
				{
					IntPtr _gc = new IntPtr(destination.ToInt32() + destOffset);
					MemoryXor(_gc, source, size);
					destOffset += size;
					size = 0;
				}
				else // (size + destOffset >= destLength)
				{
					IntPtr _gc = new IntPtr(destination.ToInt32() + destOffset);
					MemoryXor(_gc, source, destLength - destOffset);
					source = new IntPtr(source.ToInt32() + destLength - destOffset);
					size -= destLength - destOffset;
					destOffset = 0;
				}
			}

			return destOffset;
		}

		public static unsafe void MemoryXor(IntPtr destination, IntPtr source, int size)
		{
			int wsize = size / sizeof(int); size -= wsize * sizeof(int);
			uint* d = (uint*)destination.ToPointer();
			uint* s = (uint*)source.ToPointer();

			while (wsize-- > 0) *d++ ^= *s++;

			if (size > 0)
			{
				byte* db = (byte*)d, ds = (byte*)s;
				while (size-- > 0) *db++ ^= *ds++;
			}
		}
	}
}

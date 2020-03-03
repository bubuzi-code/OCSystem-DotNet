using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace OnlyChain.Secp256k1.Math {
	internal struct Point {
		public static readonly Point Zero = new Point() { IsZero = true };

		public Fraction X, Y;
		public bool IsZero;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Point(in Fraction x, in Fraction y) {
			X = x;
			Y = y;
			IsZero = false;
		}
	}
}

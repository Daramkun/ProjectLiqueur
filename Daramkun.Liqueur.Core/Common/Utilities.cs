using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// Liqueur utilities
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// subtype checker method
		/// </summary>
		/// <param name="majorType">Major type (child)</param>
		/// <param name="minorType">Minor type (parent)</param>
		/// <returns>subtype state</returns>
		public static bool IsSubtypeOf ( Type majorType, Type minorType )
		{
			if ( majorType == minorType || majorType.IsSubclassOf ( minorType ) )
				return true;
			else if ( minorType.IsInterface )
			{
				foreach ( Type type in majorType.GetInterfaces () )
					if ( type == minorType )
						return true;
			}
			return false;
		}

		[StructLayout ( LayoutKind.Sequential )]
		internal struct BITMAPFILEHEADER
		{
			public ushort bfType;
			public uint bfSize;
			public ushort bfReserved1;
			public ushort bfReserved2;
			public uint bfOffBits;
		}

		[StructLayout ( LayoutKind.Sequential )]
		internal struct BITMAPINFOHEADER
		{
			public uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.IO.Compression.Utilities
{
	static class ZlibConstants
	{
		public const int WindowBitsMax = 15;
		public const int WindowBitsDefault = WindowBitsMax;

		public const int Z_OK = 0;
		public const int Z_STREAM_END = 1;
		public const int Z_NEED_DICT = 2;
		public const int Z_STREAM_ERROR = -2;
		public const int Z_DATA_ERROR = -3;
		public const int Z_BUF_ERROR = -5;
       
        public const int WorkingBufferSizeDefault = 8192;
		public const int WorkingBufferSizeMin = 1024;

		internal static readonly int MAX_BITS = 15;
		internal static readonly int BL_CODES = 19;
		internal static readonly int D_CODES = 30;
		internal static readonly int LITERALS = 256;
		internal static readonly int LENGTH_CODES = 29;
		internal static readonly int L_CODES = ( LITERALS + 1 + LENGTH_CODES );

		internal static readonly int MAX_BL_BITS = 7;
		internal static readonly int REP_3_6 = 16;
		internal static readonly int REPZ_3_10 = 17;
		internal static readonly int REPZ_11_138 = 18;
	}
}

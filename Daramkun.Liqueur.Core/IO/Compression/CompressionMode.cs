using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.IO.Compression
{
	public enum FlushType
	{
		None = 0,
		Partial,
		Sync,
		Full,
		Finish,
	}

	public enum CompressionLevel
	{
		None = 0,
		Level0 = 0,
		BestSpeed = 1,
		Level1 = 1,
		Level2 = 2,
		Level3 = 3,
		Level4 = 4,
		Level5 = 5,
		Default = 6,
		Level6 = 6,
		Level7 = 7,
		Level8 = 8,
		BestCompression = 9,
		Level9 = 9,
	}

	public enum CompressionStrategy
	{
		Default = 0,
		Filtered = 1,
		HuffmanOnly = 2,
	}

	public enum CompressionMode
	{
		Compress = 0,
		Decompress = 1,
	}
}

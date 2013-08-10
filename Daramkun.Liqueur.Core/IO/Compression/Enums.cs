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
		BestSpeed = 1,
		Default = 6,
		BestCompression = 9,
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

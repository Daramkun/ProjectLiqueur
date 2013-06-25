using System;
using System.IO;

namespace Daramkun.Liqueur.Contents
{
	public interface IContentLoader
	{
		Type ContentType { get; }
		bool IsSelfStreamDispose { get; }
		object Load ( Stream stream, params object [] args );
	}
}

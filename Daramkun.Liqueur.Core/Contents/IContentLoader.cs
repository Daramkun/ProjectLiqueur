using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents
{
	public interface IContentLoader
	{
		Type ContentType { get; }
		IEnumerable<string> FileExtensions { get; }
		bool IsSelfStreamDispose { get; }
		object Load ( Stream stream, params object [] args );
	}
}

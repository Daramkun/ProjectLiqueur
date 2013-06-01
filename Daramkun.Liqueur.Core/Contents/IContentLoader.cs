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
		bool IsAutoStreamDispose { get; }
		object Load ( Stream stream, params object [] args );
	}
}

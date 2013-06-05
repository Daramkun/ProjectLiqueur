using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class SoundContentLoader : IContentLoader
	{
		public static Type SoundType { get; set; }
		public Type ContentType { get { return typeof ( ISoundPlayer ); } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return Activator.CreateInstance ( SoundType, stream );
		}
	}
}

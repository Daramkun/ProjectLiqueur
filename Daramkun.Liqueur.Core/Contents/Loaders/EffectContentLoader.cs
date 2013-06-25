using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class EffectContentLoader : IContentLoader
	{
		public static Type EffectType { get; set; }

		public Type ContentType { get { return typeof ( IEffect ); } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return Activator.CreateInstance ( EffectType, stream );
		}
	}
}

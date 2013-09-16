using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Animi.Graphics;

namespace Daramkun.Liqueur.Animi.Contents.Loaders
{
	public class FbxModelContentLoader : FbxModelContentLoader<BaseAnimiVertex>
	{
		
	}

	public class FbxModelContentLoader<T> : IContentLoader where T : struct
	{
		public Type ContentType { get { return typeof ( Model<T> ); } }
		public IEnumerable<string> FileExtensions { get { yield return "FX"; } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object[] args )
		{
			byte [] buffer = new byte [ 21 ];
			stream.Read ( buffer, 0, 21 );
			if ( !Encoding.UTF8.GetBytes ( "Kaydara FBX Binary  \0" ).Equals ( buffer ) )
			{

			}
		}
	}
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class ImageContentLoader : IContentLoader
	{
		public static Type ImageType { get; set; }
		public Type ContentType { get { return typeof ( IImage ); } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			if ( args.Length == 1 )
				return Activator.CreateInstance ( ImageType, stream, ( Color ) args [ 0 ] );
			else
				return Activator.CreateInstance ( ImageType, stream );
		}

		public IImage Instantiate ( int width, int height, Color [] pixels )
		{
			return Activator.CreateInstance ( ImageType, new ImageData ()
			{
				Width = width,
				Height = height,
				ImageDecoder = new DirectPixelDecoder(),
				Data = pixels
			} ) as IImage;
		}
	}
}

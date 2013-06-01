using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.IO;
using Daramkun.Liqueur.IO.Compression;

namespace Daramkun.Liqueur.Graphics.Fonts
{
	public sealed class LsfFont : BaseFont
	{
		private struct FontImageData { public int Width, Height; public ImageData ImageData;  }

		Dictionary<char, FontImageData> readedStream = new Dictionary<char, FontImageData> ();
		Dictionary<char, IImage> readedImage = new Dictionary<char, IImage> ();
		ImageContentLoader imageContentLoader;

		public LsfFont ( Stream stream )
		{
			using ( DeflateStream ds = new DeflateStream ( stream, CompressionMode.Decompress ) )
			{
				BinaryReader br = new BinaryReader ( ds );
				FontFamily = br.ReadString ();
				FontSize = br.ReadInt32 ();
				int bitmapCount = br.ReadInt32 ();
				for ( int i = 0; i < bitmapCount; i++ )
				{
					char ch = br.ReadChar ();
					int fileSize = br.ReadInt32 ();
					MemoryStream memStream = new MemoryStream ( br.ReadBytes ( fileSize ) );
					ImageData? imageData = ImageDecoders.GetImageData ( memStream );
					if ( imageData == null ) continue;
					readedStream.Add ( ch, new FontImageData ()
					{
						Width = imageData.Value.Width,
						Height = imageData.Value.Height,
						ImageData = imageData.Value
					} );
				}
			}

			imageContentLoader = new ImageContentLoader ();

			stream.Dispose ();
		}

		protected override void Dispose ( bool isDisposing)
		{
			if ( isDisposing )
			{
				readedStream.Clear ();
				foreach ( KeyValuePair<char, IImage> image in readedImage )
					image.Value.Dispose ();
				readedImage.Clear ();
			}
		}

		protected override IImage this [ char ch ]
		{
			get
			{
				if ( readedImage.ContainsKey ( ch ) )
					return readedImage [ ch ];
				else if ( readedStream.ContainsKey ( ch ) )
				{
					FontImageData fontImageData = readedStream [ ch ];
					IImage fontImage = imageContentLoader.Instantiate (
						fontImageData.Width,
						fontImageData.Height,
						fontImageData.ImageData.ImageDecoder.GetPixels ( fontImageData.ImageData, Color.Magenta )
					);
					readedImage.Add ( ch, fontImage );
					return fontImage;
				}
				else return null;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class Texture2DLoader : IContentLoader
	{
		public static List<IImageDecoder> Decoders { get; private set; }

		static Texture2DLoader ()
		{
			Decoders = new List<IImageDecoder> ();
		}

		public static void AddDefaultDecoders ()
		{
			Decoders.Add ( new BitmapDecoder () );
			Decoders.Add ( new PngDecoder () );
		}

		public Type ContentType { get { return typeof ( ITexture2D ); } }

		public IEnumerable<string> FileExtensions
		{
			get
			{
				foreach ( IImageDecoder decoder in Decoders )
					foreach ( object attr in decoder.GetType ().GetCustomAttributes ( typeof ( FileFormatAttribute ), true ) )
						foreach ( string ext in ( attr as FileFormatAttribute ).FileExtension )
							yield return ext;
			}
		}

		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			bool isLoadComplete = false;
			ImageInfo imageInfo = new ImageInfo ();
			foreach(IImageDecoder decoder in Decoders)
			{
				try
				{
					imageInfo = decoder.Decode ( stream );
					isLoadComplete = true;
					break;
				}
				catch { }
			}
			if ( isLoadComplete )
				return LiqueurSystem.GraphicsDevice.CreateTexture2D ( imageInfo, ( args.Length == 1 ) ? new Color? ( ( Color ) args [ 0 ] ) : null );
			else throw new FileFormatMismatchException ();
		}
	}
}

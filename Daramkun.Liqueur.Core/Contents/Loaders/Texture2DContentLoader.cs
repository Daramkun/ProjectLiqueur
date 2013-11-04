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
	/// <summary>
	/// Texture2D Content Loader
	/// </summary>
	public class Texture2DContentLoader : IContentLoader
	{
		/// <summary>
		/// Image Decoders
		/// </summary>
		public static List<IImageDecoder> Decoders { get; private set; }

		static Texture2DContentLoader ()
		{
			Decoders = new List<IImageDecoder> ();
		}

		/// <summary>
		/// Add Default Decoders
		/// (BMP, PNG)
		/// </summary>
		public static void AddDefaultDecoders ()
		{
			Decoders.Add ( new BitmapDecoder () );
			Decoders.Add ( new PngDecoder () );
		}

		/// <summary>
		/// Content Type (ITexture2D)
		/// </summary>
		public Type ContentType { get { return typeof ( ITexture2D ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
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

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return true; } }

		/// <summary>
		/// Load Texture2D Content
		/// </summary>
		/// <param name="stream">Image stream</param>
		/// <param name="args">If you need Color key value, set this argument</param>
		/// <returns>Loaded Texture2D</returns>
		public object Load ( Stream stream, params object [] args )
		{
			bool isLoadComplete = false;
			ImageInfo imageInfo = new ImageInfo ();
			foreach(IImageDecoder decoder in Decoders)
			{
				try
				{
					stream.Position = 0;
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

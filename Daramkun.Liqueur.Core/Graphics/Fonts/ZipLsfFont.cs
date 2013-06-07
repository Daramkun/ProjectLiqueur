﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Decoders;

namespace Daramkun.Liqueur.Graphics.Fonts
{
	public class ZipLsfFont : BaseFont
	{
		Dictionary<char, IImage> readedImage = new Dictionary<char, IImage> ();
		List<char> noneList = new List<char> ();
		ImageContentLoader imageContentLoader;

		ZipFileSystem fileSystem;

		private char GetChar ( string filename )
		{
			if ( filename [ 0 ] >= '0' && filename [ 0 ] <= '9' )
			{
				return ( char ) int.Parse ( filename.Substring ( 0, filename.LastIndexOf ( '.' ) - 1 ) );
			}
			else return filename [ 0 ];
		}

		public ZipLsfFont ( Stream stream )
		{
			fileSystem = new ZipFileSystem ( stream );
			Stream file = fileSystem.OpenFile ( "info.json" );
			JsonEntry entry = JsonParser.Parse ( file );
			FontFamily = entry [ "fontfamily" ].Data as string;
			FontSize = ( int ) entry [ "fontsize" ].Data;

			imageContentLoader = new ImageContentLoader ();
		}

		protected override void Dispose(bool isDisposing)
		{
			if ( isDisposing )
			{
				foreach ( KeyValuePair<char, IImage> image in readedImage )
					image.Value.Dispose ();
				readedImage.Clear ();
				fileSystem.Dispose ();
			}
 			base.Dispose(isDisposing);
		}

		private string IsExistCharFile ( char ch )
		{
			string filename = String.Format ( "{0}.bmp", ( int ) ch );
			if ( fileSystem.Files.Contains ( filename ) )
				return filename;
			filename = String.Format ( "{0}.png", ( int ) ch );
			if ( fileSystem.Files.Contains ( filename ) )
				return filename;
			filename = String.Format ( "{0}.bmp", ch );
			if ( fileSystem.Files.Contains ( filename ) )
				return filename;
			filename = String.Format ( "{0}.png", ch );
			if ( fileSystem.Files.Contains ( filename ) )
				return filename;
			return null;
		}

		protected override IImage this [ char ch ]
		{
			get
			{
				if ( readedImage.ContainsKey ( ch ) )
					return readedImage [ ch ];
				else
				{
					if ( noneList.Contains ( ch ) ) return null;
					string filename = IsExistCharFile ( ch );
					if ( filename == null )
					{
						noneList.Add ( ch );
						return null;
					}
					ImageData imageData = ImageDecoders.GetImageData ( fileSystem.OpenFile ( filename ) ).Value;
					IImage fontImage = imageContentLoader.Instantiate ( imageData.Width, imageData.Height,
						imageData.ImageDecoder.GetPixels ( imageData, Color.Magenta ) );
					readedImage.Add ( ch, fontImage );
					return fontImage;
				}
			}
		}
	}
}
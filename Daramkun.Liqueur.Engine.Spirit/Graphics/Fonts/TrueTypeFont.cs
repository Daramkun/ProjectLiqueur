using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Spirit.Graphics
{
	public class TrueTypeFont : Font
	{
		TrueTypeSharp.TrueTypeFont trueType;
		Dictionary<char, ITexture2D> readedImage = new Dictionary<char, ITexture2D> ();
		List<char> noneList = new List<char> ();

		float fontSize;
		int fontSizeOfPixel;

		public TrueTypeFont ( Stream trueTypeFontStream, int fontSizeOfPixel )
		{
			trueType = new TrueTypeSharp.TrueTypeFont ( trueTypeFontStream );
			fontSize = trueType.GetScaleForPixelHeight ( this.fontSizeOfPixel = fontSizeOfPixel );
		}

		protected override void Dispose ( bool isDisposing )
		{
			base.Dispose ( isDisposing );
		}

		protected override ITexture2D this [ char ch ]
		{
			get
			{
				if ( noneList.Contains ( ch ) ) return null;
				if ( readedImage.ContainsKey ( ch ) ) return readedImage [ ch ];

				if ( ch == ' ' )
				{
					Color [] buffer = new Color [ fontSizeOfPixel * ( fontSizeOfPixel / 3 ) ];
					ImageInfo imageInfo = new ImageInfo ()
					{
						Width = fontSizeOfPixel / 3,
						Height = fontSizeOfPixel,
						ImageDecoder = new ImageDirectCopyDecoder ( buffer ),
					};
					ITexture2D texture = LiqueurSystem.GraphicsDevice.CreateTexture2D ( imageInfo );
					readedImage.Add ( ch, texture );
					return texture;
				}
				else
				{
					int width, height, xOffset, yOffset;
					byte [] data = trueType.GetCodepointBitmap ( ch, fontSize, fontSize, out width, out height, out xOffset, out yOffset );

					if ( data == null ) { noneList.Add ( ch ); return null; }

					int extendedHeight = Math.Abs ( yOffset ) * ( width/* + xOffset*/ );
					Color [] buffer = new Color [ ( width/* + xOffset*/ ) * fontSizeOfPixel ];
					for ( int x = 0; x < width; x++ )
					{
						for ( int y = 0; y < height; y++ )
						{
							int dataIndex = ( y * width ) + x;
							int index = ( ( y + ( fontSizeOfPixel / 3 * 2 ) + yOffset ) * ( width/* + xOffset*/ ) ) + x;
							if ( index >= buffer.Length || index < 0 ) continue;
							buffer [ index ] = data [ dataIndex ] > 0 ? new Color ( 255, 255, 255, data [ dataIndex ] ) : new Color ( 0, 0, 0, 0 );
						}
					}

					ImageInfo imageInfo = new ImageInfo ()
					{
						Width = width/* + xOffset*/,
						Height = fontSizeOfPixel,
						ImageDecoder = new ImageDirectCopyDecoder ( buffer ),
					};
					ITexture2D texture = LiqueurSystem.GraphicsDevice.CreateTexture2D ( imageInfo );
					readedImage.Add ( ch, texture );
					return texture;
				}
			}
		}
	}
}

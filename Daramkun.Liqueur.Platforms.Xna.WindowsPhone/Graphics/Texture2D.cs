using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Graphics
{
	class Texture2D : ITexture2D
	{
		Microsoft.Xna.Framework.Graphics.Texture2D texture;

		public int Width { get { return texture.Width; } }
		public int Height { get { return texture.Height; } }
		public Vector2 Size { get { return new Vector2 ( Width, Height ); } }

		public Color [] Buffer
		{
			get
			{
				Color [] data = new Color [ Width * Height ];
				texture.GetData<Color> ( data );
				return data;
			}
			set { texture.SetData<Color> ( value ); }
		}

		public object Handle { get { return texture; } }

		public Texture2D ( IGraphicsDevice graphicsDevice, int width, int height )
		{
			texture = new Microsoft.Xna.Framework.Graphics.Texture2D ( graphicsDevice.Handle as Microsoft.Xna.Framework.Graphics.GraphicsDevice,
				width, height );
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, ImageInfo imageInfo, Color? colorKey )
			: this ( graphicsDevice, imageInfo.Width, imageInfo.Height )
		{
			Buffer = imageInfo.GetPixel ( colorKey );
		}

		~Texture2D ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				texture.Dispose ();
				texture = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}

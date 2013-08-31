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
		SharpDX.Direct3D11.Texture2D texture;

		public int Width { get { throw new NotImplementedException (); } }
		public int Height { get { throw new NotImplementedException (); } }

		public Vector2 Size
		{
			get { throw new NotImplementedException (); }
		}

		public Color [] Buffer
		{
			get
			{

				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public object Handle { get { return texture; } }

		public Texture2D ( IGraphicsDevice graphicsDevice, ImageInfo imageInfo, Color? colorKey )
		{
			texture = new SharpDX.Direct3D11.Texture2D ( graphicsDevice.Handle as SharpDX.Direct3D11.Device,
				new SharpDX.Direct3D11.Texture2DDescription ()
				{
					Width = imageInfo.Width,
					Height = imageInfo.Height,
					SampleDescription = new SharpDX.DXGI.SampleDescription ( 1, 0 ),
					Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
					ArraySize = 1,
					MipLevels = 1,
				} );
			Buffer = imageInfo.GetPixel ( colorKey );
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
}

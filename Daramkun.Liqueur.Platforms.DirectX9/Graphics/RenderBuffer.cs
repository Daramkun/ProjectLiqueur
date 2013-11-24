using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Graphics
{
	class RenderBuffer : Texture2D, IRenderBuffer
	{
		internal SharpDX.Direct3D9.RenderToSurface rts;

		public RenderBuffer ( IGraphicsDevice graphicsDevice, int width, int height )
			: base ( graphicsDevice, width, height )
		{
			rts = new SharpDX.Direct3D9.RenderToSurface ( graphicsDevice.Handle as SharpDX.Direct3D9.Device,
				width, height, SharpDX.Direct3D9.Format.A8R8G8B8, true, SharpDX.Direct3D9.Format.D24S8 );
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				rts.Dispose ();
			}
			base.Dispose ( isDisposing );
		}
	}
}

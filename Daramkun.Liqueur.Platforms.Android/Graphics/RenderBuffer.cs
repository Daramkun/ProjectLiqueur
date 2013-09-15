using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;
using OpenTK.Graphics.ES20;

namespace Daramkun.Liqueur.Graphics
{
	class RenderBuffer : Texture2D, IRenderBuffer
	{
		internal int frameBuffer;
		int depthBuffer;

		public RenderBuffer ( IGraphicsDevice graphicsDevice, int width, int height )
			: base ( graphicsDevice, width, height)
		{
			GL.GenFramebuffers ( 1, out depthBuffer );
			GL.BindFramebuffer ( All.Framebuffer, depthBuffer );
			GL.RenderbufferStorage ( All.Renderbuffer, All.Depth24Stencil8Oes, width, height );

			GL.GenFramebuffers ( 1, out frameBuffer );
			GL.BindFramebuffer ( All.Framebuffer, frameBuffer );
			GL.FramebufferRenderbuffer ( All.Framebuffer, All.DepthStencilOes, All.Renderbuffer, depthBuffer );
			GL.FramebufferTexture2D ( All.Framebuffer, All.ColorAttachment0, All.Texture, texture, 0 );

			GL.BindFramebuffer ( All.Framebuffer, 0 );
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				GL.DeleteFramebuffers ( 1, ref depthBuffer );
				GL.DeleteFramebuffers ( 1, ref frameBuffer );
			}

			base.Dispose ( isDisposing );
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;
using OpenTK.Graphics.OpenGL;

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
			GL.BindFramebuffer ( FramebufferTarget.Framebuffer, depthBuffer );
			GL.RenderbufferStorage ( RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height );

			GL.GenFramebuffers ( 1, out frameBuffer );
			GL.BindFramebuffer ( FramebufferTarget.Framebuffer, frameBuffer );
			GL.FramebufferRenderbuffer ( FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthBuffer );
			GL.FramebufferTexture2D ( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0 );

			GL.BindFramebuffer ( FramebufferTarget.Framebuffer, 0 );
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

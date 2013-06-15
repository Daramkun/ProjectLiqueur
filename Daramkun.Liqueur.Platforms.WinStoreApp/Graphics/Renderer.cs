using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Platforms;
#if OPENTK
using OpenTK.Graphics.OpenGL;
#elif XNA
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Daramkun.Liqueur.Graphics
{
	class Renderer : IRenderer, IDisposable
	{
		Window window;
		Vector2 screenSize;


		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> screenSizes = new List<Vector2> ();

				return screenSizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return screenSize; }
			set
			{
				screenSize = value;

			}
		}

		public bool FullscreenMode
		{
			get { return true; }
			set { }
		}

		public Renderer ( Window window )
		{
			this.window = window;
#if OPENTK
			window.window.Resize += ( object sender, EventArgs e ) =>
			{
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				GL.Ortho ( 0, 800, 600, 0, -0.0001f, 1000.0f );
			};
#elif XNA
#if WINDOWS_PHONE
			SpriteBatch = new SpriteBatch ( Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.GraphicsDevice );
#endif
#endif
		}

		public void Dispose ()
		{
#if XNA
			SpriteBatch.Dispose ();
			SpriteBatch = null;
#endif
		}

		public void Begin2D ()
		{
#if OPENTK
			GL.MatrixMode ( MatrixMode.Modelview );
			GL.LoadIdentity ();

			GL.Enable ( EnableCap.Texture2D );
			GL.Enable ( EnableCap.Blend );

			GL.BlendFunc ( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );

			GL.EnableClientState ( ArrayCap.VertexArray );
			GL.EnableClientState ( ArrayCap.TextureCoordArray );
#elif XNA
			SpriteBatch.Begin ();
#endif
		}

		public void End2D ()
		{
#if OPENTK
			GL.DisableClientState ( ArrayCap.TextureCoordArray );
			GL.DisableClientState ( ArrayCap.VertexArray );

			GL.Disable ( EnableCap.Blend );
			GL.Disable ( EnableCap.Texture2D );
#elif XNA
			SpriteBatch.End ();
#endif
		}

		public void Clear ( Color color )
		{
#if OPENTK
			GL.ClearColor ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar );
			GL.Clear ( ClearBufferMask.ColorBufferBit );
#elif XNA
			Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear ( 
				new Microsoft.Xna.Framework.Color ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar ) );
#endif
		}

		public void Present ()
		{
#if OPENTK
			window.window.SwapBuffers ();
#elif XNA
			Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.GraphicsDevice.Present ();
#endif
		}
	}
}

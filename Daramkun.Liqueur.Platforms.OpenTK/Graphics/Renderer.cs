using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
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
				foreach ( OpenTK.DisplayResolution resolution in
					OpenTK.DisplayDevice.Default.AvailableResolutions )
					screenSizes.Add ( new Vector2 ( resolution.Width, resolution.Height ) );
				return screenSizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return screenSize; }
			set
			{
				screenSize = value;
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				window.window.ClientSize = new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y );
				GL.Ortho ( 0, screenSize.X, screenSize.Y, 0, 0.0001f, 1000.0f );
			}
		}

		public bool FullscreenMode
		{
			get
			{
				return window.window.WindowState == OpenTK.WindowState.Fullscreen;
			}
			set
			{
				window.window.WindowState = ( value ) ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;
			}
		}

		public Renderer ( Window window )
		{
			this.window = window;
			window.window.Resize += ( object sender, EventArgs e ) =>
			{
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				GL.Ortho ( 0, 800, 600, 0, -0.0001f, 1000.0f );
			};

			
		}

		public void Dispose ()
		{

		}

		public void Begin2D ()
		{
			GL.MatrixMode ( MatrixMode.Modelview );
			GL.LoadIdentity ();

			GL.Enable ( EnableCap.Texture2D );
			GL.Enable ( EnableCap.Blend );

			GL.BlendFunc ( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );

			GL.EnableClientState ( ArrayCap.VertexArray );
			GL.EnableClientState ( ArrayCap.TextureCoordArray );
		}

		public void End2D ()
		{
			GL.DisableClientState ( ArrayCap.TextureCoordArray );
			GL.DisableClientState ( ArrayCap.VertexArray );

			GL.Disable ( EnableCap.Blend );
			GL.Disable ( EnableCap.Texture2D );
		}

		public void Clear ( Color color )
		{
			GL.ClearColor ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar );
			GL.Clear ( ClearBufferMask.ColorBufferBit );
		}

		public void Present ()
		{
			window.window.SwapBuffers ();
		}
	}
}

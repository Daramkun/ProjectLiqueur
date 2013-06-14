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

#if XNA
		internal GraphicsDevice GraphicsDevice
		{
			get { return null; }
		}

		internal SpriteBatch SpriteBatch { get; private set; }
#endif

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> screenSizes = new List<Vector2> ();
#if OPENTK
				foreach ( OpenTK.DisplayResolution resolution in
					OpenTK.DisplayDevice.Default.AvailableResolutions )
					screenSizes.Add ( new Vector2 ( resolution.Width, resolution.Height ) );
#elif XNA
				foreach ( DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes )
					screenSizes.Add ( new Vector2 ( displayMode.Width, displayMode.Height ) );
#endif
				return screenSizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return screenSize; }
			set
			{
				screenSize = value;
#if OPENTK
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				window.ClientSize = value;
				GL.Ortho ( 0, screenSize.X, screenSize.Y, 0, 0.0001f, 1000.0f );
#elif XNA
#if WINDOWS_PHONE
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = ( int ) value.X;
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = ( int ) value.Y;
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.ApplyChanges ();
#endif
#endif
			}
		}

		public bool FullscreenMode
		{
			get
			{
#if OPENTK
				return window.window.WindowState == OpenTK.WindowState.Fullscreen;
#elif XNA
#if WINDOWS_PHONE
				return true;
#endif
#endif
			}
			set
			{
#if OPENTK
				window.window.WindowState = ( value ) ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;
#elif XNA

#endif
			}
		}

		public Renderer ( Window window )
		{
			this.window = window;
#if OPENTK
			window.window.Resize += ( object sender, EventArgs e ) =>
			{
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				GL.Ortho ( 0, window.ClientSize.X, window.ClientSize.Y, 0, -0.0001f, 1000.0f );
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;
using Microsoft.Xna.Framework.Graphics;

namespace Daramkun.Liqueur.Graphics
{
	class GraphicsDevice : IGraphicsDevice
	{
		Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice;
		Microsoft.Xna.Framework.GraphicsDeviceManager graphicsDeviceManager;

		public object Handle { get { return graphicsDevice; } }

		public BaseRenderer BaseRenderer { get { return Graphics.BaseRenderer.XNA; } }
		public Version RendererVersion { get { return new Version ( 4, 0 ); } }

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> displayModes = new List<Vector2>();
				foreach ( DisplayMode displayMode in graphicsDevice.Adapter.SupportedDisplayModes )
					displayModes.Add ( new Vector2 ( displayMode.Width, displayMode.Height ) );
				return displayModes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return new Vector2 ( graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight ); }
			set
			{
				graphicsDeviceManager.PreferredBackBufferWidth = ( int ) value.X;
				graphicsDeviceManager.PreferredBackBufferHeight = ( int ) value.Y;
				graphicsDeviceManager.ApplyChanges ();
			}
		}

		public bool FullscreenMode
		{
			get { return graphicsDeviceManager.IsFullScreen; }
			set { graphicsDeviceManager.IsFullScreen = value; graphicsDeviceManager.ApplyChanges (); }
		}

		public bool VerticalSyncMode
		{
			get { return graphicsDeviceManager.SynchronizeWithVerticalRetrace; }
			set { graphicsDeviceManager.SynchronizeWithVerticalRetrace = value; graphicsDeviceManager.ApplyChanges (); }
		}

		public CullingMode CullingMode
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

		public FillMode FillMode
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

		public bool IsZWriteEnable
		{
			get { return graphicsDevice.DepthStencilState.DepthBufferEnable; }
			set { graphicsDevice.DepthStencilState.DepthBufferEnable = value; }
		}

		public bool BlendState
		{
			get { return graphicsDevice.BlendState == Microsoft.Xna.Framework.Graphics.BlendState.Opaque; }
			set
			{
				throw new NotImplementedException ();
			}
		}

		public bool StencilState
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

		public BlendOperation BlendOperation
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

		public StencilOperation StencilOperation
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

		public IRenderBuffer RenderTarget
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

		public Viewport Viewport
		{
			get
			{
				return new Viewport ()
				{
					X = graphicsDevice.Viewport.X,
					Y = graphicsDevice.Viewport.Y,
					Width = graphicsDevice.Viewport.Width,
					Height = graphicsDevice.Viewport.Height
				};
			}
			set
			{
				graphicsDevice.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport (
					value.X, value.Y, value.Width, value.Height );
			}
		}

		public void BeginScene () { }
		public void EndScene () { }

		public GraphicsDevice ( Microsoft.Xna.Framework.Game game )
		{
			graphicsDevice = game.GraphicsDevice;
			graphicsDeviceManager = ( game as GraphicsDeviceManagerInGame ).GraphicsDeviceManager;
			ScreenSize = new Vector2 ( 800, 600 );
		}

		public void Dispose ()
		{

		}

		public void Clear ( ClearBuffer clearBuffer, Color color )
		{
			Microsoft.Xna.Framework.Graphics.ClearOptions options = ( Microsoft.Xna.Framework.Graphics.ClearOptions ) 0;
			if ( ( clearBuffer & ClearBuffer.ColorBuffer ) != 0 ) options |= Microsoft.Xna.Framework.Graphics.ClearOptions.Target;
			if ( ( clearBuffer & ClearBuffer.DepthBuffer ) != 0 ) options |= Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer;
			if ( ( clearBuffer & ClearBuffer.StencilBuffer ) != 0 ) options |= Microsoft.Xna.Framework.Graphics.ClearOptions.Stencil;
			graphicsDevice.Clear ( options,
				new Microsoft.Xna.Framework.Color ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar ),
				1, 1 );
		}

		public void SwapBuffer ()
		{
			graphicsDevice.Present ();
		}

		public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer ) where T : struct
		{
			throw new NotImplementedException ();
		}

		public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer ) where T : struct
		{
			throw new NotImplementedException ();
		}

		public ITexture2D CreateTexture2D ( int width, int height )
		{
			return new Texture2D ( this, width, height );
		}

		public ITexture2D CreateTexture2D ( ImageInfo imageInfo )
		{
			return CreateTexture2D ( imageInfo, null );
		}

		public ITexture2D CreateTexture2D ( ImageInfo imageInfo, Color? colorKey )
		{
			return new Texture2D ( this, imageInfo, colorKey );
		}

		public IRenderBuffer CreateRenderBuffer ( int width, int height )
		{
			throw new NotImplementedException ();
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, int vertexCount ) where T : struct
		{
			throw new NotImplementedException ();
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, T [] vertices ) where T : struct
		{
			throw new NotImplementedException ();
		}

		public IIndexBuffer CreateIndexBuffer ( int indexCount )
		{
			throw new NotImplementedException ();
		}

		public IIndexBuffer CreateIndexBuffer ( int [] indices )
		{
			throw new NotImplementedException ();
		}

		public IShader CreateShader ( Stream stream, ShaderType shaderType )
		{
			throw new NotImplementedException ();
		}

		public IShader CreateShader ( string code, ShaderType shaderType )
		{
			throw new NotImplementedException ();
		}

		public IEffect CreateEffect ( params IShader [] shaders )
		{
			throw new NotImplementedException ();
		}
	}
}

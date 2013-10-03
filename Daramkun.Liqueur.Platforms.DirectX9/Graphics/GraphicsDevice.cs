using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Graphics
{
	class GraphicsDevice : IGraphicsDevice
	{
		SharpDX.Direct3D9.Direct3D direct3d;
		SharpDX.Direct3D9.Device d3dDevice;
		SharpDX.Direct3D9.PresentParameters d3dPp;

		public object Handle { get { return d3dDevice; } }

		public BaseRenderer BaseRenderer { get { return BaseRenderer.DirectX; } }
		public Version RendererVersion { get { return new Version ( 9, 0 ); } }

		public Mathematics.Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Mathematics.Vector2> sizes = new List<Mathematics.Vector2> ();
				int count = direct3d.GetAdapterModeCount ( 0, SharpDX.Direct3D9.Format.A8R8G8B8 );
				for ( int i = 0; i < count; i++ )
				{
					SharpDX.Direct3D9.DisplayMode mode = direct3d.EnumAdapterModes ( 0, SharpDX.Direct3D9.Format.A8R8G8B8, count );
					sizes.Add ( new Mathematics.Vector2 ( mode.Width, mode.Height ) );
				}
				return sizes.ToArray ();
			}
		}

		public Mathematics.Vector2 ScreenSize
		{
			get { return new Mathematics.Vector2 ( d3dPp.BackBufferWidth, d3dPp.BackBufferHeight ); }
			set
			{
				d3dPp.BackBufferWidth = ( int ) value.X;
				d3dPp.BackBufferHeight = ( int ) value.Y;
				d3dDevice.Reset ( d3dPp );
			}
		}

		public bool FullscreenMode
		{
			get { return !d3dPp.Windowed; }
			set { d3dPp.Windowed = !value; d3dDevice.Reset ( d3dPp ); }
		}

		public bool VerticalSyncMode
		{
			get { return d3dPp.PresentationInterval == SharpDX.Direct3D9.PresentInterval.Default; }
			set
			{ 
				d3dPp.PresentationInterval = ( value ) ? SharpDX.Direct3D9.PresentInterval.Default : SharpDX.Direct3D9.PresentInterval.Immediate;
				d3dDevice.Reset ( d3dPp );
			}
		}

		public CullingMode CullingMode
		{
			get { return ConvertCullMode ( d3dDevice.GetRenderState ( SharpDX.Direct3D9.RenderState.CullMode ) ); }
			set { d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.CullMode, ChangeCullMode ( value ) ); }
		}

		private int ChangeCullMode ( Graphics.CullingMode value )
		{
			switch ( value )
			{
				case Graphics.CullingMode.None: return ( int ) SharpDX.Direct3D9.Cull.None;
				case Graphics.CullingMode.ClockWise: return ( int ) SharpDX.Direct3D9.Cull.Clockwise;
				case Graphics.CullingMode.CounterClockWise: return ( int ) SharpDX.Direct3D9.Cull.Counterclockwise;
				default: return -1;
			}
		}

		private Graphics.CullingMode ConvertCullMode ( int p )
		{
			switch ( ( SharpDX.Direct3D9.Cull ) p )
			{
				case SharpDX.Direct3D9.Cull.None: return Graphics.CullingMode.None;
				case SharpDX.Direct3D9.Cull.Clockwise: return Graphics.CullingMode.ClockWise;
				case SharpDX.Direct3D9.Cull.Counterclockwise: return Graphics.CullingMode.CounterClockWise;
				default: return ( Graphics.CullingMode ) ( -1 );
			}
		}

		public FillMode FillMode
		{
			get { return ConvertFillMode ( d3dDevice.GetRenderState ( SharpDX.Direct3D9.RenderState.FillMode ) ); }
			set { d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.FillMode, ChangeFillMode ( value ) ); }
		}

		private int ChangeFillMode ( Graphics.FillMode value )
		{
			switch ( value )
			{
				case Graphics.FillMode.Point: return ( int ) SharpDX.Direct3D9.FillMode.Point;
				case Graphics.FillMode.Wireframe: return ( int ) SharpDX.Direct3D9.FillMode.Wireframe;
				case Graphics.FillMode.Solid: return ( int ) SharpDX.Direct3D9.FillMode.Solid;
				default: return -1;
			}
		}

		private Graphics.FillMode ConvertFillMode ( int p )
		{
			switch ( ( SharpDX.Direct3D9.FillMode ) p )
			{
				case SharpDX.Direct3D9.FillMode.Solid: return Graphics.FillMode.Solid;
				case SharpDX.Direct3D9.FillMode.Wireframe: return Graphics.FillMode.Wireframe;
				case SharpDX.Direct3D9.FillMode.Point: return Graphics.FillMode.Point;
				default: return ( Graphics.FillMode ) ( -1 );
			}
		}

		public Viewport Viewport
		{
			get
			{
				SharpDX.Viewport viewPort = d3dDevice.Viewport;
				return new Viewport () { X = viewPort.X, Y = viewPort.Y, Width = viewPort.Width, Height = viewPort.Height };
			}
			set
			{
				d3dDevice.Viewport = new SharpDX.Viewport ( value.X, value.Y, value.Width, value.Height );
			}
		}

		public bool IsZWriteEnable
		{
			get { return d3dDevice.GetRenderState ( SharpDX.Direct3D9.RenderState.ZWriteEnable ) != 0 ? true : false; }
			set { d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.ZWriteEnable, value ); }
		}

		public bool BlendState
		{
			get { return d3dDevice.GetRenderState ( SharpDX.Direct3D9.RenderState.AlphaBlendEnable ) != 0 ? true : false; }
			set { d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.AlphaBlendEnable, value ); }
		}

		public bool StencilState
		{
			get { return d3dDevice.GetRenderState ( SharpDX.Direct3D9.RenderState.StencilEnable ) != 0 ? true : false; }
			set { d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.StencilEnable, value ); }
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

		public GraphicsDevice ( IWindow window )
		{
			direct3d = new SharpDX.Direct3D9.Direct3D ();

			IntPtr handle = ( window.Handle as System.Windows.Forms.Form ).Handle;

			d3dPp = new SharpDX.Direct3D9.PresentParameters ( 800, 600, SharpDX.Direct3D9.Format.A8R8G8B8,
				1, SharpDX.Direct3D9.MultisampleType.None, 0, SharpDX.Direct3D9.SwapEffect.Discard,
				handle, true, true, SharpDX.Direct3D9.Format.D24S8, SharpDX.Direct3D9.PresentFlags.None,
				0, SharpDX.Direct3D9.PresentInterval.Immediate );

			d3dDevice = new SharpDX.Direct3D9.Device ( direct3d, 0, SharpDX.Direct3D9.DeviceType.Hardware,
				handle, SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing,
				d3dPp );
		}

		public void Dispose ()
		{
			d3dDevice.Dispose ();
			direct3d.Dispose ();
		}

		public void BeginScene ()
		{
			d3dDevice.BeginScene ();
		}

		public void EndScene ()
		{
			d3dDevice.EndScene ();
		}

		public void Clear ( ClearBuffer clearBuffer, Color color )
		{
			d3dDevice.Clear ( ChangeClearBuffer ( clearBuffer ), ChangeColor ( color ), 1, 0 );
		}

		private SharpDX.Direct3D9.ClearFlags ChangeClearBuffer ( ClearBuffer clearBuffer )
		{
			SharpDX.Direct3D9.ClearFlags clearFlags = SharpDX.Direct3D9.ClearFlags.None;
			if ( clearBuffer.HasFlag ( ClearBuffer.ColorBuffer ) ) clearFlags |= SharpDX.Direct3D9.ClearFlags.Target;
			if ( clearBuffer.HasFlag ( ClearBuffer.DepthBuffer ) ) clearFlags |= SharpDX.Direct3D9.ClearFlags.ZBuffer;
			if ( clearBuffer.HasFlag ( ClearBuffer.StencilBuffer ) ) clearFlags |= SharpDX.Direct3D9.ClearFlags.Stencil;
			return clearFlags;
		}

		private SharpDX.ColorBGRA ChangeColor ( Color color )
		{
			return new SharpDX.ColorBGRA ( new SharpDX.Vector3 ( color.RedScalar, color.GreenScalar, color.BlueScalar ), color.AlphaScalar );
		}

		public void SwapBuffer ()
		{
			d3dDevice.Present ();
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
			throw new NotImplementedException ();
		}

		public ITexture2D CreateTexture2D ( Contents.ImageInfo imageInfo, Color? colorKey = null )
		{
			throw new NotImplementedException ();
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

		public IShader CreateShader ( System.IO.Stream stream, ShaderType shaderType )
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

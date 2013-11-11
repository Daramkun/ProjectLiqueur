using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Graphics
{
	class GraphicsDevice : IGraphicsDevice
	{
		SharpDX.Direct3D9.Direct3D d3d;
		SharpDX.Direct3D9.Device d3dDevice;
		SharpDX.Direct3D9.PresentParameters d3dpp;

		public object Handle { get { return d3dDevice; } }

		public BaseRenderer BaseRenderer { get { return BaseRenderer.DirectX; } }
		public Version RendererVersion { get { return new Version ( 9, 0 ); } }

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> sizes = new List<Vector2> ();
				int count = d3d.GetAdapterModeCount ( 0, SharpDX.Direct3D9.Format.A8R8G8B8 );
				for ( int i = 0; i < count; i++ )
				{
					SharpDX.Direct3D9.DisplayMode mode = d3d.EnumAdapterModes ( 0, SharpDX.Direct3D9.Format.A8R8G8B8, count );
					sizes.Add ( new Mathematics.Vector2 ( mode.Width, mode.Height ) );
				}
				return sizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return new Vector2 ( d3dpp.BackBufferWidth, d3dpp.BackBufferHeight ); }
			set { d3dpp.BackBufferWidth = ( int ) value.X; d3dpp.BackBufferHeight = ( int ) value.Y; d3dDevice.Reset ( d3dpp ); }
		}

		public bool FullscreenMode
		{
			get { return !d3dpp.Windowed; }
			set { d3dpp.Windowed = !value; d3dDevice.Reset ( d3dpp ); }
		}

		public bool VerticalSyncMode
		{
			get { return d3dpp.PresentationInterval == SharpDX.Direct3D9.PresentInterval.Default; }
			set
			{
				d3dpp.PresentationInterval = ( value ) ? SharpDX.Direct3D9.PresentInterval.Default : SharpDX.Direct3D9.PresentInterval.Immediate;
				d3dDevice.Reset ( d3dpp );
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
			set { d3dDevice.Viewport = new SharpDX.Viewport ( value.X, value.Y, value.Width, value.Height ); }
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
				d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.BlendOperation, ConvertBlendOp ( value.Operator ) );
				d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.SourceBlend, ConvertBlendParam ( value.SourceParameter ) );
				d3dDevice.SetRenderState ( SharpDX.Direct3D9.RenderState.DestinationBlend, ConvertBlendParam ( value.DestinationParameter ) );
			}
		}

		private int ConvertBlendParam ( BlendParameter blendParameter )
		{
			switch ( blendParameter )
			{
				case BlendParameter.Zero: return ( int ) SharpDX.Direct3D9.Blend.Zero;
				case BlendParameter.One: return ( int ) SharpDX.Direct3D9.Blend.One;
				case BlendParameter.SourceColor: return ( int ) SharpDX.Direct3D9.Blend.SourceColor;
				case BlendParameter.SourceAlpha: return ( int ) SharpDX.Direct3D9.Blend.SourceAlpha;
				case BlendParameter.DestinationColor: return ( int ) SharpDX.Direct3D9.Blend.DestinationColor;
				case BlendParameter.DestinationAlpha: return ( int ) SharpDX.Direct3D9.Blend.DestinationAlpha;
				case BlendParameter.InvertSourceColor: return ( int ) SharpDX.Direct3D9.Blend.InverseSourceColor;
				case BlendParameter.InvertSourceAlpha: return ( int ) SharpDX.Direct3D9.Blend.InverseSourceAlpha;
				case BlendParameter.InvertDestinationColor: return ( int ) SharpDX.Direct3D9.Blend.InverseDestinationColor;
				case BlendParameter.InvertDestinationAlpha: return ( int ) SharpDX.Direct3D9.Blend.InverseDestinationAlpha;

				default: throw new ArgumentException ();
			}
		}

		private int ConvertBlendOp ( BlendOperator blendOperator )
		{
			switch ( blendOperator )
			{
				case BlendOperator.Add: return ( int ) SharpDX.Direct3D9.BlendOperation.Add;
				case BlendOperator.Minimum: return ( int ) SharpDX.Direct3D9.BlendOperation.Minimum;
				case BlendOperator.Maximum: return ( int ) SharpDX.Direct3D9.BlendOperation.Maximum;
				case BlendOperator.Subtract: return ( int ) SharpDX.Direct3D9.BlendOperation.Subtract;
				case BlendOperator.ReverseSubtract: return ( int ) SharpDX.Direct3D9.BlendOperation.ReverseSubtract;
				default: throw new ArgumentException ();
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
			d3d = new SharpDX.Direct3D9.Direct3D ();
			
			IntPtr handle = ( window.Handle as System.Windows.Forms.Form ).Handle;

			d3dpp = new SharpDX.Direct3D9.PresentParameters ( 800, 600, SharpDX.Direct3D9.Format.A8R8G8B8,
					1, SharpDX.Direct3D9.MultisampleType.None, 0, SharpDX.Direct3D9.SwapEffect.Discard,
					handle, true, true, SharpDX.Direct3D9.Format.D24S8, SharpDX.Direct3D9.PresentFlags.None,
					0, SharpDX.Direct3D9.PresentInterval.Immediate );

			try
			{
				d3dDevice = new SharpDX.Direct3D9.Device ( d3d, 0, SharpDX.Direct3D9.DeviceType.Hardware,
						handle, SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing,
						d3dpp );
			}
			catch
			{
				d3dDevice = new SharpDX.Direct3D9.Device ( d3d, 0, SharpDX.Direct3D9.DeviceType.Hardware,
						handle, SharpDX.Direct3D9.CreateFlags.SoftwareVertexProcessing,
						d3dpp );
			}

		}

		public void Dispose ()
		{
			d3dDevice.Dispose ();
			d3d.Dispose ();
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
			d3dDevice.VertexFormat = ConvertFVF ( vertexBuffer.FVF );
			d3dDevice.SetStreamSource ( 0, vertexBuffer.Handle as SharpDX.Direct3D9.VertexBuffer, 0, Marshal.SizeOf ( typeof ( T ) ) );
			d3dDevice.VertexDeclaration = ( vertexBuffer as VertexBuffer<T> ).vertexDeclaration;
			d3dDevice.DrawPrimitives ( ConvertPrimitiveType ( primitiveType ), 0, vertexBuffer.Length / GetPrimitiveUnit ( primitiveType ) );
		}

		public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer ) where T : struct
		{
			d3dDevice.VertexFormat = ConvertFVF ( vertexBuffer.FVF );
			d3dDevice.SetStreamSource ( 0, vertexBuffer.Handle as SharpDX.Direct3D9.VertexBuffer, 0, Marshal.SizeOf ( typeof ( T ) ) );
			d3dDevice.VertexDeclaration = ( vertexBuffer as VertexBuffer<T> ).vertexDeclaration;
			d3dDevice.Indices = indexBuffer.Handle as SharpDX.Direct3D9.IndexBuffer;
			d3dDevice.DrawIndexedPrimitive ( ConvertPrimitiveType ( primitiveType ), 0, 0, vertexBuffer.Length,
				0, indexBuffer.Length / GetPrimitiveUnit ( primitiveType ) );
		}

		private SharpDX.Direct3D9.VertexFormat ConvertFVF ( FlexibleVertexFormat flexibleVertexFormat )
		{
			SharpDX.Direct3D9.VertexFormat vertexFormat = SharpDX.Direct3D9.VertexFormat.None;
			
			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.PositionXY ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Position;
			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.PositionXYZ ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Position;

			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.Diffuse ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Diffuse;

			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.Normal ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Normal;

			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.TextureUV1 ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Texture1;
			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.TextureUV2 ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Texture2;
			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.TextureUV3 ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Texture3;
			if ( flexibleVertexFormat.HasFlag ( FlexibleVertexFormat.TextureUV4 ) )
				vertexFormat |= SharpDX.Direct3D9.VertexFormat.Texture4;

			return vertexFormat;
		}

		private SharpDX.Direct3D9.PrimitiveType ConvertPrimitiveType ( PrimitiveType primitiveType )
		{
			switch ( primitiveType )
			{
				case PrimitiveType.PointList: return SharpDX.Direct3D9.PrimitiveType.PointList;
				case PrimitiveType.LineList: return SharpDX.Direct3D9.PrimitiveType.LineList;
				case PrimitiveType.LineStrip: return SharpDX.Direct3D9.PrimitiveType.LineStrip;
				case PrimitiveType.TriangleList: return SharpDX.Direct3D9.PrimitiveType.TriangleList;
				case PrimitiveType.TriangleStrip: return SharpDX.Direct3D9.PrimitiveType.TriangleStrip;
				case PrimitiveType.TriangleFan: return SharpDX.Direct3D9.PrimitiveType.TriangleFan;
				default: throw new ArgumentException ();
			}
		}

		private int GetPrimitiveUnit ( PrimitiveType primitiveType )
		{
			switch ( primitiveType )
			{
				case PrimitiveType.PointList: return 1;
				case PrimitiveType.LineList:
				case PrimitiveType.LineStrip: return 2;
				case PrimitiveType.TriangleList:
				case PrimitiveType.TriangleStrip:
				case PrimitiveType.TriangleFan: return 3;
				default: throw new ArgumentException ();
			}
		}

		public ITexture2D CreateTexture2D ( int width, int height )
		{
			return new Texture2D ( this, width, height );
		}

		public ITexture2D CreateTexture2D ( Contents.ImageInfo imageInfo, Color? colorKey = null )
		{
			return new Texture2D ( this, imageInfo, colorKey );
		}

		public IRenderBuffer CreateRenderBuffer ( int width, int height )
		{
			throw new NotImplementedException ();
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, int vertexCount ) where T : struct
		{
			return new VertexBuffer<T> ( this, fvf, vertexCount );
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, T [] vertices ) where T : struct
		{
			return new VertexBuffer<T> ( this, fvf, vertices );
		}

		public IIndexBuffer CreateIndexBuffer ( int indexCount )
		{
			return new IndexBuffer ( this, indexCount );
		}

		public IIndexBuffer CreateIndexBuffer ( int [] indices )
		{
			return new IndexBuffer ( this, indices );
		}

		public IShader CreateShader ( Stream stream, ShaderType shaderType )
		{
			return new Shader ( this, stream, shaderType );
		}

		public IShader CreateShader ( string code, ShaderType shaderType )
		{
			return new Shader ( this, code, shaderType );
		}

		public IEffect CreateEffect ( params IShader [] shaders )
		{
			return new Effect ( this, shaders );
		}
	}
}

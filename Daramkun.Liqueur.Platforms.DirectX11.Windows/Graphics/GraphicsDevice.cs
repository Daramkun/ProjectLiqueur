using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Graphics
{
	class GraphicsDevice : IGraphicsDevice
	{
		SharpDX.Direct3D11.Device d3dDevice;
		SharpDX.Direct3D11.DeviceContext d3dContext;
		SharpDX.Direct3D11.RenderTargetView d3dRenderTarget;
		SharpDX.Direct3D11.DepthStencilView d3dDepthStencil;
		SharpDX.DXGI.SwapChain dxgiSwapChain;
		SharpDX.DXGI.Output dxgiOutput;

		public object Handle
		{
			get { throw new NotImplementedException (); }
		}

		public BaseRenderer BaseRenderer { get { return BaseRenderer.DirectX; } }

		public Version RendererVersion { get { return new Version ( 11, 0 ); } }

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> list = new List<Vector2> ();
				foreach ( var v in dxgiOutput.GetDisplayModeList ( SharpDX.DXGI.Format.R8G8B8A8_UNorm,
					SharpDX.DXGI.DisplayModeEnumerationFlags.Scaling ) )
					list.Add ( new Vector2 ( v.Width, v.Height ) );
				return list.ToArray ();
			}
		}

		public Vector2 ScreenSize
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

		public bool FullscreenMode
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

		public bool VerticalSyncMode
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

		public Viewport Viewport
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
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public bool BlendState
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

		public GraphicsDevice ( IWindow window )
		{
			SharpDX.Direct3D11.Device.CreateWithSwapChain ( SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.None,
				new SharpDX.DXGI.SwapChainDescription ()
				{
					BufferCount = 1,
					IsWindowed = true,
					ModeDescription = new SharpDX.DXGI.ModeDescription ( 800, 600, new SharpDX.DXGI.Rational ( 60, 1 ),
						SharpDX.DXGI.Format.R8G8B8A8_UNorm ),
					OutputHandle = ( window.Handle as Form ).Handle,
					SwapEffect = SharpDX.DXGI.SwapEffect.Discard,
					Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
					Flags = SharpDX.DXGI.SwapChainFlags.None,
					SampleDescription = new SharpDX.DXGI.SampleDescription ( 1, 0 ),
				}, out d3dDevice, out dxgiSwapChain );
			d3dContext = d3dDevice.ImmediateContext;

			SharpDX.DXGI.Factory factory = dxgiSwapChain.GetParent<SharpDX.DXGI.Factory> ();
			factory.MakeWindowAssociation ( ( window.Handle as Form ).Handle, SharpDX.DXGI.WindowAssociationFlags.Valid );
			dxgiOutput = factory.GetAdapter ( 0 ).GetOutput ( 0 );
		}

		public void BeginScene ()
		{
			
		}

		public void EndScene ()
		{

		}

		public void Clear ( ClearBuffer clearBuffer, Color color )
		{
			if ( clearBuffer.HasFlag ( ClearBuffer.ColorBuffer ) )
				d3dContext.ClearRenderTargetView ( d3dRenderTarget, new SharpDX.Color4 ( color.RedScalar,
					color.GreenScalar, color.BlueScalar, color.AlphaScalar ) );
			if ( clearBuffer.HasFlag ( ClearBuffer.DepthBuffer ) || clearBuffer.HasFlag ( ClearBuffer.StencilBuffer ) )
				d3dContext.ClearDepthStencilView ( d3dDepthStencil,
					( clearBuffer.HasFlag ( ClearBuffer.DepthBuffer ) ? SharpDX.Direct3D11.DepthStencilClearFlags.Depth : 0 ) |
					( clearBuffer.HasFlag ( ClearBuffer.StencilBuffer ) ? SharpDX.Direct3D11.DepthStencilClearFlags.Stencil : 0 ),
					0, 0 );
		}

		public void SwapBuffer ()
		{
			dxgiSwapChain.Present ( 0, SharpDX.DXGI.PresentFlags.None );
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

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
}

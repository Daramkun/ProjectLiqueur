using System;
using System.Collections.Generic;
using System.IO;
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
		SharpDX.Direct3D11.Texture2D d3dDepthStencilBuffer;
		SharpDX.Direct3D11.DepthStencilState d3dDepthStencilState;
		SharpDX.DXGI.SwapChain dxgiSwapChain;
		SharpDX.DXGI.Output dxgiOutput;

		public object Handle { get { return d3dDevice; } }

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
			dxgiOutput = factory.GetAdapter ( 0 ).Outputs [ 0 ];

			d3dRenderTarget = new SharpDX.Direct3D11.RenderTargetView ( d3dDevice, dxgiSwapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D> ( 0 ) );
			
			d3dDepthStencilBuffer = new SharpDX.Direct3D11.Texture2D ( d3dDevice, new SharpDX.Direct3D11.Texture2DDescription ()
			{
				Width = 800,
				Height = 600,
				MipLevels = 1,
				ArraySize = 1,
				Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
				SampleDescription = new SharpDX.DXGI.SampleDescription ( 1, 0 ),
				Usage = SharpDX.Direct3D11.ResourceUsage.Default,
				BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil,
				CpuAccessFlags = 0,
			} );
			d3dDepthStencil = new SharpDX.Direct3D11.DepthStencilView ( d3dDevice, d3dDepthStencilBuffer, 
				new SharpDX.Direct3D11.DepthStencilViewDescription ()
				{
					Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
					Dimension = SharpDX.Direct3D11.DepthStencilViewDimension.Texture2D,
					Texture2D = new SharpDX.Direct3D11.DepthStencilViewDescription.Texture2DResource () { MipSlice = 0 },
				}
			);
			d3dContext.OutputMerger.SetRenderTargets ( d3dDepthStencil, d3dRenderTarget );
		}

		~GraphicsDevice ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				if ( d3dDepthStencilState != null )
					d3dDepthStencilState.Dispose ();
				d3dDepthStencilBuffer.Dispose ();
				d3dDepthStencil.Dispose ();
				d3dRenderTarget.Dispose ();
				d3dDevice.Dispose ();
				dxgiSwapChain.Dispose ();
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
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
			throw new NotImplementedException ();
		}
	}
}

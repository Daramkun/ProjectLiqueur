using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Graphics
{
	public class Renderer : IRenderer
	{
		Window window;

		SharpDX.Direct3D11.Device1 d3dDevice;
		SharpDX.Direct3D11.DeviceContext1 d3dDeviceContext;

		SharpDX.DXGI.SwapChain dxgiSwapChain;

		SharpDX.Direct2D1.DeviceContext d2dDeviceContext;
		SharpDX.Direct2D1.Bitmap1 d2dBitmap;

		public Transform2 GlobalTransform { get; set; }
		public Transform2 LocalTransform { get; set; }

		public Vector2 [] AvailableScreenSize
		{
			get { throw new NotImplementedException (); }
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

		public bool FullscreenMode { get { return true; } set { } }

		public Renderer ( Window window )
		{
			this.window = window;

			SharpDX.Direct3D11.Device tempDevice = new SharpDX.Direct3D11.Device (
				SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport );
			d3dDevice = tempDevice.QueryInterface<SharpDX.Direct3D11.Device1> ();
			d3dDeviceContext = d3dDevice.ImmediateContext.QueryInterface<SharpDX.Direct3D11.DeviceContext1> ();

			SharpDX.DXGI.Device2 dxgiDevice2 = d3dDevice.QueryInterface<SharpDX.DXGI.Device2> ();
			SharpDX.DXGI.Adapter dxgiAdapter = dxgiDevice2.Adapter;
			SharpDX.DXGI.Factory2 dxgiFactory = dxgiAdapter.GetParent<SharpDX.DXGI.Factory2> ();

			SharpDX.DXGI.SwapChainDescription1 swapChainDesc = new SharpDX.DXGI.SwapChainDescription1 ()
			{
				Width = 800,
				Height = 600,
				Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
				Stereo = false,
				SwapEffect = SharpDX.DXGI.SwapEffect.FlipSequential,
				Scaling = SharpDX.DXGI.Scaling.Stretch,
				BufferCount = 2,
				Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
				SampleDescription = new SharpDX.DXGI.SampleDescription ( 1, 0 ),
			};

			dxgiSwapChain = dxgiFactory.CreateSwapChainForCoreWindow ( d3dDevice,
				new SharpDX.ComObject ( window.coreWindow ), ref swapChainDesc, null );

			SharpDX.Direct2D1.Device d2dDevice = new SharpDX.Direct2D1.Device ( dxgiDevice2 );
			d2dDeviceContext = new SharpDX.Direct2D1.DeviceContext ( d2dDevice, SharpDX.Direct2D1.DeviceContextOptions.None );

			SharpDX.Direct2D1.BitmapProperties1 bitmapProp = new SharpDX.Direct2D1.BitmapProperties1 ()
			{
				PixelFormat = new SharpDX.Direct2D1.PixelFormat ( SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied ),
				DpiX = Windows.Graphics.Display.DisplayProperties.LogicalDpi,
				DpiY = Windows.Graphics.Display.DisplayProperties.LogicalDpi,
				BitmapOptions = SharpDX.Direct2D1.BitmapOptions.Target | SharpDX.Direct2D1.BitmapOptions.CannotDraw,
			};

			SharpDX.DXGI.Surface dxgiSurface = dxgiSwapChain.GetBackBuffer<SharpDX.DXGI.Surface> ( 0 );
			d2dBitmap = new SharpDX.Direct2D1.Bitmap1 ( d2dDeviceContext, dxgiSurface, bitmapProp );

			d2dDeviceContext.Target = d2dBitmap;
		}

		public void Begin ()
		{
			d2dDeviceContext.BeginDraw ();
		}

		public void End ()
		{
			d2dDeviceContext.EndDraw ();
		}

		public void Clear ( Color color )
		{
			d2dDeviceContext.Clear ( new SharpDX.Color4 ( color.RedScalar,
				color.GreenScalar, color.BlueScalar, color.AlphaScalar ) );
		}

		public void Present ()
		{
			dxgiSwapChain.Present ( 1, SharpDX.DXGI.PresentFlags.None );
		}

		public void DrawImage ( IImage bitmap, Color overlay )
		{
			DrawImage ( bitmap, overlay, new Rectangle ( new Vector2 (), bitmap.Size ) );
		}

		public void DrawImage ( IImage bitmap, Color overlay, Rectangle sourceRectangle )
		{

		}

		public void DrawFont ( IFont font, string text, Color color )
		{
			throw new NotImplementedException ();
		}

		public void DrawFont ( IFont font, string text, Color color, Vector2 area )
		{
			throw new NotImplementedException ();
		}

		public Vector2 MeasureString ( IFont font, string text )
		{
			Vector2 area = new Vector2 ();
			float largestY = 0;

			foreach ( char ch in text )
			{
				if ( ch == '\n' )
				{
					area.Y += largestY;
					largestY = 0;
				}

				IImage fontChar;
				if ( font [ ch ] == null ) fontChar = font [ ' ' ];
				else fontChar = font [ ch ];
				area.X += fontChar.Width;
				largestY = fontChar.Height;
			}
			area.Y += largestY;

			return area;
		}

		public int MeasureString ( IFont font, string text, Vector2 area )
		{
			Vector2 carea = new Vector2 ();
			float largestY = 0;

			int i;
			for ( i = 0; i < text.Length; i++ )
			{
				char ch = text [ i ];
				if ( ch == '\n' || carea.X >= area.X )
				{
					carea.Y += largestY;
					largestY = 0;
					if ( carea.Y >= area.Y )
						break;
				}

				IImage fontChar;
				if ( font [ ch ] == null ) fontChar = font [ ' ' ];
				else fontChar = font [ ch ];
				carea.X += fontChar.Width;
				largestY = fontChar.Height;
			}
			carea.Y += largestY;
			return i;
		}
	}
}

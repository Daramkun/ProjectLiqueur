using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Platforms;
using Windows.UI.Core;

namespace Daramkun.Liqueur.Graphics
{
	class Renderer : IRenderer, IDisposable
	{
		Window window;
		Vector2 screenSize;

		internal SharpDX.Direct3D11.Device1 d3dDevice;
		internal SharpDX.Direct3D11.DeviceContext1 d3dDeviceContext;

		SharpDX.Direct3D11.RenderTargetView renderTarget;

		SharpDX.DXGI.Adapter dxgiAdapter;
		SharpDX.DXGI.SwapChain dxgiSwapChain;

		internal SharpDX.Direct2D1.DeviceContext d2dDeviceContext;
		SharpDX.Direct2D1.Bitmap1 d2dBitmap;

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> screenSizes = new List<Vector2> ();
				foreach ( var output in dxgiAdapter.Outputs )
				{
					foreach ( var format in Enum.GetValues ( typeof ( SharpDX.DXGI.Format ) ) )
					{
						var displayModes = output.GetDisplayModeList ( ( SharpDX.DXGI.Format ) format,
							SharpDX.DXGI.DisplayModeEnumerationFlags.Interlaced | SharpDX.DXGI.DisplayModeEnumerationFlags.Scaling );
						foreach ( var displayMode in displayModes )
							if ( displayMode.Scaling == SharpDX.DXGI.DisplayModeScaling.Unspecified )
								screenSizes.Add ( new Vector2 ( displayMode.Width, displayMode.Height ) );
					}
				}
				return screenSizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return screenSize; }
			set
			{
				screenSize = value;
				dxgiSwapChain.ResizeBuffers ( 0, ( int ) value.X, ( int ) value.Y,
					SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.DXGI.SwapChainFlags.DisplayOnly );
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
		}

		internal void CreateInstance ()
		{
			SharpDX.Direct3D11.Device tempDevice = new SharpDX.Direct3D11.Device (
				SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport );
			d3dDevice = tempDevice.QueryInterface<SharpDX.Direct3D11.Device1> ();
			d3dDeviceContext = d3dDevice.ImmediateContext.QueryInterface<SharpDX.Direct3D11.DeviceContext1> ();

			SharpDX.DXGI.Device2 dxgiDevice2 = d3dDevice.QueryInterface<SharpDX.DXGI.Device2> ();
			dxgiAdapter = dxgiDevice2.Adapter;
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
				new SharpDX.ComObject ( window.Handle as CoreWindow ), ref swapChainDesc, null );
			SharpDX.Direct3D11.Texture2D surfaceTexture = dxgiSwapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D> ( 0 );

			renderTarget = new SharpDX.Direct3D11.RenderTargetView ( d3dDevice, surfaceTexture );
			d3dDeviceContext.OutputMerger.SetTargets ( renderTarget );

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

		public void Dispose ()
		{
			dxgiSwapChain.Dispose ();
			d2dBitmap.Dispose ();
			renderTarget.Dispose ();
			d3dDeviceContext.Dispose ();
			d2dDeviceContext.Dispose ();
			d3dDevice.Dispose ();
		}

		public void Clear ( Color color )
		{
			d2dDeviceContext.BeginDraw ();
			d2dDeviceContext.Clear ( new SharpDX.Color4 ( color.RedScalar, 
				color.GreenScalar, color.BlueScalar, color.AlphaScalar ) );
			d3dDeviceContext.ClearRenderTargetView ( renderTarget, new SharpDX.Color4 ( color.RedScalar,
				color.GreenScalar, color.BlueScalar, color.AlphaScalar ) );
		}

		public void Present ()
		{
			d2dDeviceContext.EndDraw ();
			dxgiSwapChain.Present ( 0, SharpDX.DXGI.PresentFlags.None );
		}


		public IImage CreateImage ( ImageData imageData, Color colorKey )
		{
			return new Image ( imageData, colorKey );
		}
	}
}

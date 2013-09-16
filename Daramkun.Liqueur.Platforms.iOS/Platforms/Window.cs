using System;
using Daramkun.Liqueur.Mathematics;
using MonoTouch.Foundation;
using System.Drawing;

namespace Daramkun.Liqueur.Platforms
{
	public class Window : IWindow
	{
		OpenTK.Platform.iPhoneOS.iPhoneOSGameView gameView;

		public string Title
		{
			get { return gameView.Title; }
			set { gameView.Title = value; }
		}

		public Vector2 ClientSize { get { return new Vector2 ( gameView.Frame.Width, gameView.Frame.Height ); } }

		public object Handle { get { return gameView; } }

		public Window ( RectangleF frame )
		{
			gameView = new OpenTK.Platform.iPhoneOS.iPhoneOSGameView ( frame );
		}

		~Window()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				gameView.Dispose ();
				gameView = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void DoEvents ()
		{
			NSRunLoop.Current.RunUntil(DateTime.Now.AddMilliseconds(2000));
		}
	}
}


using System;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Platforms
{
	public class Window : IWindow
	{
		OpenTK.Platform.Android.AndroidGameView gameView;

		public string Title
		{
			get { return gameView.Title; }
			set { gameView.Title = value; }
		}

		public Vector2 ClientSize { get { return new Vector2 ( gameView.ClientSize.Width, gameView.ClientSize.Height ); } }

		public object Handle { get { return gameView; } }

		public Window ( Android.Content.Context context )
		{
			gameView = new OpenTK.Platform.Android.AndroidGameView ( context );
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
			gameView.ProcessEvents ();
		}
	}
}


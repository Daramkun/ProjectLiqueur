using System;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur
{
	public class TouchPanel : TouchDevice
	{
		private class InternalTouchListener : Android.Views.View.IOnTouchListener
		{
			public bool OnTouch ( Android.Views.View v, Android.Views.MotionEvent e )
			{

			}

			public void Dispose ()
			{

			}

			public IntPtr Handle { get { return 0; } }
		}

		InternalTouchListener touchListener = new InternalTouchListener ();

		public TouchPanel ( IWindow window )
		{
			OpenTK.Platform.Android.AndroidGameView gameView = ( window.Handle as OpenTK.Platform.Android.AndroidGameView );
			gameView.SetOnTouchListener ( touchListener = new InternalTouchListener () );
		}

		protected override TouchState GenerateState ()
		{
			throw new NotImplementedException ();
		}

		public override int MaximumTouchCount
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool IsSupport
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool IsConnected
		{
			get
			{
				throw new NotImplementedException ();
			}
		}
	}
}


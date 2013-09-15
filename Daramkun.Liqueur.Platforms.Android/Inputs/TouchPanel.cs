using System;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs;
using System.Collections.Generic;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs
{
	public class TouchPanel : TouchDevice
	{
		private class InternalTouchListener : Android.Views.View.IOnTouchListener
		{
			public List<TouchPointer> touchPointers = new List<TouchPointer> ();

			public bool OnTouch ( Android.Views.View v, Android.Views.MotionEvent e )
			{
				Android.Views.MotionEventActions action = e.Action;

				if ( ( action & Android.Views.MotionEventActions.Mask ) != Android.Views.MotionEventActions.Move )
				{
					TouchPointer tec = new TouchPointer ();

					switch ( action & Android.Views.MotionEventActions.Mask )
					{
						case Android.Views.MotionEventActions.Down:
							{
								tec.Mode = PointerMode.Pressed;
								tec.Id = e.GetPointerId ( 0 );
								tec.Position = new Vector2 ( e.GetX (), e.GetY () );
							}
							break;
						case Android.Views.MotionEventActions.PointerDown:
							{
								tec.Mode = PointerMode.Pressed;
								tec.Id = ( action & Android.Views.MotionEventActions.PointerIdMask ) >> Android.Views.MotionEventActions.PointerIdShift;
								tec.Id = e.GetPointerId ( tec.Id );
								tec.Position = new Vector2 ( e.GetX (), e.GetY () );
							}
							break;
						case Android.Views.MotionEventActions.Up:
							{
								tec.Mode = PointerMode.Released;
								tec.Id = e.GetPointerId ( 0 );
								tec.Position = new Vector2 ( e.GetX (), e.GetY () );
							}
							break;
						case Android.Views.MotionEventActions.PointerUp:
							{
								tec.Mode = PointerMode.Released;
								tec.Id = ( action & Android.Views.MotionEventActions.PointerIdMask ) >> Android.Views.MotionEventActions.PointerIdShift;
								int id = e.GetPointerId ( tec.Id );
								tec.Position = new Vector2 ( e.GetX (), e.GetY () );
							}
							break;
					}

					bool isChanged = false;
					for ( int i = 0; i < touchPointers.Count; i++ )
					{
						if ( touchPointers [ i ].Id == tec.Id )
						{
							touchPointers [ i ] = tec;
							isChanged = true;
							break;
						}
					}

					if ( !isChanged )
						touchPointers.Add ( tec );
				}
				else if ( ( action & Android.Views.MotionEventActions.Mask ) == Android.Views.MotionEventActions.Move )
				{
					for ( int i = 0; i < e.PointerCount; i++ )
					{
						TouchPointer tec = new TouchPointer ();
						tec.Mode = PointerMode.Moved;
						tec.Id = e.GetPointerId ( i );
						tec.Position = new Vector2 ( e.GetX (), e.GetY () );
						touchPointers [ i ] = tec;
					}
				}

				return true;
			}

			public void Dispose () { }
			public IntPtr Handle { get { return 0; } }
		}

		InternalTouchListener touchListener;

		public TouchPanel ( IWindow window )
		{
			OpenTK.Platform.Android.AndroidGameView gameView = ( window.Handle as OpenTK.Platform.Android.AndroidGameView );
			gameView.SetOnTouchListener ( touchListener = new InternalTouchListener () );
		}

		protected override TouchState GenerateState ()
		{
			return new TouchState ( touchListener.touchPointers.ToArray () );
		}

		public override int MaximumTouchCount
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }
	}
}


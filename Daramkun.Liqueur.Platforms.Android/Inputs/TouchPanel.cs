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
							tec = new TouchPointer ( new IntPtr ( e.GetPointerId ( 0 ) ), new Vector2 ( e.GetX (), e.GetY () ), PointerMode.Pressed );
							break;
						case Android.Views.MotionEventActions.PointerDown:
							tec = new TouchPointer ( new IntPtr ( e.GetPointerId ( ( int ) ( action & Android.Views.MotionEventActions.PointerIdMask ) >> ( int ) Android.Views.MotionEventActions.PointerIdShift ) ),
							                         new Vector2 ( e.GetX (), e.GetY () ), PointerMode.Pressed );
							break;
						case Android.Views.MotionEventActions.Up:
							tec = new TouchPointer ( new IntPtr ( e.GetPointerId ( 0 ) ), new Vector2 ( e.GetX (), e.GetY () ), PointerMode.Released );
							break;
						case Android.Views.MotionEventActions.PointerUp:
							tec = new TouchPointer ( new IntPtr ( e.GetPointerId ( ( int ) ( action & Android.Views.MotionEventActions.PointerIdMask ) >> ( int ) Android.Views.MotionEventActions.PointerIdShift ) ),
							                        new Vector2 ( e.GetX (), e.GetY () ), PointerMode.Released );
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
						TouchPointer tec = new TouchPointer ( new IntPtr ( e.GetPointerId ( i ) ), new Vector2 ( e.GetX (), e.GetY () ), PointerMode.Moved );
						touchPointers [ i ] = tec;
					}
				}

				return true;
			}

			public void Dispose () { }
			public IntPtr Handle { get { return new IntPtr ( 0 ); } }
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


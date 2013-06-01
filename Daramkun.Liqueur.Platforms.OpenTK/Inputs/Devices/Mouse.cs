using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;
using OpenTK;

namespace Daramkun.Liqueur.Inputs
{
	public class Mouse : RawMouse
	{
		IWindow window;

		public override bool IsConnected { get { return true; } }

		public Mouse ( IWindow window )
			: base ( window )
		{
			this.window = window;

			GameWindow otkWindow = window.Handle as GameWindow;

			otkWindow.Mouse.ButtonDown += ButtonDownEvent;
			otkWindow.Mouse.ButtonUp += ButtonUpEvent;
			otkWindow.Mouse.Move += MoveEvent;
			otkWindow.Mouse.WheelChanged += WheelEvent;
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				GameWindow otkWindow = window.Handle as GameWindow;

				otkWindow.Mouse.ButtonDown -= ButtonDownEvent;
				otkWindow.Mouse.ButtonUp -= ButtonUpEvent;
				otkWindow.Mouse.Move -= MoveEvent;
				otkWindow.Mouse.WheelChanged -= WheelEvent;
			}

			base.Dispose ( isDisposing );
		}

		private void ButtonDownEvent ( object sender, OpenTK.Input.MouseButtonEventArgs e )
		{
			if ( ( e.Button & OpenTK.Input.MouseButton.Left ) != 0 )
				MouseButton |= MouseButton.Left;
			if ( ( e.Button & OpenTK.Input.MouseButton.Right ) != 0 )
				MouseButton |= MouseButton.Right;
			if ( ( e.Button & OpenTK.Input.MouseButton.Middle ) != 0 )
				MouseButton |= MouseButton.Middle;
			Position = new Daramkun.Liqueur.Geometries.Vector2 ( e.X, e.Y );
		}

		private void ButtonUpEvent ( object sender, OpenTK.Input.MouseButtonEventArgs e )
		{
			if ( ( e.Button & OpenTK.Input.MouseButton.Left ) != 0 )
				MouseButton &= MouseButton.Left;
			if ( ( e.Button & OpenTK.Input.MouseButton.Right ) != 0 )
				MouseButton &= MouseButton.Right;
			if ( ( e.Button & OpenTK.Input.MouseButton.Middle ) != 0 )
				MouseButton &= MouseButton.Middle;
			Position = new Daramkun.Liqueur.Geometries.Vector2 ( e.X, e.Y );
		}

		private void MoveEvent ( object sender, OpenTK.Input.MouseMoveEventArgs e )
		{
			Position = new Daramkun.Liqueur.Geometries.Vector2 ( e.X, e.Y );
		}

		private void WheelEvent ( object sender, OpenTK.Input.MouseWheelEventArgs e )
		{
			Wheel = e.DeltaPrecise;
			Position = new Daramkun.Liqueur.Geometries.Vector2 ( e.X, e.Y );
		}
	}
}

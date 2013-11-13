using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs
{
	class Mouse : MouseDevice
	{
		IWindow window;

		protected MouseButton MouseButton = MouseButton.Unknown;
		protected Daramkun.Liqueur.Mathematics.Vector2 Position = new Daramkun.Liqueur.Mathematics.Vector2 ();
		protected float Wheel = 0;

		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }

		public Mouse ( IWindow window )
		{
			this.window = window;

			Form otkWindow = window.Handle as Form;

			otkWindow.MouseDown += ButtonDownEvent;
			otkWindow.MouseUp += ButtonUpEvent;
			otkWindow.MouseMove += MoveEvent;
			otkWindow.MouseWheel += WheelEvent;
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				Form otkWindow = window.Handle as Form;

				otkWindow.MouseDown -= ButtonDownEvent;
				otkWindow.MouseUp -= ButtonUpEvent;
				otkWindow.MouseMove -= MoveEvent;
				otkWindow.MouseWheel -= WheelEvent;
			}

			base.Dispose ( isDisposing );
		}

		private void ButtonDownEvent ( object sender, MouseEventArgs e )
		{
			if ( e.Button == MouseButtons.Left )
				MouseButton |= MouseButton.Left;
			if ( e.Button == MouseButtons.Right )
				MouseButton |= MouseButton.Right;
			if ( e.Button == MouseButtons.Middle )
				MouseButton |= MouseButton.Middle;
			Position = new Daramkun.Liqueur.Mathematics.Vector2 ( e.X, e.Y );
		}

		private void ButtonUpEvent ( object sender, MouseEventArgs e )
		{
			if ( e.Button == MouseButtons.Left )
				MouseButton &= MouseButton.Left;
			if ( e.Button == MouseButtons.Right )
				MouseButton &= MouseButton.Right;
			if ( e.Button == MouseButtons.Middle )
				MouseButton &= MouseButton.Middle;
			Position = new Daramkun.Liqueur.Mathematics.Vector2 ( e.X, e.Y );
		}

		private void MoveEvent ( object sender, MouseEventArgs e )
		{
			Position = new Daramkun.Liqueur.Mathematics.Vector2 ( e.X, e.Y );
		}

		private void WheelEvent ( object sender, MouseEventArgs e )
		{
			Wheel = e.Delta / 120.0f;
			Position = new Daramkun.Liqueur.Mathematics.Vector2 ( e.X, e.Y );
		}

		protected override MouseState GenerateState ()
		{
			return new MouseState ( Position, Wheel, MouseButton );
		}
	}
}

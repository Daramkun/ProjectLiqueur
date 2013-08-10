using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Platforms
{
	class Window : IWindow
	{
		OpenTK.GameWindow window;

		public string Title
		{
			get { return window.Title; }
			set { window.Title = value; }
		}

		public bool IsCursorVisible
		{
			get { return window.CursorVisible; }
			set { window.CursorVisible = value; }
		}

		public bool IsResizable
		{
			get { return window.WindowBorder == OpenTK.WindowBorder.Resizable; }
			set { window.WindowBorder = value ? OpenTK.WindowBorder.Resizable : OpenTK.WindowBorder.Fixed; }
		}

		public object Icon
		{
			get { return window.Icon; }
			set { window.Icon = value as System.Drawing.Icon; }
		}

		public Vector2 ClientSize
		{
			get { return new Vector2 ( window.ClientSize.Width, window.ClientSize.Height ); }
		}

		public object Handle
		{
			get { return window; }
		}

		public Window ()
		{
			window = new OpenTK.GameWindow ( 800, 600,
				new OpenTK.Graphics.GraphicsMode ( new OpenTK.Graphics.ColorFormat ( 32 ), 24, 8 ),
				"Project Liqueur", OpenTK.GameWindowFlags.Default, OpenTK.DisplayDevice.Default,
				4, 0, OpenTK.Graphics.GraphicsContextFlags.Default );
			window.ClientSize = new System.Drawing.Size ( 800, 600 );
			window.WindowBorder = OpenTK.WindowBorder.Fixed;
		}

		~Window ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				window.Dispose ();
				window = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void DoEvents ()
		{
			window.ProcessEvents ();
		}
	}
}

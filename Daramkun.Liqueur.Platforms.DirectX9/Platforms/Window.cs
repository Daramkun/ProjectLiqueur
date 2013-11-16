using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Platforms
{
	class Window : IWindow, IDesktopWindow
	{
		Form window;

		bool isCursorVisible = true;
		public bool IsCursorVisible
		{
			get { return isCursorVisible; }
			set { isCursorVisible = value; if ( isCursorVisible ) Cursor.Show (); else Cursor.Hide (); }
		}


		public bool IsResizable
		{
			get { return window.FormBorderStyle == FormBorderStyle.Sizable; }
			set { window.FormBorderStyle = ( value ) ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle; }
		}


		public object Icon { get { return window.Icon; } set { window.Icon = value as Icon; } }
		public string Title { get { return window.Text; } set { window.Text = value; } }
		public Vector2 ClientSize { get { return new Vector2 ( window.ClientSize.Width, window.ClientSize.Height ); } }
		public object Handle { get { return window; } }

		internal event EventHandler UpdateFrame, RenderFrame;

		public Window ()
		{
			window = new Form ();
			window.Text = "Project Liqueur";
			window.FormBorderStyle = FormBorderStyle.FixedSingle;
			window.MaximizeBox = false;
			window.Icon = System.Drawing.Icon.ExtractAssociatedIcon ( "C:\\Windows\\System32\\user32.dll" );
			window.ClientSize = new Size ( 800, 600 );
			window.StartPosition = FormStartPosition.CenterScreen;
		}

		public void Dispose ()
		{
			window.Dispose ();
		}

		public void DoEvents ()
		{
			Application.DoEvents ();
		}

		internal void Run ()
		{
			window.Show ();
			while ( window.IsHandleCreated && !( LiqueurSystem.GraphicsDevice.Handle as SharpDX.Direct3D9.Device ).IsDisposed )
			{
				DoEvents ();
				UpdateFrame ( this, null );
				RenderFrame ( this, null );
				Thread.Sleep ( 0 );
			}
		}
	}
}

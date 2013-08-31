using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			set { if ( isCursorVisible = value ) Cursor.Show (); else Cursor.Hide (); }
		}

		public bool IsResizable
		{
			get { return window.FormBorderStyle == FormBorderStyle.Sizable; }
			set { if ( value ) window.FormBorderStyle = FormBorderStyle.Sizable; else window.FormBorderStyle = FormBorderStyle.FixedSingle; }
		}

		public object Icon
		{
			get { return window.Icon; }
			set { window.Icon = value as System.Drawing.Icon; }
		}

		public string Title
		{
			get { return window.Text; }
			set { window.Text = value;  }
		}

		public Vector2 ClientSize
		{
			get { return new Vector2 ( window.ClientSize.Width, window.ClientSize.Height ); }
			set { window.ClientSize = new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y ); }
		}

		public object Handle { get { return window; } }

		public Window ()
		{
			window = new Form ();
			window.Text = "Project Liqueur";
			window.ClientSize = new System.Drawing.Size ( 800, 600 );
			window.FormBorderStyle = FormBorderStyle.FixedSingle;
			window.StartPosition = FormStartPosition.CenterScreen;
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
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void DoEvents ()
		{
			Application.DoEvents ();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;
using OpenTK;

namespace Daramkun.Liqueur.Platforms
{
    class Window : IWindow, IDisposable
    {
		internal OpenTK.GameWindow window;
		List<Action> dispatcherAction = new List<Action> ();

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
			set
			{ 
				if ( value ) window.WindowBorder = OpenTK.WindowBorder.Resizable;
				else window.WindowBorder = OpenTK.WindowBorder.Fixed;
			}
		}

		public object Handle { get { return window; } }

		public object Icon
		{
			get { return window.Icon; }
			set { window.Icon = value as System.Drawing.Icon; }
		}

		public Daramkun.Liqueur.Math.Vector2 ClientSize
		{
			get
			{
				return new Math.Vector2 ( window.ClientSize.Width, window.ClientSize.Height );
			}
		}

		internal Window ()
		{
			window = new OpenTK.GameWindow ( 800, 600,
				new OpenTK.Graphics.GraphicsMode ( new OpenTK.Graphics.ColorFormat ( 8, 8, 8, 8 ), 0, 0 ),
				"Project Liqueur", OpenTK.GameWindowFlags.Default,
				OpenTK.DisplayDevice.Default );
			window.ClientSize = new System.Drawing.Size ( 800, 600 );
		}

		public void Dispose ()
		{
			window.Dispose ();
		}

		public void DoEvents ()
		{
			window.ProcessEvents ();
		}

		public void FailFast ( string message, Exception exception )
		{
			Environment.FailFast ( message, exception );
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !XNA
using System.Net.NetworkInformation;
using System.Threading.Tasks;
#else
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endif
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Platforms
{
    class Window : IWindow, IDisposable
    {
#if OPENTK
		internal OpenTK.GameWindow window;
#elif XNA
		internal InternalGame game;
#if !WINDOWS_PHONE

		internal class InternalGame : Microsoft.Xna.Framework.Game
		{
			public event EventHandler UpdateLogic, DrawLogic;

			protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
			{
				UpdateLogic ();
				base.Update ( gameTime );
			}

			protected override void Draw ( Microsoft.Xna.Framework.GameTime gameTime )
			{
				DrawLogic ();
				base.Draw ( gameTime );
			}
		}
#else
		internal sealed class InternalGame : IDisposable
		{
			Microsoft.Xna.Framework.GameTimer gameTimer;

			public event EventHandler UpdateLogic, DrawLogic;

			public InternalGame ()
			{
				gameTimer = new Microsoft.Xna.Framework.GameTimer ();
				gameTimer.UpdateInterval = LiqueurSystem.FixedUpdateTimeStep;
				gameTimer.Update += ( object sender, Microsoft.Xna.Framework.GameTimerEventArgs e ) => { UpdateLogic ( this, EventArgs.Empty ); };
				gameTimer.Draw += ( object sender, Microsoft.Xna.Framework.GameTimerEventArgs e ) => { DrawLogic ( this, EventArgs.Empty ); };

				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode ( true );
			}

			public void Run ()
			{
				gameTimer.Start ();
			}

			public void Dispose ()
			{
				gameTimer.Stop ();
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode ( false );
			}
		}
#endif
#endif

		public string Title
		{
			get
			{
#if OPENTK
				return window.Title;
#elif XNA
#if WINDOWS_PHONE
				return "";
#endif
#endif
			}
			set
			{
#if OPENTK
				window.Title = value;
#elif XNA

#endif
			}
		}

		public Vector2 ClientSize
		{
			get
			{
#if OPENTK
				return new Vector2 ( window.ClientSize.Width, window.ClientSize.Height );
#elif XNA
#if WINDOWS_PHONE
				return new Vector2 ( Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth,
					Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight );
#endif
#endif
			}
			set
			{
#if OPENTK
				window.ClientSize = new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y );
#elif XNA
#if WINDOWS_PHONE
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = ( int ) value.X;
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = ( int ) value.Y;
				Microsoft.Xna.Framework.SharedGraphicsDeviceManager.Current.ApplyChanges ();
#endif
#endif
			}
		}

		public bool IsCursorVisible
		{
			get
			{
#if OPENTK
				return window.CursorVisible;
#elif XNA
#if WINDOWS_PHONE
				return false;
#endif
#endif
			}
			set
			{
#if OPENTK
				window.CursorVisible = value;
#elif XNA

#endif
			}
		}

		public bool IsResizable
		{
			get
			{
#if OPENTK
				return window.WindowBorder == OpenTK.WindowBorder.Resizable;
#elif XNA
				return false;
#endif
			}
			set
			{
#if OPENTK
				if ( value ) window.WindowBorder = OpenTK.WindowBorder.Resizable;
				else window.WindowBorder = OpenTK.WindowBorder.Fixed;
#elif XNA

#endif
			}
		}

		public object Handle
		{
			get
			{
#if OPENTK
				return window;
#elif XNA
				return game;
#endif
			}
		}

		public object Icon
		{
			get
			{
#if OPENTK
				return window.Icon;
#elif XNA
				return null;
#endif
			}
			set
			{
#if OPENTK
				window.Icon = value as System.Drawing.Icon;
#elif XNA
				
#endif
			}
		}

		internal Window ()
		{
#if OPENTK
			window = new OpenTK.GameWindow ( 800, 600,
				new OpenTK.Graphics.GraphicsMode ( new OpenTK.Graphics.ColorFormat ( 8, 8, 8, 8 ), 0, 0 ),
				"Project Liqueur", OpenTK.GameWindowFlags.Default,
				OpenTK.DisplayDevice.Default );
			window.ClientSize = new System.Drawing.Size ( 800, 600 );
#elif XNA
			game = new InternalGame ();
#endif
		}

		public void Dispose ()
		{
#if OPENTK
			window.Dispose ();
#elif XNA
			game.Dispose ();
#endif
		}

		public void DoEvent ()
		{
#if OPENTK
			window.ProcessEvents ();
#elif XNA
			
#endif
		}

		public void FailFast ( string message, Exception exception )
		{
#if !XNA
			Environment.FailFast ( message, exception );
#endif
		}
	}
}

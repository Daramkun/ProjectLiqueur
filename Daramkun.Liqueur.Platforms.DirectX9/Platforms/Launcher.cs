using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		List<object> invokedMethod = new List<object> ();
		Thread thisThread, updateThread;
		bool isMultithread;

		public bool IsInitialized { get; private set; }

		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					PlatformType = PlatformType.WindowsNT,
					PlatformVersion = Environment.OSVersion.Version,
					UserName = Environment.UserName,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString ()
				};
			}
		}

		public Launcher ( bool isMultithread = false )
		{
			IsInitialized = false;
			this.isMultithread = isMultithread;
		}

		public void LauncherInitialize ( out IWindow window, out Graphics.IGraphicsDevice graphicsDevice, out Audio.IAudioDevice audioDevice )
		{
			window = new Window ();
			graphicsDevice = new GraphicsDevice ( window );
			audioDevice = null;
		}

		public void LauncherRun ( LauncherArgument args )
		{
			thisThread = Thread.CurrentThread;

			Form window = LiqueurSystem.Window.Handle as Form;

			window.Load += ( object sender, EventArgs e ) =>
			{
				LiqueurSystem.Window.Title = "Project Liqueur";
				args.Initialize ();
			};
			window.Activated += ( object sender, EventArgs e ) => { args.Activated (); };
			window.Deactivate += ( object sender, EventArgs e ) => { args.Deactivated (); };
			( LiqueurSystem.Window as Window ).UpdateFrame += ( object sender, EventArgs e ) =>
			{
				if ( invokedMethod.Count > 0 )
				{
					foreach ( Action action in invokedMethod.ToArray () )
					{
						action ();
						lock ( invokedMethod ) { invokedMethod.Remove ( action ); }
					}
				}
				if ( !isMultithread )
					args.UpdateLogic ();
			};
			window.Resize += ( object sender, EventArgs e ) => { args.Resize (); };
			( LiqueurSystem.Window as Window ).RenderFrame += ( object sender, EventArgs e ) =>
			{
				args.DrawLogic ();
			};
			if ( isMultithread )
			{
				updateThread = new Thread ( () =>
				{
					while ( window.IsHandleCreated )
					{
						args.UpdateLogic ();
						Thread.Sleep ( 0 );
					}
				} );
				updateThread.Start ();
			}

			( LiqueurSystem.Window as Window ).Run ();
		}

		public void LauncherFinalize ( IWindow window, Graphics.IGraphicsDevice graphicsDevice, Audio.IAudioDevice audioDevice )
		{
			if ( updateThread != null )
				updateThread.Abort ();
			updateThread = null;
			if ( audioDevice != null )
				audioDevice.Dispose ();
			graphicsDevice.Dispose ();
			window.Dispose ();
		}

		public void InvokeInMainThread ( Action action, bool waitForEndOfMethod = true )
		{
			if ( thisThread == Thread.CurrentThread ) { action (); return; }
			lock ( invokedMethod ) { invokedMethod.Add ( action ); }
			while ( waitForEndOfMethod && invokedMethod.Contains ( action ) ) Thread.Sleep ( 1 );
		}
	}
}

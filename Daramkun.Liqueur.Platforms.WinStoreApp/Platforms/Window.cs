using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Platforms
{
	class Window : IWindow, IFrameworkView, IDisposable
    {
		internal CoreWindow window;

		internal Action UpdateLogic, DrawLogic;

		public string Title
		{
			get { return ""; }
			set { }
		}

		public bool IsCursorVisible
		{
			get { return false; }
			set { }
		}

		public bool IsResizable
		{
			get { return false; }
			set { }
		}

		public object Handle
		{
			get { return window; }
		}

		public object Icon
		{
			get { return null; }
			set { }
		}

		internal Window ()
		{

		}

		public void Dispose ()
		{
			
		}

		public void DoEvents ()
		{
			window.Dispatcher.ProcessEvents ( CoreProcessEventsOption.ProcessAllIfPresent );
		}

		public void FailFast ( string message, Exception exception )
		{
			Environment.FailFast ( message, exception );
		}

		public Action<CoreWindow> InitializeWindow;

		#region IFrameworkView 구현
		public void Initialize ( CoreApplicationView applicationView )
		{

		}

		public void Load ( string entryPoint )
		{
			( LiqueurSystem.Renderer as Renderer ).CreateInstance ();
			InitializeWindow ( window );
		}

		public void Run ()
		{
			window.Activate ();

			while ( true )
			{
				DoEvents ();
				UpdateLogic ();
				DrawLogic ();
			}
		}

		public void SetWindow ( CoreWindow window )
		{
			this.window = window;
		}

		public void Uninitialize ()
		{

		}
		#endregion
	}
}
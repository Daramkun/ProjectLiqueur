using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Scenes;
using Windows.ApplicationModel.Core;

namespace Daramkun.Liqueur
{
    public static class Game
	{
		public static Window Window { get; private set; }
		public static Renderer Renderer { get; internal set; }

		static Scene currentScene;
		public static Scene CurrentScene
		{
			get { return currentScene; }
			set
			{
				if ( currentScene != null ) currentScene.OnFinalize ();
				currentScene = value;
				if ( currentScene != null ) currentScene.OnInitialize ();
			}
		}

		static GameTime updateGameTime = new GameTime (),
						drawGameTime = new GameTime ();

		public static EventHandler Initialize, Finalize;
		public static EventHandler<GameTimeEventArgs> Update, Draw;

		static Game ()
		{
			Window = new Window ();

			Window.UpdateFrame += ( object sender, EventArgs e ) =>
			{
				updateGameTime.Update ();
				if ( CurrentScene != null )
					CurrentScene.OnUpdate ( updateGameTime );
				if ( Update != null )
					Update ( null, new GameTimeEventArgs ( updateGameTime ) );
			};

			Window.RenderFrame += ( object sender, EventArgs e ) =>
			{
				drawGameTime.Update ();
				if ( CurrentScene != null )
					CurrentScene.OnDraw ( drawGameTime );
				if ( Draw != null )
					Draw ( null, new GameTimeEventArgs ( drawGameTime ) );
			};
		}

		private class FrameworkViewSource : IFrameworkViewSource
		{
			public IFrameworkView CreateView ()
			{
				return Window;
			}
		}

		public static void Run ()
		{
			if ( Initialize != null )
				Initialize ( null, EventArgs.Empty );
			CoreApplication.Run ( new FrameworkViewSource () );
			if ( Finalize != null )
				Finalize ( null, EventArgs.Empty );
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Scenes
{
	public sealed class FrameScene : Scene
	{
		BaseSceneTransitor sceneTransitor = null;
		Scene currentScene = null, nextScene = null;

		public Scene CurrentScene { get { return currentScene; } }

		internal FrameScene ( Scene scene ) { currentScene = scene; }

		public void TransitionStart ( BaseSceneTransitor transitor, Scene scene )
		{
			sceneTransitor = transitor;
			nextScene = scene;
		}

		public override void OnInitialize ()
		{
			if ( currentScene != null )
				currentScene.OnInitialize ();
			base.OnInitialize ();
		}

		public override void OnFinalize ()
		{
			if ( currentScene != null )
				currentScene.OnFinalize ();
			base.OnFinalize ();
		}

		internal void Transitioning ()
		{
			if ( currentScene != null )
				currentScene.OnFinalize ();
			currentScene = nextScene;
			if ( currentScene != null )
				currentScene.OnInitialize ();
		}

		public override void OnUpdate ( GameTime gameTime )
		{
			if ( sceneTransitor != null )
			{
				if ( sceneTransitor.OnTransitUpdate ( this, gameTime ) )
				{
					sceneTransitor = null;
					nextScene = null;
				}
				else
				{
					if ( sceneTransitor.IsTransitParallelFrameUpdate )
					{
						base.OnUpdate ( gameTime );
						return;
					}
				}
			}

			if ( currentScene != null )
				currentScene.OnUpdate ( gameTime );

			base.OnUpdate ( gameTime );
		}

		public override void OnDraw ( GameTime gameTime )
		{
			if ( currentScene != null )
				currentScene.OnDraw ( gameTime );

			if ( sceneTransitor != null )
				sceneTransitor.OnTransitDraw ( this, gameTime );

			base.OnDraw ( gameTime );
		}

		public override void OnActivated ()
		{
			if ( currentScene != null )
				currentScene.OnActivated ();
			base.OnActivated ();
		}

		public override void OnDeactivated ()
		{
			if ( currentScene != null )
				currentScene.OnDeactivated ();
			base.OnDeactivated ();
		}

		public override void OnResize ()
		{
			if ( currentScene != null )
				currentScene.OnResize ();
			base.OnResize ();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Scenes
{
	public abstract class BaseSceneTransitor
	{
		public bool IsTransitParallelFrameUpdate { get; set; }

		public BaseSceneTransitor ()
		{
			IsTransitParallelFrameUpdate = false;
		}

		public abstract bool OnTransitUpdate ( FrameScene frameScene, GameTime gameTime );
		public virtual void OnTransitDraw ( FrameScene frameScene, GameTime gameTime )
		{
		
		}

		protected void Transitioning ( FrameScene frameScene )
		{
			frameScene.Transitioning ();
		}
	}
}

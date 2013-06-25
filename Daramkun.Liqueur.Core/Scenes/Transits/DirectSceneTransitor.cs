using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Scenes.Transits
{
	public class DirectSceneTransitor : BaseSceneTransitor
	{
		public override bool OnTransitUpdate ( FrameScene frameScene, GameTime gameTime )
		{
			Transitioning ( frameScene );
			return true;
		}
	}
}

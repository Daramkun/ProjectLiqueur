using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Nodes.Scenes
{
	public class DirectTransitor : ISceneTransitor
	{
		public TransitionState Transitioning ( TransitionState currentState, Node scene )
		{
			if ( currentState == TransitionState.Begin )
				return TransitionState.PretransitionEnd;
			else if ( currentState == TransitionState.PretransitionEnd )
				return TransitionState.End;
			else throw new ArgumentException ();
		}
	}
}

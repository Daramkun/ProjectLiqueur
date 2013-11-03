using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Nodes.Scenes
{
	public enum TransitionState
	{
		None,
		Begin,
		Pretransition,
		PretransitionEnd,
		Posttransition,
		End,
	}

	public interface ISceneTransitor
	{
		TransitionState Transitioning ( TransitionState currentState, Node scene );
	}
}

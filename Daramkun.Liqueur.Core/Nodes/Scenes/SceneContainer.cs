using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Nodes.Scenes
{
	public enum SceneContainMethod
	{
		Flat = 0,
		Stack = 1,
	}

	public class SceneContainer : Node
	{
		Stack<Node> sceneList;
		Node nextNode;

		public ISceneTransitor SceneTransitor { get; set; }
		public TransitionState TransitionState { get; set; }
		public SceneContainMethod ContainMethod { get; set; }

		public SceneContainer ( Node firstScene )
		{
			sceneList = new Stack<Node> ();
			firstScene.Parent = this;
			sceneList.Push ( firstScene );

			ContainMethod = SceneContainMethod.Flat;
			SceneTransitor = new DirectTransitor ();
			TransitionState = TransitionState.None;
		}

		public void Transition ( Node node )
		{
			if ( TransitionState != TransitionState.None ) return;
			nextNode = node;
			TransitionState = TransitionState.Begin;
		}

		public void PreviousTransition ()
		{
			if ( TransitionState != TransitionState.None ) return;
			if ( ContainMethod != SceneContainMethod.Stack ) return;
			if ( sceneList.Count == 1 ) return;
			nextNode = null;
			TransitionState = TransitionState.Begin;
		}

		public override void Intro ( params object [] args )
		{
			sceneList.Peek ().Intro ();
			base.Intro ( args );
		}

		public override void Outro ()
		{
			while ( sceneList.Count > 0 )
				sceneList.Pop ().Outro ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( sceneList.Count > 0 )
				sceneList.Peek ().Update ( gameTime );
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			if ( TransitionState != TransitionState.None )
			{
				TransitionState = SceneTransitor.Transitioning ( TransitionState, sceneList.Peek () );
				if ( TransitionState == Scenes.TransitionState.PretransitionEnd )
				{
					switch ( ContainMethod )
					{
						case SceneContainMethod.Flat:
							sceneList.Pop ().Outro ();
							nextNode.Intro ();
							nextNode.Parent = this;
							sceneList.Push ( nextNode );
							break;
						case SceneContainMethod.Stack:
							if ( nextNode == null )
							{
								Node nn = sceneList.Pop ();
								nn.Parent = null;
								nn.Outro ();
							}
							else
							{
								nextNode.Intro ();
								sceneList.Push ( nextNode );
							}
							break;
					}
				}
				else if ( TransitionState == Scenes.TransitionState.End )
				{
					TransitionState = TransitionState.None;
					nextNode = null;
				}
			}
			else
				if ( sceneList.Count > 0 )
					sceneList.Peek ().Draw ( gameTime );
			base.Draw ( gameTime );
		}
	}
}

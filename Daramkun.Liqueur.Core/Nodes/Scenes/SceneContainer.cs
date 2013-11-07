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
		Node currentNode;
		Node nextNode;
		object [] nextNodeArgs;

		SpinLock spinlock;

		public ISceneTransitor SceneTransitor { get; set; }
		public TransitionState TransitionState { get; set; }
		public SceneContainMethod ContainMethod { get; set; }

		public SceneContainer ( Node firstScene )
		{
			spinlock = new SpinLock ();

			sceneList = new Stack<Node> ();
			firstScene.Parent = this;
			sceneList.Push ( currentNode = firstScene );

			ContainMethod = SceneContainMethod.Flat;
			SceneTransitor = new DirectTransitor ();
			TransitionState = TransitionState.None;
		}

		public void Transition ( Node node, params object [] args )
		{
			if ( TransitionState != TransitionState.None ) return;
			nextNode = node;
			nextNodeArgs = args;
			TransitionState = TransitionState.Begin;
		}

		public void PreviousTransition ()
		{
			if ( TransitionState != TransitionState.None ) return;
			if ( ContainMethod != SceneContainMethod.Stack ) return;
			if ( sceneList.Count == 1 ) return;
			nextNode = null;
			nextNodeArgs = null;
			TransitionState = TransitionState.Begin;
		}

		public override void Intro ( params object [] args )
		{
			currentNode.Intro ();
			base.Intro ( args );
		}

		public override void Outro ()
		{
			spinlock.Enter ();
			while ( sceneList.Count > 0 )
				sceneList.Pop ().Outro ();
			spinlock.Exit ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			spinlock.Enter ();
			Node currentNode = this.currentNode;
			spinlock.Exit ();
			if ( currentNode != null )
				currentNode.Update ( gameTime );
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			if ( currentNode != null )
				currentNode.Draw ( gameTime );

			if ( TransitionState != TransitionState.None )
			{
				TransitionState = SceneTransitor.Transitioning ( TransitionState, sceneList.Peek () );
				if ( TransitionState == Scenes.TransitionState.PretransitionEnd )
				{
					switch ( ContainMethod )
					{
						case SceneContainMethod.Flat:
							spinlock.Enter ();
							sceneList.Pop ().Outro ();
							nextNode.Intro ( nextNodeArgs );
							nextNode.Parent = this;
							sceneList.Push ( currentNode = nextNode );
							nextNode = null;
							nextNodeArgs = null;
							spinlock.Exit ();
							break;
						case SceneContainMethod.Stack:
							if ( nextNode == null )
							{
								spinlock.Enter ();
								Node nn = sceneList.Pop ();
								nn.Parent = null;
								nn.Outro ();
								currentNode = null;
								nextNode = null;
								nextNodeArgs = null;
								spinlock.Exit ();
							}
							else
							{
								spinlock.Enter ();
								nextNode.Intro ( nextNodeArgs );
								sceneList.Push ( currentNode = nextNode );
								nextNode = null;
								nextNodeArgs = null;
								spinlock.Exit ();
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
			base.Draw ( gameTime );
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Nodes
{
	public class Node
	{
		List<Node> children;
		uint zOrder;

		GameTimeEventArgs updateGameTimeEventArgs = null, drawGameTimeEventArgs = null;

		public Node Parent { get; private set; }
		public IEnumerable<Node> Children { get { return children; } }

		public uint ZOrder
		{
			get { return zOrder; }
			set
			{
				zOrder = value;
				Parent.children.Sort ();
			}
		}

		public bool IsEnabled { get; set; }
		public bool IsVisible { get; set; }

		public T Instantiate<T> ( params object [] args ) where T : Node
		{
			T instantiate = Activator.CreateInstance ( typeof ( T ), args ) as T;
			Add ( instantiate );
			return instantiate;
		}

		public void StartCoroutine ( Func<IEnumerable> coroutine )
		{
			foreach ( IEnumerable e in coroutine () )
				LiqueurSystem.Window.DoEvents ();
		}

		public void StartCoroutine ( Func<object, IEnumerable> coroutine, object argument )
		{
			foreach ( IEnumerable e in coroutine ( argument ) )
				LiqueurSystem.Window.DoEvents ();
		}

		public Node Add ( Node node, params object [] args )
		{
			children.Add ( node );
			children.Sort ();
			node.Parent = this;
			node.Intro ( args );
			return node;
		}

		public void Remove ( Node node )
		{
			node.Outro ();
			node.Parent = null;
			children.Remove ( node );
		}

		public Node ()
		{
			children = new List<Node> ();
			IsEnabled = IsVisible = true;
		}

		public event EventHandler IntroEvent, OutroEvent;
		public event EventHandler<GameTimeEventArgs> UpdateEvent, DrawEvent;

		public virtual void Intro ( params object [] args )
		{
			if ( IntroEvent != null )
				IntroEvent ( this, EventArgs.Empty );
		}

		public virtual void Outro ()
		{
			if ( OutroEvent != null )
				OutroEvent ( this, EventArgs.Empty );
			foreach ( Node node in children.ToArray () )
				Remove ( node );
			children.Clear ();
		}

		public virtual void Update ( GameTime gameTime )
		{
			if(updateGameTimeEventArgs == null)
				updateGameTimeEventArgs = new GameTimeEventArgs () { GameTime = gameTime };

			if ( UpdateEvent != null )
				UpdateEvent ( this, updateGameTimeEventArgs );

			if ( children.Count > 0 )
			{
				LiqueurSystem.UpdateLooper.Run ( children.ToArray (), ( Node item ) =>
				{
					if ( item.IsEnabled )
						item.Update ( gameTime );
				} );
			}
		}

		public virtual void Draw ( GameTime gameTime )
		{
			if ( drawGameTimeEventArgs == null )
				drawGameTimeEventArgs = new GameTimeEventArgs () { GameTime = gameTime };

			if ( DrawEvent != null )
				DrawEvent ( this, drawGameTimeEventArgs );

			if ( children.Count > 0 )
			{
				foreach ( Node node in children.ToArray () )
				{
					if ( node.IsVisible )
						node.Draw ( gameTime );
				}
			}
		}

		public int CompareTo ( Node other )
		{
			return -ZOrder.CompareTo ( other.ZOrder );
		}
	}
}

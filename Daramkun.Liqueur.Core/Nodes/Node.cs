using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Nodes
{
	public class Node
	{
		object forLock = new object ();
		List<Node> children;
		uint zOrder;

		GameTimeEventArgs updateGameTimeEventArgs = null, drawGameTimeEventArgs = null;

		public Node Parent { get; internal set; }
		public IEnumerable<Node> Children { get { return children; } }
		public int ChildrenCount { get { return children.Count; } }
		//public static IForEach UpdateLooper { get; set; }

		public virtual uint ZOrder
		{
			get { return zOrder; }
			set
			{
				zOrder = value;
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
			if ( node == null ) return null;
			lock ( forLock )
			{
				children.Add ( node );
				node.Parent = this;
				node.Intro ( args );
			}
			return node;
		}

		public void Remove ( Node node )
		{
			lock ( forLock )
			{
				node.Outro ();
				node.Parent = null;
				children.Remove ( node );
			}
		}

		public Node this [ int index ] { get { lock ( forLock ) { return children [ index ]; } } }

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
				Node [] arr;
				lock ( forLock ) arr = children.ToArray ();
				var arrEnum = from a in arr where a.IsEnabled select a;
				//if ( UpdateLooper == null )
				//{
					foreach ( Node item in arrEnum )
						item.Update ( gameTime );
				/*}
				else
				{
					UpdateLooper.Run ( arrEnum, ( object item ) =>
					{
						( item as Node ).Update ( gameTime );
					} );
				}*/
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
				Node [] arr;
				lock ( forLock ) arr = children.ToArray ();
				foreach ( Node node in from a in arr where a.IsVisible orderby a.ZOrder select a )
				{
					node.Draw ( gameTime );
				}
			}
		}
	}
}

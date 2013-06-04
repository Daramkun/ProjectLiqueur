using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Nodes
{
	public class Node : IComparable<Node>
	{
		List<Node> children;
		uint zOrder;

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
			AddChild ( instantiate );
			return instantiate;
		}

		public void StartCoroutine ( Func<IEnumerable> coroutine )
		{
			foreach ( IEnumerable e in coroutine () )
				LiqueurSystem.Window.DoEvent ();
		}

		public void StartCoroutine ( Func<object, IEnumerable> coroutine, object argument )
		{
			foreach ( IEnumerable e in coroutine ( argument ) )
				LiqueurSystem.Window.DoEvent ();
		}

		public Node AddChild ( Node node )
		{
			children.Add ( node );
			children.Sort ();
			node.Parent = this;
			node.OnInitialize ();
			return node;
		}

		public void RemoveChild ( Node node )
		{
			node.OnFinalize ();
			node.Parent = null;
			children.Remove ( node );
		}

		public Node ()
		{
			children = new List<Node> ();
			IsEnabled = IsVisible = true;
		}

		public event EventHandler Initialize, Finalize;
		public event EventHandler<GameTimeEventArgs> Update, Draw;

		public virtual void OnInitialize ()
		{
			if ( Initialize != null )
				Initialize ( this, EventArgs.Empty );
		}

		public virtual void OnFinalize ()
		{
			if ( Finalize != null )
				Finalize ( this, EventArgs.Empty );
			Node [] nodes = children.ToArray ();
			foreach ( Node node in nodes )
				RemoveChild ( node );
			children.Clear ();
		}

		public virtual void OnUpdate ( GameTime gameTime )
		{
			if ( Update != null )
				Update ( this, new GameTimeEventArgs ( gameTime ) );

			Node [] nodes = children.ToArray ();
			foreach ( Node node in nodes )
				if ( node.IsEnabled )
					node.OnUpdate ( gameTime );
		}

		public virtual void OnDraw ( GameTime gameTime )
		{
			if ( Draw != null )
				Draw ( this, new GameTimeEventArgs ( gameTime ) );

			Node [] nodes = children.ToArray ();
			foreach ( Node node in nodes )
				if ( node.IsVisible )
					node.OnDraw ( gameTime );
		}

		public int CompareTo ( Node other )
		{
			return -ZOrder.CompareTo ( other.ZOrder );
		}
	}
}

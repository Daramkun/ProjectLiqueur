using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Nodes;

namespace Daramkun.Liqueur.Scenes
{
	public abstract class Scene : Node
	{
		public event EventHandler Activated, Deactivated, Resize;

		public string SceneName { get; protected set; }

		public virtual void OnActivated ()
		{
			if ( Activated != null )
				Activated ( this, EventArgs.Empty );
		}

		public virtual void OnDeactivated ()
		{
			if ( Deactivated != null )
				Deactivated ( this, EventArgs.Empty );
		}

		public virtual void OnResize ()
		{
			if ( Resize != null )
				Resize ( this, EventArgs.Empty );
		}
	}
}

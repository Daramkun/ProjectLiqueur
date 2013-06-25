using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Nodes;
using Daramkun.Walnut.Nodes;

namespace Daramkun.Walnut.Scripts
{
	public abstract class EventCallee
	{
		public virtual void onInitialize () { }
		public virtual void onFinalize () { }
		public virtual void onUpdate ( Node sender, float gameTime ) { }
		public virtual void onDraw ( Node sender, float gameTime ) { }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Nodes;

namespace Daramkun.Liqueur.Marionette.Controls
{
	public class Control : IDisposable
	{
		List<Control> children = new List<Control> ();

		public Control Parent { get; private set; }
		public IEnumerable<Control> Children { get { return children; } }

		~Control ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
		
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}

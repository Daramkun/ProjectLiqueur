using System;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs;
using System.Collections.Generic;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs
{
	public class TouchPanel : TouchDevice
	{
		public TouchPanel ( IWindow window )
		{

		}

		protected override TouchState GenerateState ()
		{
			return new TouchState (  );
		}

		public override int MaximumTouchCount
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }
	}
}


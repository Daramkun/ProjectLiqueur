using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs
{
	public struct TouchPointer
	{
		public IntPtr Id { get; private set; }
		public Vector2 Position { get; private set; }
		public PointerMode Mode { get; private set; }

		public TouchPointer ( IntPtr id, Vector2 position, PointerMode mode )
			: this ()
		{
			Id = id;
			Position = position;
			Mode = mode;
		}
	}
}

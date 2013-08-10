using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public enum BlendParameter
	{
		Zero = 0,
		One,

		SourceColor,
		InvertSourceColor,
		SourceAlpha,
		InvertSourceAlpha,

		DestinationAlpha,
		InvertDestinationAlpha,
		DestinationColor,
		InvertDestinationColor,
	}

	public struct BlendOperation
	{
		public BlendParameter SourceParameter { get; set; }
		public BlendParameter DestinationParameter { get; set; }
	}
}

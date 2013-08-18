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

	public enum BlendOperator
	{
		Add,
		Subtract,
		ReverseSubtract,
		Minimum,
		Maximum,
	}

	public struct BlendOperation
	{
		public static BlendOperation None { get { return new BlendOperation ( BlendOperator.Add, BlendParameter.One, BlendParameter.Zero ); } }
		public static BlendOperation AlphaBlend { get { return new BlendOperation ( BlendOperator.Add, BlendParameter.SourceAlpha, BlendParameter.InvertSourceAlpha ); } }
		public static BlendOperation AdditiveBlend { get { return new BlendOperation ( BlendOperator.Add, BlendParameter.SourceAlpha, BlendParameter.One ); } }
		public static BlendOperation SubtractBlend { get { return new BlendOperation ( BlendOperator.ReverseSubtract, BlendParameter.SourceAlpha, BlendParameter.One ); } }
		public static BlendOperation MultiplyBlend { get { return new BlendOperation ( BlendOperator.Add, BlendParameter.DestinationColor, BlendParameter.Zero ); } }

		public BlendParameter SourceParameter { get; set; }
		public BlendParameter DestinationParameter { get; set; }
		public BlendOperator Operator { get; set; }

		public BlendOperation ( BlendOperator op, BlendParameter sourceParam, BlendParameter destParam )
			: this ()
		{
			Operator = op;
			SourceParameter = sourceParam;
			DestinationParameter = destParam;
		}
	}
}

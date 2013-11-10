using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public struct Color
	{
		public float RedScalar { get; set; }
		public float GreenScalar { get; set; }
		public float BlueScalar { get; set; }
		public float AlphaScalar { get; set; }
		
		public byte AlphaValue { get { return ( byte ) ( AlphaScalar * 255 ); } set { AlphaScalar = value / 255.0f; } }
		public byte RedValue { get { return ( byte ) ( RedScalar * 255 ); } set { RedScalar = value / 255.0f; } }
		public byte GreenValue { get { return ( byte ) ( GreenScalar * 255 ); } set { GreenScalar = value / 255.0f; } }
		public byte BlueValue { get { return ( byte ) ( BlueScalar * 255 ); } set { BlueScalar = value / 255.0f; } }
		/*
		public byte RedValue { get; set; }
		public byte GreenValue { get; set; }
		public byte BlueValue { get; set; }
		public byte AlphaValue { get; set; }

		public float AlphaScalar { get { return AlphaValue / 255.0f; } set { AlphaValue = ( byte ) ( value * 255 ); } }
		public float RedScalar { get { return RedValue / 255.0f; } set { RedValue = ( byte ) ( value * 255 ); } }
		public float GreenScalar { get { return GreenValue / 255.0f; } set { GreenValue = ( byte ) ( value * 255 ); } }
		public float BlueScalar { get { return BlueValue / 255.0f; } set { BlueValue = ( byte ) ( value * 255 ); } }
		*/
		public int ColorValue { get { return ( ( ( int ) RedValue ) << 24 ) + ( ( ( int ) GreenValue ) << 16 ) + ( ( ( int ) BlueValue ) << 8 ) + AlphaValue; } }
		public uint ARGBColorValue { get { return ( ( ( uint ) AlphaValue ) << 24 ) + ( ( ( uint ) RedValue ) << 16 ) + ( ( ( uint ) GreenValue ) << 8 ) + BlueValue; } }

		public Color ( byte red, byte green, byte blue )
			: this ( red, green, blue, 255 )
		{ }

		public Color ( float red, float green, float blue )
			: this ( red, green, blue, 1 )
		{ }

		public Color ( Color sourceColor, byte alpha )
			: this ( sourceColor.RedScalar, sourceColor.GreenScalar, sourceColor.BlueScalar, alpha / 255.0f )
		{ }

		public Color ( Color sourceColor, float alpha )
			: this ( sourceColor.RedScalar, sourceColor.GreenScalar, sourceColor.BlueScalar, alpha )
		{ }

		public Color ( float red, float green, float blue, float alpha )
			: this ()
		{
			RedScalar = red;
			GreenScalar = green;
			BlueScalar = blue;
			AlphaScalar = alpha;
		}

		public Color ( byte red, byte green, byte blue, byte alpha )
			: this ()
		{
			RedValue = red;
			GreenValue = green;
			BlueValue = blue;
			AlphaValue = alpha;
		}

		public Color ( uint argbColorValue )
			: this ()
		{
			RedValue = ( byte ) ( ( argbColorValue >> 16 ) & 0xff );
			GreenValue = ( byte ) ( ( argbColorValue >> 8 ) & 0xff );
			BlueValue = ( byte ) ( ( argbColorValue >> 0 ) & 0xff );
			AlphaValue = ( byte ) ( ( argbColorValue >> 24 ) & 0xff );
		}

		public override int GetHashCode ()
		{
			return ColorValue;
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Color ) ) return false;
			Color color = ( Color ) obj;
			return AlphaValue == color.AlphaValue && RedValue == color.RedValue &&
				GreenValue == color.GreenValue && BlueValue == color.BlueValue;
		}

		public static bool operator == ( Color v1, Color v2 )
		{
			return v1.Equals ( v2 );
		}

		public static bool operator != ( Color v1, Color v2 )
		{
			return !v1.Equals ( v2 );
		}

		public override string ToString ()
		{
			return String.Format ( "Red:{0}, Green:{1}, Blue:{2}, Alpha:{3}",
				RedValue, GreenValue, BlueValue, AlphaValue );
		}

		public static Color White { get { return new Color ( 1.0f, 1.0f, 1.0f ); } }
		public static Color Black { get { return new Color ( 0, 0, 0 ); } }

		public static Color Red { get { return new Color ( 1.0f, 0, 0 ); } }
		public static Color Green { get { return new Color ( 0, 1.0f, 0 ); } }
		public static Color Blue { get { return new Color ( 0, 0, 1.0f ); } }

		public static Color Cyan { get { return new Color ( 0, 1.0f, 1.0f ); } }
		public static Color Magenta { get { return new Color ( 1.0f, 0, 1.0f ); } }
		public static Color Yellow { get { return new Color ( 1.0f, 1.0f, 0 ); } }

		public static Color Transparent { get { return new Color ( 0, 0, 0, 0 ); } }
	}
}

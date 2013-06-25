using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public enum ColorArrange
	{
		ARGB,
		RGBA,
		BGRA,
	}

	public struct Color
	{
		public static ColorArrange DefaultArrange { get; set; }

		int colorValue;

		private int AlphaShift { get { return ( DefaultArrange == ColorArrange.ARGB ) ? 24 : ( DefaultArrange == ColorArrange.RGBA ) ? 0 : 0; } }
		private int RedShift { get { return ( DefaultArrange == ColorArrange.ARGB ) ? 16 : ( DefaultArrange == ColorArrange.RGBA ) ? 24 : 8; } }
		private int GreenShift { get { return ( DefaultArrange == ColorArrange.ARGB ) ? 8 : ( DefaultArrange == ColorArrange.RGBA ) ? 16 : 16; } }
		private int BlueShift { get { return ( DefaultArrange == ColorArrange.ARGB ) ? 0 : ( DefaultArrange == ColorArrange.RGBA ) ? 8 : 24; } }

		public int ColorValue { get { return colorValue; } set { colorValue = value; } }

		public byte AlphaValue
		{
			get { return ( byte ) ( ( colorValue >> AlphaShift ) & 0xff ); }
			set { colorValue &= ~( 0xff << AlphaShift ); colorValue |= value << AlphaShift; }
		}
		public byte RedValue
		{
			get { return ( byte ) ( ( colorValue >> RedShift ) & 0xff ); }
			set { colorValue &= ~( 0xff << RedShift ); colorValue |= value << RedShift; }
		}
		public byte GreenValue
		{
			get { return ( byte ) ( ( colorValue >> GreenShift ) & 0xff ); }
			set { colorValue &= ~( 0xff << GreenShift ); colorValue |= value << GreenShift; }
		}
		public byte BlueValue
		{
			get { return ( byte ) ( ( colorValue >> BlueShift ) & 0xff ); }
			set { colorValue &= ~( 0xff << BlueShift ); colorValue |= value << BlueShift; }
		}

		public float AlphaScalar
		{
			get { return AlphaValue / 255.0f; }
			set { AlphaValue = ( byte ) ( value * 255 ); }
		}
		public float RedScalar
		{
			get { return RedValue / 255.0f; }
			set { RedValue = ( byte ) ( value * 255 ); }
		}
		public float GreenScalar
		{
			get { return GreenValue / 255.0f; }
			set { GreenValue = ( byte ) ( value * 255 ); }
		}
		public float BlueScalar
		{
			get { return BlueValue / 255.0f; }
			set { BlueValue = ( byte ) ( value * 255 ); }
		}

		public Color ( byte red, byte green, byte blue )
		{
			colorValue = 0;

			RedValue = red;
			GreenValue = green;
			BlueValue = blue;
			AlphaValue = 255;
		}

		public Color ( byte red, byte green, byte blue, byte alpha )
		{
			colorValue = 0;

			RedValue = red;
			GreenValue = green;
			BlueValue = blue;
			AlphaValue = alpha;
		}

		public Color ( float red, float green, float blue )
		{
			colorValue = 0;

			RedScalar = red;
			GreenScalar = green;
			BlueScalar = blue;
			AlphaScalar = 1.0f;
		}

		public Color ( float red, float green, float blue, float alpha )
		{
			colorValue = 0;

			RedScalar = red;
			GreenScalar = green;
			BlueScalar = blue;
			AlphaScalar = alpha;
		}

		public Color ( Color sourceColor, byte alpha )
		{
			colorValue = sourceColor.ColorValue;
			AlphaValue = alpha;
		}

		public Color ( Color sourceColor, float alpha )
		{
			colorValue = sourceColor.ColorValue;
			AlphaScalar = alpha;
		}

		public override int GetHashCode ()
		{
			return colorValue;
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

		public static Color White { get { return new Color ( 255, 255, 255 ); } }
		public static Color Black { get { return new Color ( 0, 0, 0 ); } }

		public static Color Red { get { return new Color ( 255, 0, 0 ); } }
		public static Color Green { get { return new Color ( 0, 255, 0 ); } }
		public static Color Blue { get { return new Color ( 0, 0, 255 ); } }

		public static Color Cyan { get { return new Color ( 0, 255, 255 ); } }
		public static Color Magenta { get { return new Color ( 255, 0, 255 ); } }
		public static Color Yellow { get { return new Color ( 255, 255, 0 ); } }

		public static Color Transparent { get { return new Color ( 0, 0, 0, 0 ); } }
	}
}

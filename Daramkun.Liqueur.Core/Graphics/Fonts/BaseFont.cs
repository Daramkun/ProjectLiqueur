using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;

namespace Daramkun.Liqueur.Graphics.Fonts
{
	public abstract class BaseFont : IFont
	{
		public string FontFamily { get; protected set; }
		public float FontSize { get; protected set; }

		~BaseFont ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
		
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}

		protected abstract IImage this [ char ch ] { get; }

		public void DrawFont ( string text, Color color, Vector2 position )
		{
			DrawFont ( text, color, position, MeasureString ( text ) );
		}

		public void DrawFont ( string text, Color color, Vector2 position, Vector2 area )
		{
			List<Vector2> lines = new List<Vector2> ();
			int i = 0;
			float height = 0;
			for ( i = 0; i < text.Length; i++ )
			{
				char ch = text [ i ];
				if ( ch == '\n' )
				{
					lines.Add ( new Vector2 () );
					continue;
				}
				IImage image = this [ ch ];
				if ( image == null ) image = this [ '?' ];

				if ( lines.Count == 0 || lines [ lines.Count - 1 ].X + image.Width > area.X )
					if ( height + image.Height > area.Y ) return;
					else lines.Add ( new Vector2 () );

				lines [ lines.Count - 1 ] += new Vector2 ( image.Width, 0 );
				if ( lines [ lines.Count - 1 ].Y == 0 )
				{
					lines [ lines.Count - 1 ] += new Vector2 ( 0, image.Height );
					height += image.Height;
				}

				image.DrawBitmap ( color, new Transform2 ( position + 
					new Vector2 ( lines [ lines.Count - 1 ].X - image.Width, height - image.Height ) ) );
			}
		}

		public Vector2 MeasureString ( string text )
		{
			List<Vector2> lines = new List<Vector2> ();
			
			foreach ( char ch in text )
			{
				if ( ch == '\n' )
				{
					lines.Add ( new Vector2 () );
					continue;
				}
				IImage image = this [ ch ];
				if ( image == null ) image = this [ '?' ];
				
				if ( lines.Count == 0 )
					lines.Add ( new Vector2 () );

				lines [ lines.Count - 1 ] += new Vector2 ( image.Width, 0 );
				if ( lines [ lines.Count - 1 ].Y == 0 )
					lines [ lines.Count - 1 ] += new Vector2 ( 0, image.Height );
			}

			Vector2 measure = new Vector2 ( 0, 0 );
			foreach ( Vector2 v in lines )
			{
				measure.X = ( float ) Math.Max ( measure.X, v.X );
				measure.Y += lines [ lines.Count - 1 ].Y;
			}

			return measure;
		}

		public int MeasureString ( string text, Vector2 area )
		{
			List<Vector2> lines = new List<Vector2> ();
			int i = 0;
			float height = 0;
			for ( i = 0; i < text.Length; i++ )
			{
				char ch = text [ i ];
				if ( ch == '\n' )
				{
					lines.Add ( new Vector2 () );
					continue;
				}
				IImage image = this [ ch ];
				if ( image == null ) image = this [ '?' ];

				if ( lines.Count == 0 || lines [ lines.Count - 1 ].X + image.Width > area.X )
					if ( height + image.Height > area.Y ) return i;
					else lines.Add ( new Vector2 () );

				lines [ lines.Count - 1 ] += new Vector2 ( image.Width, 0 );
				if ( lines [ lines.Count - 1 ].Y == 0 )
				{
					lines [ lines.Count - 1 ] += new Vector2 ( 0, image.Height );
					height += image.Height;
				}
			}

			return i;
		}
	}
}

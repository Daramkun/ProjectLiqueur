using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Graphics.Fonts;
using Daramkun.Liqueur.Nodes;
using Daramkun.Walnut.Contents;

namespace Daramkun.Walnut.Nodes
{
	public class Sprite : WalnutNode
	{
		Transform2 transform;

		public virtual IImage Image { get; set; }
		public Rectangle? SourceRectangle { get; set; }
		public Color OverlayColor { get; set; }

		public float Opacity
		{
			get { return OverlayColor.AlphaScalar; }
			set { OverlayColor = new Color ( OverlayColor, value ); }
		}

		public virtual Vector2 Position
		{
			get { return transform.Translate; }
			set { transform.Translate = value; }
		}

		public virtual Vector2 Size
		{
			get { return transform.Scale * Image.Size; }
			set { transform.Scale = value / Image.Size; }
		}

		public virtual Vector2 Center
		{
			get { return transform.RotationCenter; }
			set { transform.RotationCenter = transform.ScaleCenter = value; }
		}

		public virtual float Rotation
		{
			get { return transform.Rotation; }
			set { transform.Rotation = value; }
		}

		public Transform2 Transform
		{
			get { return transform; }
			set { transform = value; }
		}

		public ObjectOffset ObjectOffset { get; set; }

		public Sprite ( IImage image )
			: base ()
		{
			Image = image;
			transform = Transform2.Identity;
			OverlayColor = Color.White;
		}

		public Sprite ( ContentManager contentManager, string filename )
			: base ()
		{
			Image = contentManager.Load<IImage> ( filename );
			transform = Transform2.Identity;
			OverlayColor = Color.White;
		}

		public Sprite ( ContentManager contentManager, string filename, Color colorKey )
			: base ()
		{
			Image = contentManager.Load<IImage> ( filename, colorKey );
			transform = Transform2.Identity;
			OverlayColor = Color.White;
		}

		private Vector2 GetVector2 ( JsonArray jsonArray )
		{
			return new Vector2 ( ( float ) jsonArray [ 0 ].Data, ( float ) jsonArray [ 1 ].Data );
		}

		private Color GetColor ( JsonArray jsonArray )
		{
			return new Color ( ( byte ) jsonArray [ 0 ].Data, ( byte ) jsonArray [ 1 ].Data,
				( byte ) jsonArray [ 3 ].Data, ( byte ) jsonArray [ 4 ].Data );
		}

		private Rectangle GetRectangle ( JsonArray jsonArray )
		{
			return new Rectangle ( ( float ) jsonArray [ 0 ].Data, ( float ) jsonArray [ 1 ].Data,
				( float ) jsonArray [ 2 ].Data, ( float ) jsonArray [ 3 ].Data );
		}

		public Sprite ( JsonEntry jsonEntry, ContentManager contentManager )
			: base ( jsonEntry, contentManager )
		{
			Image = contentManager.Load<IImage> ( jsonEntry [ "resource" ].Data as string );
			Position = GetVector2 ( jsonEntry [ "position" ].Data as JsonArray );
			Size = GetVector2 ( jsonEntry [ "size" ].Data as JsonArray );
			Center = GetVector2 ( jsonEntry [ "center" ].Data as JsonArray );
			Rotation = ( float ) jsonEntry [ "rotation" ].Data;
			ObjectOffset = ( ObjectOffset ) jsonEntry [ "objectoffset" ].Data;

			if ( jsonEntry.Contains ( "overlay" ) )
				OverlayColor = GetColor ( jsonEntry [ "overlay" ].Data as JsonArray );
			if ( jsonEntry.Contains ( "sourcerectangle" ) )
				SourceRectangle = GetRectangle ( jsonEntry [ "sourcerectangle" ].Data as JsonArray );
		}

		public override void OnDraw ( GameTime gameTime )
		{
			Transform2 transform = this.transform;
			transform.Translate -= CalculatedAnchorPoint;
			Vector2 imageSize = ( SourceRectangle != null ) ? SourceRectangle.Value.Size : Image.Size;
			imageSize *= transform.Scale;
			if ( ( ObjectOffset & Nodes.ObjectOffset.Middle ) != 0 )
				transform.Translate -= new Vector2 ( imageSize.X / 2, 0 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Right ) != 0 )
				transform.Translate -= new Vector2 ( imageSize.X, 0 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Center ) != 0 )
				transform.Translate -= new Vector2 ( 0, imageSize.Y / 2 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Bottom ) != 0 )
				transform.Translate -= new Vector2 ( 0, imageSize.Y );
			
			if ( SourceRectangle == null )
				Image.DrawBitmap ( OverlayColor, transform );
			else
				Image.DrawBitmap ( OverlayColor, transform, SourceRectangle.Value );
			base.OnDraw ( gameTime );
		}
	}
}

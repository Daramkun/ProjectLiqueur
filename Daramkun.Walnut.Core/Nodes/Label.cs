using System;
using System.Collections.Generic;
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
using Daramkun.Walnut.Scripts;

namespace Daramkun.Walnut.Nodes
{
	public class Label : WalnutNode
	{
		public virtual IFont Font { get; set; }
		public virtual Vector2 Position { get; set; }
		public virtual Vector2? Area { get; set; }

		public virtual string Text { get; set; }
		public virtual Color ForeColor { get; set; }

		public ObjectOffset ObjectOffset { get; set; }

		public Label ( IFont font )
			: base ()
		{
			Font = font;
			ForeColor = Color.Black;
		}

		public Label ( ContentManager contentManager, string filename )
			: base ()
		{
			Font = contentManager.Load<BaseFont> ( filename );
			ForeColor = Color.Black;
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

		public Label ( JsonEntry jsonEntry, ContentManager contentManager )
			: base ( jsonEntry, contentManager )
		{
			Font = contentManager.Load<BaseFont> ( jsonEntry [ "resource" ].Data as string );
			Text = jsonEntry [ "text" ].Data as string;
			Position = GetVector2 ( jsonEntry [ "position" ].Data as JsonArray );
			ForeColor = GetColor ( jsonEntry [ "forecolor" ].Data as JsonArray );
			ObjectOffset = ( ObjectOffset ) jsonEntry [ "objectoffset" ].Data;
			
			if ( jsonEntry.Contains ( "area" ) )
			{
				Area = GetVector2 ( jsonEntry [ "area" ].Data as JsonArray );
			}
		}

		public override void OnDraw ( GameTime gameTime )
		{
			Vector2 area = ( Area == null ) ? Font.MeasureString ( Text ) : Area.Value;

			Vector2 position = Position;
			position -= CalculatedAnchorPoint;
			if ( ( ObjectOffset & Nodes.ObjectOffset.Middle ) != 0 )
				position -= new Vector2 ( area.X / 2, 0 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Right ) != 0 )
				position -= new Vector2 ( area.X, 0 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Center ) != 0 )
				position -= new Vector2 ( 0, area.Y / 2 );
			if ( ( ObjectOffset & Nodes.ObjectOffset.Bottom ) != 0 )
				position -= new Vector2 ( 0, area.Y );
			
			Font.DrawFont ( Text, ForeColor, position, area );
			base.OnDraw ( gameTime );
		}
	}
}

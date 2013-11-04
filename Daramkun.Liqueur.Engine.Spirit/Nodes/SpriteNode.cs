using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Daramkun.Liqueur.Spirit.Nodes
{
	public enum SpriteAlignment
	{
		Left = 0,
		Center = 1 << 0,
		Right = 1 << 1,

		Top = 0,
		Middle = 1 << 2,
		Bottom = 1 << 3,

		LeftTop = 0,
		LeftMiddle = Left | Middle,
		LeftBottom = Left | Bottom,

		CenterTop = Center,
		CenterMiddle = Center | Middle,
		CenterBottom = Center | Bottom,

		RightTop = Right,
		RightMiddle = Right | Middle,
		RightBottom = Right | Bottom,
	}

	public class SpriteNode : Node
	{
		Sprite sprite;
		World2 tempWorld;

		public Color OverlayColor { get { return sprite.OverlayColor; } set { sprite.OverlayColor = value; } }
		public Rectangle ClippingArea { get { return sprite.ClippingArea; } set { sprite.ClippingArea = value; } }
		public IEffect Effect { get { return sprite.Effect; } set { sprite.Effect = value; } }
		public World2 World { get; set; }
		public ITexture2D Texture { get { return sprite.Texture; } set { sprite.Reset ( value ); } }
		public SpriteAlignment Alignment { get; set; }

		public SpriteNode ( ITexture2D texture )
		{
			sprite = new Sprite ( texture );
			World = World2.Identity;
			tempWorld = World2.Identity;
			Alignment = SpriteAlignment.LeftTop;
		}

		public SpriteNode ( ImageInfo imageInfo, Color? colorKey = null )
			: this ( LiqueurSystem.GraphicsDevice.CreateTexture2D ( imageInfo, colorKey ) )
		{ }

		public override void Draw ( GameTime gameTime )
		{
			tempWorld.Translate = World.Translate;
			tempWorld.Scale = World.Scale;
			tempWorld.ScaleCenter = World.ScaleCenter;
			tempWorld.Rotation = World.Rotation;
			tempWorld.RotationCenter = World.RotationCenter;

			if ( ( Alignment & SpriteAlignment.Center ) != 0 )
				tempWorld.Translate += new Vector2 ( -sprite.ClippingArea.Size.X * tempWorld.Scale.X / 2, 0 );
			else if ( ( Alignment & SpriteAlignment.Right ) != 0 )
				tempWorld.Translate += new Vector2 ( -sprite.ClippingArea.Size.X * tempWorld.Scale.X, 0 );

			if ( ( Alignment & SpriteAlignment.Middle ) != 0 ) 
				tempWorld.Translate += new Vector2 ( 0, -sprite.ClippingArea.Size.Y * tempWorld.Scale.Y / 2 );
			else if ( ( Alignment & SpriteAlignment.Bottom ) != 0 )
				tempWorld.Translate += new Vector2 ( 0, -sprite.ClippingArea.Size.Y * tempWorld.Scale.Y );

			sprite.Draw ( tempWorld );

			base.Draw ( gameTime );
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Spirit.Nodes;

namespace Test.Game.PerformanceTester
{
	public class PerformanceSpriteNode : SpriteNode
	{
		static Random random = new Random ( Environment.TickCount );

		float rotateUnit;

		public PerformanceSpriteNode ( ITexture2D texture )
			: base ( texture )
		{
			rotateUnit = ( ( float ) ( random.NextDouble () - random.NextDouble () ) ) * 10.0f;
			World.RotationCenter = texture.Size / 2;
			World.Translate = new Vector2 ( random.Next ( 800 ), random.Next ( 600 ) );
		}

		public override void Update ( GameTime gameTime )
		{
			World.Rotation += rotateUnit * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
			base.Update ( gameTime );
		}
	}
}

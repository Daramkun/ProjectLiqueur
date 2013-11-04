using System;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Spirit.Nodes;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;

namespace Test.Game.Dodge
{
	public class Bullet : SpriteNode
	{
		float angle;

		public Bullet ( ITexture2D texture )
			: base ( texture )
		{
			World.RotationCenter = texture.Size / 2;
		}

		private void InitializeBullet()
		{

		}

		public override void Intro ( params object [] args )
		{
			InitializeBullet ();
			base.Intro ( args );
		}

		public override void Outro ()
		{
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			World.Translate += new Vector2 ( ( float ) Math.Cos ( angle ), ( float ) Math.Sin ( angle ) ) *
				( gameTime.ElapsedGameTime.Milliseconds / 5.0f );

			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			base.Draw ( gameTime );
		}
	}
}
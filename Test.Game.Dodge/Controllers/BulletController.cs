using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;

namespace Test.Game.Dodge.Controllers
{
	public class BulletController : Node
	{
		ITexture2D bulletImage;

		public override void Intro ( params object [] args )
		{
			bulletImage = ( Parent as GameScene ).Contents.Load<ITexture2D> ( "Test.Game.Dodge.Resources.bullet.bmp",
				Color.Magenta);
			for ( int i = 0; i < 256; i++ )
			{
				Add ( new Bullet ( bulletImage ) );
			}
			base.Intro ( args );
		}

		public override void Update ( GameTime gameTime )
		{
			base.Update ( gameTime );
		}
	}
}

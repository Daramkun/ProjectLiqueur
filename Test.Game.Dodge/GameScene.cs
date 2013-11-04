using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Nodes;
using Test.Game.Dodge.Controllers;

namespace Test.Game.Dodge
{
	public class GameScene : Node
	{
		public ContentManager Contents { get; private set; }

		public override void Intro ( params object [] args )
		{
			Contents = new ContentManager ( new ManifestFileSystem () );
			Contents.AddDefaultContentLoader ();

			Add ( new PlayerController () );
			Add ( new BulletController () );
			base.Intro ( args );
		}

		public override void Outro ()
		{
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			base.Draw ( gameTime );
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Platforms
{
	class Window : IWindow
	{
		Microsoft.Xna.Framework.Game game;

		public string Title
		{
			get { return game.Window.Title; }
			set { game.Window.Title = value; }
		}

		public Vector2 ClientSize { get { return new Vector2 ( game.Window.ClientBounds.Width, game.Window.ClientBounds.Height ); } }

		public object Handle { get { return game.Window; } }

		public Window ( Microsoft.Xna.Framework.Game game )
		{
			this.game = game;
		}

		public void Dispose ()
		{

		}

		public void DoEvents ()
		{
			
		}
	}
}

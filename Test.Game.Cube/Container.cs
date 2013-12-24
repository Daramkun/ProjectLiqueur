using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Nodes;

namespace Test.Game.Cube
{
	/*private struct CubeVertex
	{
	
	}*/

    public class Container : Node
    {
		//IVertexBuffer<CubeVertex> vertices;
		IIndexBuffer indices;

		public override void Intro ( params object [] args )
		{


			base.Intro ( args );
		}

		public override void Update ( GameTime gameTime )
		{
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			LiqueurSystem.GraphicsDevice.BeginScene ();
			
			base.Draw ( gameTime );
			LiqueurSystem.GraphicsDevice.EndScene ();
		}
    }
}

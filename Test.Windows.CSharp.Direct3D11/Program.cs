using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.Decoder.Audios;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Windows.CSharp.Direct3D11
{
	static class Program
	{
		public struct Vertex
		{
			Vector2 position;
			Vector2 texture;

			public Vertex ( Vector2 position, Vector2 texture )
			{
				this.position = position;
				this.texture = texture;
			}
		}

		class InternalScene : Node
		{
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
				LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Magenta );
				base.Draw ( gameTime );
			}

			public override void Outro ()
			{
				base.Outro ();
			}
		}

		[STAThread]
		static void Main ()
		{
			LiqueurSystem.Run ( new Launcher (), new InternalScene () );
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Game.PlaySound
{
    public class Container : Node
    {
		ContentManager contentManager;
		IAudio audio1, audio2;

		Font font;

		public Container ()
		{
			Texture2DContentLoader.AddDefaultDecoders ();
			AudioContentLoader.AddDefaultDecoders ();
			ContentManager.ContentLoaderAssemblies.Add ( Assembly.Load ( "Daramkun.Liqueur.Spirit" ) );
		}

		public override void Intro ( params object [] args )
		{
			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();

			audio1 = contentManager.Load<IAudio> ( "Test.Game.PlaySound.Resources.test1.ogg" );
			audio2 = contentManager.Load<IAudio> ( "Test.Game.PlaySound.Resources.test2.ogg" );

			font = contentManager.Load<TrueTypeFont> ( "Test.Game.PlaySound.Resources.GameFont.ttf", 20 );

			Add ( InputHelper.CreateInstance () );

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Q ) )
				LiqueurSystem.AudioDevice.Play ( audio1 );
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.A ) )
				LiqueurSystem.AudioDevice.Stop ( audio1 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.W ) )
				LiqueurSystem.AudioDevice.Play ( audio2 );
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.S ) )
				LiqueurSystem.AudioDevice.Stop ( audio2 );

			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			LiqueurSystem.GraphicsDevice.BeginScene ();

			font.DrawFont ( string.Format ( "Audio1: {0}/{1}\nAudio2: {2}/{3}",
				audio1.Position, audio1.Duration, audio2.Position, audio2.Duration ), Color.White, new Vector2 () );

			base.Draw ( gameTime );
			LiqueurSystem.GraphicsDevice.EndScene ();
		}
    }
}

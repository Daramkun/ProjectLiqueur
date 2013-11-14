using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Game.InfoViewer
{
    public class Container : Node
    {
		ContentManager contentManager;
		Font font;
		float offset;
		string infoText;

		public Container ()
		{
			Texture2DContentLoader.AddDefaultDecoders ();
			ContentManager.ContentLoaderAssemblies.Add ( Assembly.Load ( "Daramkun.Liqueur.Spirit" ) );
		}

		public override void Intro ( params object [] args )
		{
			LiqueurSystem.GraphicsDevice.BlendState = true;
			LiqueurSystem.GraphicsDevice.BlendOperation = BlendOperation.AlphaBlend;

			LiqueurSystem.Window.Title = "Information Viewer";

			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				font = contentManager.Load<TrueTypeFont> ( "Test.Game.InfoViewer.Resources.GameFont.ttf", 20 );
			} );

			Add ( InputHelper.CreateInstance () );

			infoText = string.Format (
@"Platform Type: {0}
Platform Version: {1}
Machine Unique ID: {2}
User Name: {3}

Base Renderer: {4}
Renderer Version: {5}
Maximum Anisotropic Level: {6}",
				LiqueurSystem.Launcher.PlatformInformation.PlatformType,
				LiqueurSystem.Launcher.PlatformInformation.PlatformVersion,
				LiqueurSystem.Launcher.PlatformInformation.MachineUniqueIdentifier,
				LiqueurSystem.Launcher.PlatformInformation.UserName,

				LiqueurSystem.GraphicsDevice.BaseRenderer,
				LiqueurSystem.GraphicsDevice.RendererVersion,
				LiqueurSystem.GraphicsDevice.MaximumAnisotropicLevel
			);

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Down ) )
				offset -= gameTime.ElapsedGameTime.Milliseconds * 0.1f;
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Up ) )
				offset += gameTime.ElapsedGameTime.Milliseconds * 0.1f;

			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			LiqueurSystem.GraphicsDevice.BeginScene ();
			font.DrawFont ( infoText, Color.White, new Vector2 ( 10, 10 + offset ) );
			base.Draw ( gameTime );
			LiqueurSystem.GraphicsDevice.EndScene ();
		}
    }
}

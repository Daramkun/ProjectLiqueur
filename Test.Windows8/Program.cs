using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Decoders.Sounds;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Graphics.Fonts;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.Devices;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Scenes;
using Daramkun.Walnut;
using Daramkun.Walnut.Nodes;
using Daramkun.Walnut.Scripts;

namespace Test.Windows8
{
	static class Program
	{
		class MyScene : Scene
		{
			BaseFont font;

			FpsCalculator fpsCalc;

			public MyScene ()
			{
			}

			public override void OnInitialize ()
			{
				LiqueurSystem.Window.Title = "Test Window";

				base.OnInitialize ();
			}

			public override void OnFinalize ()
			{
				base.OnFinalize ();
			}

			public override void OnUpdate ( GameTime gameTime )
			{
				base.OnUpdate ( gameTime );
			}

			public override void OnDraw ( GameTime gameTime )
			{
				LiqueurSystem.Renderer.Clear ( Color.Black );

				base.OnDraw ( gameTime );

				LiqueurSystem.Renderer.Present ();
			}
		}

		[MTAThread]
		static void Main ()
		{
			WalnutSystem.SetupDecoders ();
			WalnutSystem.SetupFixedLogicTimeStep ( TimeSpan.FromTicks ( 166666 ), TimeSpan.FromTicks ( 166666 ) );
			WalnutSystem.SetupInputDevices<Keyboard, Mouse, GamePad, TouchPanel, Accelerometer> ();
			WalnutSystem.Run<Launcher, NoneScriptEngine, MyScene> ( /*new LocalFileSystem ()*/null );
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Scenes;
using Daramkun.Walnut.Contents;
using Daramkun.Walnut.Contents.Loaders;
using Daramkun.Walnut.Scenes;
using Daramkun.Walnut.Scripts;

namespace Daramkun.Walnut
{
    public static class WalnutSystem
    {
		public static ContentManager MainContents { get; private set; }
		public static IScriptEngine ScriptEngine { get; private set; }

		public static WalnutPackage MainWalnutPackage { get; private set; }
		public static WalnutPackage SubWalnutPackage { get; private set; }

		private static bool IsSubtypeOf ( Type majorType, Type minorType )
		{
			if ( majorType == minorType || majorType.IsSubclassOf ( minorType ) )
				return true;
			else if ( minorType.IsInterface )
			{
				foreach ( Type type in majorType.GetInterfaces () )
					if ( type == minorType )
						return true;
			}
			return false;
		}

		public static void SetupDecoders ( params Type [] decoders )
		{
			foreach ( Type decoder in decoders )
			{
				if ( IsSubtypeOf ( decoder, typeof ( IImageDecoder ) ) )
					ImageDecoders.AddDecoder ( decoder );
				else if ( IsSubtypeOf ( decoder, typeof ( ISoundDecoder ) ) )
					SoundDecoders.AddDecoder ( decoder );
			}

			ImageDecoders.AddDefaultDecoders ();
			SoundDecoders.AddDefaultDecoders ();
		}

		public static void SetupFixedLogicTimeStep ( TimeSpan fixedUpdateTimeStep, TimeSpan fixedDrawTimeStep )
		{
			LiqueurSystem.FixedUpdateTimeStep = fixedUpdateTimeStep;
			LiqueurSystem.FixedDrawTimeStep = fixedDrawTimeStep;
		}

		public static void SetupInputDevices ( params Type [] inputDevices )
		{
			LiqueurSystem.Initialize += ( object sender, EventArgs e ) =>
			{
				foreach ( Type type in inputDevices )
				{
					if ( type.IsSubclassOf ( typeof ( RawGamePad ) ) )
					{
						LiqueurSystem.GamePads.AddGamePad ( type, LiqueurSystem.Window, PlayerIndex.Player1 );
						LiqueurSystem.GamePads.AddGamePad ( type, LiqueurSystem.Window, PlayerIndex.Player2 );
						LiqueurSystem.GamePads.AddGamePad ( type, LiqueurSystem.Window, PlayerIndex.Player3 );
						LiqueurSystem.GamePads.AddGamePad ( type, LiqueurSystem.Window, PlayerIndex.Player4 );
					}
					else
					{
						object device = Activator.CreateInstance ( type, LiqueurSystem.Window );
						if ( type.IsSubclassOf ( typeof ( RawKeyboard ) ) )
							LiqueurSystem.Keyboard = device as RawKeyboard;
						else if ( type.IsSubclassOf ( typeof ( RawMouse ) ) )
							LiqueurSystem.Mouse = device as RawMouse;
						else if ( type.IsSubclassOf ( typeof ( RawTouchPanel ) ) )
							LiqueurSystem.TouchPanel = device as RawTouchPanel;
						else if ( type.IsSubclassOf ( typeof ( RawAccelerometer ) ) )
							LiqueurSystem.Accelerometer = device as RawAccelerometer;
					}
				}
			};
		}

		public static void SetupInputDevices<T1, T2, T3, T4, T5> ()
			where T1 : RawKeyboard
			where T2 : RawMouse
			where T3 : RawGamePad
			where T4 : RawTouchPanel
			where T5 : RawAccelerometer
		{
			LiqueurSystem.Initialize += ( object sender, EventArgs e ) =>
			{
				LiqueurSystem.Keyboard = Activator.CreateInstance ( typeof ( T1 ), LiqueurSystem.Window ) as RawKeyboard;
				LiqueurSystem.Mouse = Activator.CreateInstance ( typeof ( T2 ), LiqueurSystem.Window ) as RawMouse;
				LiqueurSystem.GamePads.AddGamePad ( typeof ( T3 ), LiqueurSystem.Window, PlayerIndex.Player1 );
				LiqueurSystem.GamePads.AddGamePad ( typeof ( T3 ), LiqueurSystem.Window, PlayerIndex.Player2 );
				LiqueurSystem.GamePads.AddGamePad ( typeof ( T3 ), LiqueurSystem.Window, PlayerIndex.Player3 );
				LiqueurSystem.GamePads.AddGamePad ( typeof ( T3 ), LiqueurSystem.Window, PlayerIndex.Player4 );
				LiqueurSystem.TouchPanel = Activator.CreateInstance ( typeof ( T4 ), LiqueurSystem.Window ) as RawTouchPanel;
				LiqueurSystem.Accelerometer = Activator.CreateInstance ( typeof ( T5 ), LiqueurSystem.Window ) as RawAccelerometer;
			};
		}

		public static void Run<T1, T2, T3> ( IFileSystem mainContentsFileSystem )
			where T1 : ILauncher
			where T2 : IScriptEngine
			where T3 : Scene
		{
			Run<T1, T2> ( mainContentsFileSystem, Activator.CreateInstance<T3> () );
		}

		public static void Run<T1, T2> ( IFileSystem mainContentsFileSystem, object scene )
			where T1 : ILauncher
			where T2 : IScriptEngine
		{
			MainContents = new ContentManager ();
			MainContents.FileSystem = mainContentsFileSystem;
			MainContents.AddDefaultContentLoader ();

			try
			{
				ScriptEngine = Activator.CreateInstance<T2> ();
				ScriptEngine.AddReference ( Assembly.GetCallingAssembly () );
				ScriptEngine.AddReference ( Assembly.GetExecutingAssembly () );
			}
			catch { }

			Scene sceneObject;
			if ( scene is string )
				sceneObject = new WalnutScene ( MainContents.FileSystem.OpenFile ( scene as string ), MainContents );
			else if ( scene is Scene ) sceneObject = scene as Scene;
			else if ( scene is JsonEntry ) sceneObject = new WalnutScene ( scene as JsonEntry, MainContents );
			else throw new ArgumentNullException ();

			LiqueurSystem.Run ( Activator.CreateInstance<T1> (), sceneObject );

			MainContents.Reset ();
		}

		public static void Run<T1, T2> ( Stream package )
			where T1 : ILauncher
			where T2 : IScriptEngine
		{
			using ( MainWalnutPackage = new WalnutPackage ( package ) )
			{
				Run<T1, T2> ( MainWalnutPackage.PackageFileSystem, 
					MainWalnutPackage.ResourceTable.Load<WalnutScene> ( MainWalnutPackage.MainScene,
						MainWalnutPackage.ResourceTable ) );
			}
		}
    }
}

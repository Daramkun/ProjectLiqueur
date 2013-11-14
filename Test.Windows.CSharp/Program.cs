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
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Spirit.Graphics;
using Daramkun.Liqueur.Spirit.Nodes;

namespace Test.Windows.CSharp
{
	static class Program
	{
		[STAThread]
		static void Main ()
		{
			LiqueurSystem.SkipInitializeException = true;
			//try
			//{
				LiqueurSystem.Run ( new Launcher ( true ),
					new Test.Game.Dodge.Container (),
					//new Test.Game.PerformanceTester.Container (),
					() =>
					{
						LiqueurSystem.Keyboard = new Keyboard ( LiqueurSystem.Window );
					} );
			//}
			//catch ( Exception ex ) { Debug.WriteLine ( ex.Message ); Debug.WriteLine ( ex.StackTrace ); }
		}
	}
}

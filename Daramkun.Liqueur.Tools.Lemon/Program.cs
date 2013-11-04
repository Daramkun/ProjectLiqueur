using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Tools.Lemon.Tools;

namespace Daramkun.Liqueur.Tools.Lemon
{
	class Program
	{
		static void Main ( string [] args )
		{
			if ( args [ 0 ] == "-h" )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  lemon.exe -[lsf|res|stt] [args]" );
				Console.WriteLine ( "    lsf: Liqueur Sprite Font Generator" );
				Console.WriteLine ( "    res: Resource Table Editor" );
				Console.WriteLine ( "    stt: String Table Editor" );
				Console.WriteLine ( "    src: Script Packer" );
				Console.WriteLine ( "    pck: Content Package Packer" );
			}
			else
			{
				List<string> arguments = new List<string> ( args );
				string tool = arguments [ 0 ];
				arguments.RemoveAt ( 0 );
				switch ( tool )
				{
					case "-lsf":
						new LiqueurSpriteFontGenerator ().Run ( arguments.ToArray () );
						break;
					case "-res":
						new ResourceTableEditor ().Run ( arguments.ToArray () );
						break;
					case "-stt":
						new StringTableEditor ().Run ( arguments.ToArray () );
						break;
					case "-scr":
						new ScriptPacker ().Run ( arguments.ToArray () );
						break;
					case "-pck":
						new Packer ().Run ( arguments.ToArray () );
						break;
				}
			}
		}
	}
}

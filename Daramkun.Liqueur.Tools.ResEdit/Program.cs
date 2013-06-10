using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Tools.ResEdit
{
	class Program
	{
		static void Main ( string [] args )
		{
			if ( args.Length == 0 || args [ 0 ] == "-h" )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  resedit.exe filename" );
			}
			else
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				using ( FileStream stream = new FileStream ( args [ 0 ], FileMode.OpenOrCreate, FileAccess.ReadWrite ) )
				{

				}
			}
		}
	}
}

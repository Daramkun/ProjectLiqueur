using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public class FileFormatAttribute : Attribute
	{
		public string [] FileExtension { get; private set; }

		public FileFormatAttribute ( params string [] fileExtension )
		{
			FileExtension = new string [ fileExtension.Length ];
			for ( int i = 0; i < FileExtension.Length; i++ )
				FileExtension [ i ] = fileExtension [ i ].ToUpper ().Trim ();
		}
	}
}

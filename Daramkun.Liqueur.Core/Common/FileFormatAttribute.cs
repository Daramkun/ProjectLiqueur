using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// File format Attribute
	/// </summary>
	public class FileFormatAttribute : Attribute
	{
		/// <summary>
		/// File extensions
		/// </summary>
		public string [] FileExtension { get; private set; }

		/// <summary>
		/// Constructor of File format attribute
		/// </summary>
		/// <param name="fileExtension">File extension strings</param>
		public FileFormatAttribute ( params string [] fileExtension )
		{
			FileExtension = new string [ fileExtension.Length ];
			for ( int i = 0; i < FileExtension.Length; i++ )
				FileExtension [ i ] = fileExtension [ i ].ToUpper ().Trim ();
		}
	}
}

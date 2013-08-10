namespace Hjg.Pngcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngjInputException : PngjException
	{
		private const long serialVersionUID = 1L;

		public PngjInputException ( String message, Exception cause )
			: base ( message, cause )
		{
		}

		public PngjInputException ( String message )
			: base ( message )
		{
		}

		public PngjInputException ( Exception cause )
			: base ( cause )
		{
		}
	}
}

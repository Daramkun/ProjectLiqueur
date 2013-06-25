using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Logging
{
	[Flags]
	public enum MessageFormat
	{
		Message = 0,

		LogLevel = 1 << 0,
		DateTime = 1 << 1,
		Thread = 1 << 2,
		CultureName = 1 << 3,
	}
}

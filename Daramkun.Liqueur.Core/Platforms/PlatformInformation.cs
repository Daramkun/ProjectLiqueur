using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Platforms
{
	public struct PlatformInformation
	{
		public Platform Platform { get; set; }
		public Version PlatformVersion { get; set; }

		public string UserName { get; set; }
		public string MachineUniqueIdentifier { get; set; }
	}
}

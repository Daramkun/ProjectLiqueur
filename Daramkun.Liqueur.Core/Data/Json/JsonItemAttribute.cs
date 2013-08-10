using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	[AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class JsonItemAttribute : Attribute
	{
		string _name;
		Guid _instanceGUID;

		public string Name { get { return _name; } set { _name = value; } }

		public JsonItemAttribute ( params string [] args )
		{
			if ( args.Length > 0 )
				_name = args [ 0 ];
			_instanceGUID = Guid.NewGuid ();
		}
	}
}

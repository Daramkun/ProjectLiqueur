using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Make User class properties to JsonItem
	/// </summary>
	[AttributeUsage ( AttributeTargets.Property )]
	public sealed class JsonItembleAttribute : Attribute
	{
		string _name;
		Guid _instanceGUID;

		/// <summary>
		/// JsonItem name
		/// </summary>
		public string Name { get { return _name; } set { _name = value; } }

		/// <summary>
		/// Setting Property like JsonItem
		/// </summary>
		/// <param name="args">This is null than JsonItem's Name is property name. but set one string, that is JsonItem's Name</param>
		public JsonItembleAttribute ( params string [] args )
		{
			if ( args.Length > 0 )
				_name = args [ 0 ];
			_instanceGUID = Guid.NewGuid ();
		}
	}
}

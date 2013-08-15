using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public interface IIndexBuffer : IDisposable
	{
		int Length { get; }

		int [] Indices { get; set; }

		object Handle { get; }
	}
}

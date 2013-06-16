using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public interface IEffect : IDisposable
	{
		void BindAttribute ( int index, string attribute );
		void LinkShader ();
		void BindShader ();
		void SendUniform ( string uniform, object data );
	}
}

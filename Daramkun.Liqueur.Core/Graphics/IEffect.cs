using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public interface IEffect : IDisposable
	{
		void Dispatch ( Action<IEffect, int> dispatchEvent );

		void BeginPass ( int pass );
		void EndPass ();

		T GetArgument<T> ( string parameter );
		void SetArgument<T> ( string parameter, T argument );
	}
}
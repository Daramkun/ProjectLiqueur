using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public interface IEffect : IDisposable
	{
		void Dispatch ( Action<IEffect> dispatchEvent );

		void Commit ();

		T GetArgument<T> ( string parameter );
		void SetArgument<T> ( string parameter, T argument );

		ITexture2D Texture { get; set; }
	}
}
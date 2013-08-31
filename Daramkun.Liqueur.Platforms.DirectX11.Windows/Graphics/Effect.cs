using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{

		public object Handle
		{
			get { throw new NotImplementedException (); }
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			throw new NotImplementedException ();
		}

		public T GetArgument<T> ( string parameter )
		{
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			throw new NotImplementedException ();
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			throw new NotImplementedException ();
		}
	}
}

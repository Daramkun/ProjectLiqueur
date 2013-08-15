using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public class PassedEffect : IEffect, IDisposable
	{
		IEffect [] effects;

		public int Count { get { return effects.Length; } }

		public object Handle { get { return effects; } }

		public PassedEffect ( params IEffect [] effect )
		{
			effects = effect.Clone () as IEffect [];
		}

		~PassedEffect ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				foreach ( IEffect ef in effects )
					ef.Dispose ();
				effects = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			foreach ( IEffect ef in effects )
				ef.Dispatch ( dispatchEvent );
		}

		public void DispatchPass ( Action<IEffect> dispatchEvent, int pass )
		{
			effects [ pass ].Dispatch ( dispatchEvent );
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			foreach ( IEffect ef in effects )
				ef.SetArgument<T> ( parameter, argument );
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			foreach ( IEffect ef in effects )
				ef.SetTextures ( textures );
		}
	}
}

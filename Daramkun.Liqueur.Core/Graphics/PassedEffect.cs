﻿using System;
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
			effects = new IEffect [ effect.Length ];
			for ( int i = 0; i < effect.Length; i++ )
				effects [ i ] = effect [ i ];
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

		public T GetArgument<T> ( string parameter ) where T : struct
		{
			return effects [ 0 ].GetArgument<T> ( parameter );
		}

		public void SetArgument<T> ( string parameter, T argument ) where T : struct
		{
			foreach ( IEffect ef in effects )
				ef.SetArgument<T> ( parameter, argument );
		}

		public void SetTexture ( TextureArgument texture )
		{
			foreach ( IEffect ef in effects )
				ef.SetTexture ( texture );
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			foreach ( IEffect ef in effects )
				ef.SetTextures ( textures );
		}
	}
}

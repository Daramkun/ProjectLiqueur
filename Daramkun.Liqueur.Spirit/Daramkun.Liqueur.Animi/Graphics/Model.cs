using System;
using System.Collections.Generic;

namespace Daramkun.Liqueur.Animi.Graphics
{
	public class Model<T> : IDisposable where T : struct
	{
		private List<Mesh<T>> meshesList;

		public IEnumerable<Mesh<T>> Meshes { get { return meshesList; } }

		public Model ( params Mesh<T> [] meshes )
		{
			meshesList = new List<Mesh<T>> ( meshes );
		}

		~Model ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				foreach ( Mesh<T> mesh in meshesList )
					mesh.Dispose ();
				meshesList = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}


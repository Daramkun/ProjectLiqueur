using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Loaders;

namespace Daramkun.Liqueur.Contents
{
	public sealed class ContentManager : IDisposable
	{
		List<IContentLoader> contentLoaders = new List<IContentLoader> ();
		Dictionary<string, object> loadedContent = new Dictionary<string, object> ();

		public IFileSystem FileSystem { get; set; }

		public void AddContentLoader ( IContentLoader contentLoader )
		{
			contentLoaders.Add ( contentLoader );
		}

		public void RemoveContentLoader ( IContentLoader contentLoader )
		{
			contentLoaders.Remove ( contentLoader );
		}

		public void AddDefaultContentLoader ()
		{
			AddContentLoader ( new TextContentLoader () );
			AddContentLoader ( new ImageContentLoader () );
			AddContentLoader ( new SoundContentLoader () );
			AddContentLoader ( new LsfFontContentLoader () );
			AddContentLoader ( new ZipLsfFontContentLoader () );
			AddContentLoader ( new ZipContentLoader () );
		}

		public void AddContent ( string filename, object obj )
		{
			loadedContent.Add ( filename, obj );
		}

		public void RemoveContent ( string filename )
		{
			if ( !loadedContent.ContainsKey ( filename ) ) return;
			if ( loadedContent [ filename ] is IDisposable )
				( loadedContent [ filename ] as IDisposable ).Dispose ();
			loadedContent.Remove ( filename );
		}

		private bool IsSubtypeOf ( Type majorType, Type minorType )
		{
			if ( majorType == minorType || majorType.IsSubclassOf ( minorType ) )
				return true;
			else if ( minorType.IsInterface )
			{
				foreach ( Type type in majorType.GetInterfaces () )
					if ( type == minorType )
						return true;
			}
			return false;
		}

		public T Load<T> ( string filename, params object [] args )
		{
			if ( FileSystem == null ) return default ( T );

			if ( loadedContent.ContainsKey ( filename ) )
				return (T)loadedContent [ filename ];

			foreach ( IContentLoader contentLoader in contentLoaders )
			{
				if ( IsSubtypeOf ( typeof ( T ), contentLoader.ContentType ) )
				{
					Stream stream = FileSystem.OpenFile ( filename );
					object data = contentLoader.Load ( stream, args );
					loadedContent.Add ( filename, data );
					if ( !contentLoader.IsSelfStreamDispose )
						stream.Dispose ();
					return ( T ) data;
				}
			}
			
			return default ( T );
		}

		public void Reset ()
		{
			foreach ( KeyValuePair<string, object> obj in loadedContent )
				if ( obj.Value is IDisposable )
					( obj.Value as IDisposable ).Dispose ();
			loadedContent.Clear ();
		}

		public void Dispose ()
		{
			Reset ();
			loadedContent = null;
			contentLoaders.Clear ();
			contentLoaders = null;
		}
	}
}

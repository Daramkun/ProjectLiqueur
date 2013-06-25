using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents
{
	public sealed class ContentManager : IDisposable
	{
		public static List<Assembly> ContentLoaderAssemblies { get; private set; }

		List<IContentLoader> contentLoaders = new List<IContentLoader> ();
		Dictionary<string, object> loadedContent = new Dictionary<string, object> ();

		public IFileSystem FileSystem { get; set; }
		public bool IsCultureMode { get; set; }

		static ContentManager ()
		{
			ContentLoaderAssemblies = new List<Assembly> ();
			ContentLoaderAssemblies.Add ( Assembly.GetExecutingAssembly () );
		}

		public ContentManager ()
		{
			FileSystem = null;
			IsCultureMode = true;
		}

		public ContentManager ( IFileSystem fileSystem )
		{
			FileSystem = fileSystem;
			IsCultureMode = true;
		}

		public void AddContentLoader ( IContentLoader contentLoader )
		{
			if ( contentLoaders.Contains ( contentLoader ) ) return;
			contentLoaders.Add ( contentLoader );
		}

		public void RemoveContentLoader ( IContentLoader contentLoader )
		{
			contentLoaders.Remove ( contentLoader );
		}

		public void AddDefaultContentLoader ()
		{
			foreach ( Assembly assembly in ContentLoaderAssemblies )
			{
				foreach ( Type type in assembly.GetTypes () )
				{
					if ( Utilities.IsSubtypeOf ( type, typeof ( IContentLoader ) ) && type != typeof ( IContentLoader )
						&& !type.IsAbstract && !type.IsInterface && type.IsPublic )
						AddContentLoader ( Activator.CreateInstance ( type ) as IContentLoader );
				}
			}
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

		private string PathCombine ( string path, string filename )
		{
			if ( path == null || path.Length == 0 ) return filename;

			if ( path.IndexOf ( '\\' ) >= 0 )
			{
				if ( path [ path.Length - 1 ] == '\\' )
					return path + filename;
				else return string.Format ( "{0}\\{1}", path, filename );
			}
			else
			{
				if ( path [ path.Length - 1 ] == '/' )
					return path + filename;
				else return string.Format ( "{0}/{1}", path, filename );
			}
		}

		public T Load<T> ( string filename, params object [] args )
		{
			if ( FileSystem == null )
				throw new NullOfFileSystemException ();

			if ( IsCultureMode )
			{
				if ( FileSystem.IsFileExist ( PathCombine ( LiqueurSystem.CurrentCulture.Name, filename ) ) )
					filename = PathCombine ( LiqueurSystem.CurrentCulture.Name, filename );
				else if ( FileSystem.IsFileExist ( PathCombine ( CultureInfo.InvariantCulture.Name, filename ) ) )
					filename = PathCombine ( CultureInfo.InvariantCulture.Name, filename );
				else throw new FileNotFoundException ();
			}

			if ( loadedContent.ContainsKey ( filename ) )
				return ( T ) loadedContent [ filename ];

			foreach ( IContentLoader contentLoader in contentLoaders )
			{
				if ( Utilities.IsSubtypeOf ( typeof ( T ), contentLoader.ContentType ) )
				{
					Stream stream = FileSystem.OpenFile ( filename );
					object data = contentLoader.Load ( stream, args );
					loadedContent.Add ( filename, data );
					if ( !contentLoader.IsSelfStreamDispose )
						stream.Dispose ();
					return ( T ) data;
				}
			}
			
			throw new FileNotFoundException ();
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

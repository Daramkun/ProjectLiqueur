using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Contents
{
	public sealed class ContentManager : IDisposable
	{
		public static List<Assembly> ContentLoaderAssemblies { get; private set; }

		List<IContentLoader> contentLoaders = new List<IContentLoader> ();
		Dictionary<string, object> loadedContent = new Dictionary<string, object> ();

		public IFileSystem FileSystem { get; set; }

		static ContentManager ()
		{
			ContentLoaderAssemblies = new List<Assembly> ();
			ContentLoaderAssemblies.Add ( Assembly.GetExecutingAssembly () );
		}

		public ContentManager ()
		{
			FileSystem = null;
		}

		public ContentManager ( IFileSystem fileSystem )
		{
			FileSystem = fileSystem;
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
			string temp;
			return Load<T> ( filename, out temp, args );
		}

		public T Load<T> ( string filename, out string realname, params object [] args )
		{
			if ( FileSystem == null )
				throw new ArgumentNullException ();

			IContentLoader loader = null;
			foreach ( IContentLoader contentLoader in contentLoaders )
			{
				if ( Utilities.IsSubtypeOf ( typeof ( T ), contentLoader.ContentType ) )
					loader = contentLoader;
			}

			realname = null;

			if ( loader == null )
				throw new ArgumentException ();

			if ( !FileSystem.IsFileExist ( filename ) )
			{
				if ( FileSystem.IsFileExist ( PathCombine ( LiqueurSystem.CurrentCulture.Name, filename ) ) )
					realname = PathCombine ( LiqueurSystem.CurrentCulture.Name, filename );
				else
				{
					bool exist = false;
					foreach ( string ext in loader.FileExtensions )
					{
						if ( FileSystem.IsFileExist ( string.Format ( "{0}.{1}", filename, ext.ToLower () ) ) )
						{
							realname = string.Format ( "{0}.{1}", filename, ext.ToLower () );
							exist = true;
							break;
						}
						else if ( FileSystem.IsFileExist ( PathCombine ( LiqueurSystem.CurrentCulture.Name, string.Format ( "{0}.{1}", filename, ext.ToLower () ) ) ) )
						{
							realname = PathCombine ( LiqueurSystem.CurrentCulture.Name, string.Format ( "{0}.{1}", filename, ext.ToLower () ) );
							exist = true;
							break;
						}
					}

					if ( !exist )
						throw new FileNotFoundException ();
				}
			}

			if ( loadedContent.ContainsKey ( realname ) )
			{
				realname = filename;
				return ( T ) loadedContent [ realname ];
			}
			else
			{
				Stream stream = FileSystem.OpenFile ( realname );
				object data = loader.Load ( stream, args );
				loadedContent.Add ( realname, data );
				if ( !loader.IsSelfStreamDispose )
					stream.Dispose ();
				return ( T ) data;
			}
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

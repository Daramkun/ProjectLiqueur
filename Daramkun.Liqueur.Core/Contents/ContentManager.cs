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
	/// <summary>
	/// Content Manager class
	/// </summary>
	public sealed class ContentManager : IDisposable
	{
		/// <summary>
		/// .NET Assemblies included Content Loader classes
		/// </summary>
		public static List<Assembly> ContentLoaderAssemblies { get; private set; }

		List<IContentLoader> contentLoaders = new List<IContentLoader> ();
		Dictionary<string, object> loadedContent = new Dictionary<string, object> ();

		/// <summary>
		/// Main File System of Content Manager
		/// </summary>
		public IFileSystem FileSystem { get; set; }

		static ContentManager ()
		{
			ContentLoaderAssemblies = new List<Assembly> ();
			ContentLoaderAssemblies.Add ( Assembly.GetExecutingAssembly () );
		}

		/// <summary>
		/// Constructor of Content Manager
		/// </summary>
		public ContentManager ()
		{
			FileSystem = null;
		}

		/// <summary>
		/// Constructor of Content Manager
		/// </summary>
		/// <param name="fileSystem">Main File System of Content Manager</param>
		public ContentManager ( IFileSystem fileSystem )
		{
			FileSystem = fileSystem;
		}

		/// <summary>
		/// Add Content Loader
		/// </summary>
		/// <param name="contentLoader">Content Loader</param>
		public void AddContentLoader ( IContentLoader contentLoader )
		{
			if ( contentLoaders.Contains ( contentLoader ) ) return;
			contentLoaders.Add ( contentLoader );
		}

		/// <summary>
		/// Remove Content Loader
		/// </summary>
		/// <param name="contentLoader">Content Loader</param>
		public void RemoveContentLoader ( IContentLoader contentLoader )
		{
			contentLoaders.Remove ( contentLoader );
		}

		/// <summary>
		/// Add Default Content Loader from .NET Assemblies
		/// </summary>
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

		/// <summary>
		/// Add User Content
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <param name="obj">User Content</param>
		public void AddContent ( string filename, object obj )
		{
			loadedContent.Add ( filename, obj );
		}

		/// <summary>
		/// Remove User Content
		/// </summary>
		/// <param name="filename">Filename</param>
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

		/// <summary>
		/// Load Content
		/// </summary>
		/// <typeparam name="T">Content Type</typeparam>
		/// <param name="filename">Filename</param>
		/// <param name="args">Argument, if you need</param>
		/// <returns>Loaded content</returns>
		public T Load<T> ( string filename, params object [] args )
		{
			string temp;
			return Load<T> ( filename, out temp, args );
		}

		/// <summary>
		/// Load Content
		/// </summary>
		/// <typeparam name="T">Content Type</typeparam>
		/// <param name="filename">Filename</param>
		/// <param name="key">Real filename</param>
		/// <param name="args">Argument, if you need</param>
		/// <returns>Loaded content</returns>
		public T Load<T> ( string filename, out string key, params object [] args )
		{
			if ( FileSystem == null )
				throw new ArgumentNullException ();

			IContentLoader loader = null;
			foreach ( IContentLoader contentLoader in contentLoaders )
			{
				if ( Utilities.IsSubtypeOf ( typeof ( T ), contentLoader.ContentType ) )
					loader = contentLoader;
			}

			key = null;

			if ( loader == null )
				throw new ArgumentException ();

			if ( !FileSystem.IsFileExist ( filename ) )
			{
				if ( FileSystem.IsFileExist ( PathCombine ( LiqueurSystem.CurrentCulture.Name, filename ) ) )
					key = PathCombine ( LiqueurSystem.CurrentCulture.Name, filename );
				else
				{
					bool exist = false;
					foreach ( string ext in loader.FileExtensions )
					{
						if ( FileSystem.IsFileExist ( string.Format ( "{0}.{1}", filename, ext.ToLower () ) ) )
						{
							key = string.Format ( "{0}.{1}", filename, ext.ToLower () );
							exist = true;
							break;
						}
						else if ( FileSystem.IsFileExist ( PathCombine ( LiqueurSystem.CurrentCulture.Name, string.Format ( "{0}.{1}", filename, ext.ToLower () ) ) ) )
						{
							key = PathCombine ( LiqueurSystem.CurrentCulture.Name, string.Format ( "{0}.{1}", filename, ext.ToLower () ) );
							exist = true;
							break;
						}
					}

					if ( !exist )
						throw new FileNotFoundException ();
				}
			}
			else key = filename;

			filename = key;
			key = MakeKey ( filename, args );

			if ( loadedContent.ContainsKey ( filename ) )
			{
				key = filename;
				return ( T ) loadedContent [ key ];
			}
			else
			{
				Stream stream = FileSystem.OpenFile ( filename );
				object data = loader.Load ( stream, args );
				loadedContent.Add ( key, data );
				if ( !loader.IsSelfStreamDispose )
					stream.Dispose ();
				return ( T ) data;
			}
		}

		private string MakeKey ( string filename, params object [] args )
		{
			foreach ( object o in args )
				filename += "." + o.ToString ();
			return filename;
		}

		/// <summary>
		/// Remove all loaded contents
		/// </summary>
		public void Reset ()
		{
			foreach ( KeyValuePair<string, object> obj in loadedContent )
				if ( obj.Value is IDisposable )
					( obj.Value as IDisposable ).Dispose ();
			loadedContent.Clear ();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose ()
		{
			Reset ();
			loadedContent = null;
			contentLoaders.Clear ();
			contentLoaders = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Data.Json;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.IO.Compression;

namespace Daramkun.Liqueur.Contents
{
	public enum VariableType
	{
		Unknown = 0,

		Integer = 1,
		FloatingPoint = 2,
		Boolean = 3,
		String = 4,

		DateTime = 5,
		TimeSpan = 6,

		JsonEntry = 7,
	}

	public class VariableTable
	{
		Dictionary<Guid, object []> variableTable;

		public VariableTable ()
		{
			variableTable = new Dictionary<Guid, object []> ();
		}

		public VariableTable ( Stream stream )
			: this ()
		{
			LoadFrom ( stream );
		}

		public void LoadFrom ( Stream stream )
		{
			if ( stream == null || stream.Length == 0 )
				return;

			BinaryReader reader = new BinaryReader ( stream );

			if ( Encoding.UTF8.GetString ( reader.ReadBytes ( 4 ), 0, 4 ) != "LQSV" )
				throw new FileFormatMismatchException ();

			DeflateStream df = new DeflateStream ( stream, CompressionMode.Decompress );
			reader = new BinaryReader ( df );
			while ( true )
			{
				Guid guid = new Guid ( reader.ReadBytes ( 16 ) );
				if ( guid == Guid.Empty ) break;
				int count = reader.ReadInt32 ();
				object [] contain = new object [ count ];

				variableTable.Add ( guid, contain );

				for ( int i = 0; i < count; i++ )
				{
					VariableType type = ( VariableType ) reader.ReadByte ();
					object data;
					switch ( type )
					{
						case VariableType.Boolean: data = reader.ReadBoolean (); break;
						case VariableType.Integer: data = reader.ReadInt32 (); break;
						case VariableType.FloatingPoint: data = reader.ReadSingle (); break;
						case VariableType.String: data = reader.ReadString (); break;
						case VariableType.TimeSpan: data = new TimeSpan ( reader.ReadInt32 (), reader.ReadByte (), reader.ReadByte (), reader.ReadByte (), reader.ReadInt16 () ); break;
						case VariableType.DateTime: data = new DateTime ( reader.ReadByte (), reader.ReadByte (), reader.ReadByte (), reader.ReadByte (),
							reader.ReadByte (), reader.ReadByte (), reader.ReadInt16 () ); break;
						case VariableType.JsonEntry: data = JsonParser.Parse ( reader.ReadString () ); break;

						default: data = null; break;
					}
					contain [ i ] = data;
				}
			}
		}

		public void SaveTo ( Stream stream )
		{
			BinaryWriter writer = new BinaryWriter ( stream );
			writer.Write ( Encoding.UTF8.GetBytes ( "LQSV" ) );

			DeflateStream df = new DeflateStream ( stream, CompressionMode.Compress );
			writer = new BinaryWriter ( df );
			foreach ( KeyValuePair<Guid, object []> pair in variableTable )
			{
				writer.Write ( pair.Key.ToByteArray () );
				writer.Write ( pair.Value.Length );
				foreach ( object data in pair.Value )
				{
					if ( data.GetType () == typeof ( bool ) )
					{
						writer.Write ( ( byte ) VariableType.Boolean );
						writer.Write ( ( bool ) data );
					}
					else if ( data.GetType () == typeof ( int ) || data.GetType () == typeof ( uint ) ||
							 data.GetType () == typeof ( short ) || data.GetType () == typeof ( ushort ) ||
							 data.GetType () == typeof ( long ) || data.GetType () == typeof ( ulong ) ||
							 data.GetType () == typeof ( sbyte ) || data.GetType () == typeof ( byte ) )
					{
						writer.Write ( ( byte ) VariableType.Integer );
						writer.Write ( ( int ) data );
					}
					else if ( data.GetType () == typeof ( float ) || data.GetType () == typeof ( double ) )
					{
						writer.Write ( ( byte ) VariableType.FloatingPoint );
						writer.Write ( ( float ) data );
					}
					else if ( data.GetType () == typeof ( string ) )
					{
						writer.Write ( ( byte ) VariableType.String );
						writer.Write ( ( string ) data );
					}
					else if ( data.GetType () == typeof ( TimeSpan ) )
					{
						writer.Write ( ( byte ) VariableType.TimeSpan );
						TimeSpan timeSpan = ( TimeSpan ) data;
						writer.Write ( ( int ) timeSpan.Days );
						writer.Write ( ( byte ) timeSpan.Hours );
						writer.Write ( ( byte ) timeSpan.Minutes );
						writer.Write ( ( byte ) timeSpan.Seconds );
						writer.Write ( ( short ) timeSpan.Milliseconds );
					}
					else if ( data.GetType () == typeof ( DateTime ) )
					{
						writer.Write ( ( byte ) VariableType.DateTime );
						DateTime dateTime = ( DateTime ) data;
						writer.Write ( ( byte ) dateTime.Year );
						writer.Write ( ( byte ) dateTime.Month );
						writer.Write ( ( byte ) dateTime.Day );
						writer.Write ( ( byte ) dateTime.Hour );
						writer.Write ( ( byte ) dateTime.Minute );
						writer.Write ( ( byte ) dateTime.Second );
						writer.Write ( ( short ) dateTime.Millisecond );
					}
					else if ( data.GetType () == typeof ( JsonEntry ) )
					{
						writer.Write ( ( byte ) VariableType.JsonEntry );
						writer.Write ( ( string ) data.ToString () );
					}
				}
			}
			writer.Write ( Guid.Empty.ToByteArray () );
		}

		public bool IsSettingCompleted ( Guid packId )
		{
			return ( variableTable.ContainsKey ( packId ) );
		}

		public void AddPackageVariableTable ( Guid packId, int range )
		{
			variableTable.Add ( packId, new object [ range ] );
		}

		public object GetVariable ( Guid packId, int index )
		{
			return variableTable [ packId ] [ index ];
		}

		public T GetVariable<T> ( Guid packId, int index ) where T : struct
		{
			return ( T ) GetVariable ( packId, index );
		}

		public void SetVariable ( Guid packId, int index, object value )
		{
			variableTable [ packId ] [ index ] = value;
		}
	}
}

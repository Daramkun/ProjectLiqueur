using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Json Parser
	/// </summary>
	public static class JsonParser
	{
		private enum TokenType
		{
			Unknown,
			String,
			RecordSplit,
			Json,
			Array,
			Boolean,
			Integer,
			FloatingPoint,
			Null,
		}

		private static JsonArray ArrayParse ( Stream stream )
		{
			JsonArray jsonArray = new JsonArray ();

			StringBuilder automataToken = null;
			TokenType tokenType = TokenType.Unknown;

			BinaryReader br = new BinaryReader ( stream );
			br.ReadChar ();

			while ( stream.CanRead && stream.Position != stream.Length )
			{
				char readedCharacter = br.ReadChar ();

				switch ( tokenType )
				{
					case TokenType.Unknown:
						{
							if ( readedCharacter == ' ' || readedCharacter == '\t' )
								continue;
							else if ( readedCharacter == '"' )
								tokenType = TokenType.String;
							else if ( readedCharacter == '{' )
								jsonArray.Add ( new JsonItem ( null, Parse ( stream ) ) );
							else if ( readedCharacter == '[' )
								jsonArray.Add ( new JsonItem ( null, ArrayParse ( stream ) ) );
							else if ( readedCharacter == ']' )
							{
								return jsonArray;
							}
							else if ( ( readedCharacter == 't' || readedCharacter == 'f' ) ||
								( readedCharacter == 'T' || readedCharacter == 'F' ) )
							{
								stream.Seek ( -1, SeekOrigin.Current );
								tokenType = TokenType.Boolean;
							}
							else if ( readedCharacter == 'n' || readedCharacter == 'N' )
							{
								stream.Seek ( -1, SeekOrigin.Current );
								tokenType = TokenType.Null;
							}
							else if ( readedCharacter >= '0' &&
								readedCharacter <= '9' )
							{
								stream.Seek ( -1, SeekOrigin.Current );
								tokenType = TokenType.Integer;
							}
							automataToken = new StringBuilder ();
						}
						break;
					case TokenType.String:
						{
							if ( readedCharacter == '"' )
							{
								jsonArray.Add ( new JsonItem ( null, automataToken.ToString () ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							else if ( readedCharacter == '\\' )
							{
								readedCharacter = br.ReadChar ();
								if ( readedCharacter == 'n' )
									automataToken.Append ( '\n' );
								else
									automataToken.Append ( readedCharacter );
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.Boolean:
						{
							if ( automataToken.ToString ().ToLower () == "true" ||
								automataToken.ToString ().ToLower () == "false" )
							{
								jsonArray.Add ( new JsonItem ( null, bool.Parse ( automataToken.ToString () ) ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.Null:
						{
							if ( automataToken.ToString ().ToLower () == "null" )
							{
								jsonArray.Add ( new JsonItem ( null, null ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.Integer:
						{
							if ( readedCharacter == '.' )
								tokenType = TokenType.FloatingPoint;
							else if ( !( readedCharacter >= '0' &&
								readedCharacter <= '9' ) )
							{
								jsonArray.Add ( new JsonItem ( null, int.Parse ( automataToken.ToString () ) ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.FloatingPoint:
						{
							if ( !( readedCharacter >= '0' &&
								   readedCharacter <= '9' ) )
							{
								jsonArray.Add ( new JsonItem ( null, double.Parse ( automataToken.ToString () ) ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
				}
			}

			return jsonArray;
		}

		/// <summary>
		/// Parse Json string stream
		/// </summary>
		/// <param name="stream">Json String stream</param>
		/// <returns>Parsed Json Entry object</returns>
		public static JsonEntry Parse ( Stream stream )
		{
			JsonEntry jsonEntry = new JsonEntry ();

			StringBuilder automataToken = null;
			Stack<object> tokenStack = new Stack<object> ();
			TokenType tokenType = TokenType.Unknown;

			BinaryReader br = new BinaryReader ( stream );
			br.ReadChar ();

			while ( stream.CanRead && stream.Position != stream.Length )
			{
				char readedCharacter = br.ReadChar ();

				switch ( tokenType )
				{
					case TokenType.Unknown:
						{
							if ( readedCharacter == '"' )
								tokenType = TokenType.String;
							else if ( readedCharacter == '{' )
								tokenStack.Push ( Parse ( stream ) );
							else if ( readedCharacter == '}' )
							{
								if ( tokenStack.Count >= 2 )
								{
									object data = tokenStack.Pop ();
									string name = tokenStack.Pop () as string;
									JsonItem item = new JsonItem ( name, data );
									jsonEntry.Add ( item );
								}
								return jsonEntry;
							}
							else if ( readedCharacter == '[' )
								tokenStack.Push ( ArrayParse ( stream ) );
							else if ( readedCharacter == ',' )
							{
								object data = tokenStack.Pop ();
								string name = tokenStack.Pop () as string;
								JsonItem item = new JsonItem ( name, data );
								jsonEntry.Add ( item );
								tokenType = TokenType.Unknown;
								automataToken = null;
							}
							else if ( ( readedCharacter == 't' || readedCharacter == 'f' ) ||
								( readedCharacter == 'T' || readedCharacter == 'F' ) )
							{
								tokenType = TokenType.Boolean;
								stream.Seek ( -1, SeekOrigin.Current );
							}
							else if ( readedCharacter == 'n' || readedCharacter == 'N' )
							{
								tokenType = TokenType.Null;
								stream.Seek ( -1, SeekOrigin.Current );
							}
							else if ( readedCharacter >= '0' &&
								readedCharacter <= '9' )
							{
								tokenType = TokenType.Integer;
								stream.Seek ( -1, SeekOrigin.Current );
							}
							automataToken = new StringBuilder ();
						}
						break;
					case TokenType.String:
						{
							if ( readedCharacter == '"' )
							{
								tokenStack.Push ( automataToken.ToString () );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							else if ( readedCharacter == '\\' )
							{
								readedCharacter = br.ReadChar ();
								if ( readedCharacter == 'n' )
									automataToken.Append ( '\n' );
								else
									automataToken.Append ( readedCharacter );
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.Boolean:
						{
							automataToken.Append ( readedCharacter );
							if ( automataToken.ToString ().ToLower () == "true" ||
								automataToken.ToString ().ToLower () == "false" )
							{
								tokenStack.Push ( bool.Parse ( automataToken.ToString () ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
						}
						break;
					case TokenType.Null:
						{
							automataToken.Append ( readedCharacter );
							if ( automataToken.ToString ().ToLower () == "null" )
							{
								tokenStack.Push ( null );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
						}
						break;
					case TokenType.Integer:
						{
							if ( readedCharacter == '.' )
								tokenType = TokenType.FloatingPoint;
							else if ( !( readedCharacter >= '0' &&
								readedCharacter <= '9' ) )
							{
								stream.Seek ( -1, SeekOrigin.Current );
								tokenStack.Push ( int.Parse ( automataToken.ToString () ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
					case TokenType.FloatingPoint:
						{
							if ( !( readedCharacter >= '0' &&
								   readedCharacter <= '9' ) )
							{
								stream.Seek ( -1, SeekOrigin.Current );
								tokenStack.Push ( double.Parse ( automataToken.ToString () ) );
								tokenType = TokenType.Unknown;
								automataToken = null;
								break;
							}
							automataToken.Append ( readedCharacter );
						}
						break;
				}
			}

			return jsonEntry;
		}

		/// <summary>
		/// Parse Json string
		/// </summary>
		/// <param name="jsonString">Json String</param>
		/// <returns>Parsed Json Entry object</returns>
		public static JsonEntry Parse ( string jsonString )
		{
			using ( MemoryStream stream = new MemoryStream ( Encoding.UTF8.GetBytes ( jsonString.Trim () ) ) )
			{
				return Parse ( stream );
			}
		}
	}
}
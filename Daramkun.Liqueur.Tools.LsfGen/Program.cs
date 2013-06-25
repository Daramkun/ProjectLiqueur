using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Tools.LsfGen
{
	[Flags]
	enum Charset : uint
	{
		Unknown = 0,
		EnglishAlphabet = 1 << 0,
		AsciiSpecialChars = 1 << 1,
		Numbers = 1 << 2,

		Hangul = 1 << 3,
		UnicodeSpecialChars = 1 << 5,
		JapChars = 1 << 4,
		LatinAdditionalChars = 1 << 6,
		AsianHanja = 1 << 7,
	}

	class Program
	{
		static readonly Color FontColor = Color.White, BackgroundColor = Color.Transparent;

		#region ASCII
		static string GetEnglishAlphabets ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.EnglishAlphabet ) )
				return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
			else return "";
		}

		static string GetAsciiSpecialCharacters ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.AsciiSpecialChars ) )
				return "`-=\\][;',./~!@#$%^&*()_+{}|:\"<>?";
			else return "";
		}

		static string GetNumbers ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.Numbers ) )
				return "0123456789";
			else return "";
		}
		#endregion

		#region UNICODE
		static string GetCompletionHangulCharacters ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.Hangul ) )
			{
				StringBuilder stringBuilder = new StringBuilder ();
				for ( int i = 0xAC00; i <= 0xD7AF; i++ )
					stringBuilder.Append ( ( char ) i );
				return stringBuilder.ToString ();
			}
			else return "";
		}

		static string GetUnicodeSpecialMarks ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.UnicodeSpecialChars ) )
			{
				StringBuilder stringBuilder = new StringBuilder ();
				for ( int i = 0x2460; i <= 0x24FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x3000; i <= 0x303F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x3200; i <= 0x32FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x3300; i <= 0x33FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2580; i <= 0x259F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2500; i <= 0x257F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x25A0; i <= 0x25FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2600; i <= 0x26FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2190; i <= 0x21FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2200; i <= 0x22FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2300; i <= 0x23FF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x20A0; i <= 0x20CF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2100; i <= 0x214F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2150; i <= 0x218F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2000; i <= 0x206F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x2070; i <= 0x209F; i++ )
					stringBuilder.Append ( ( char ) i );
				return stringBuilder.ToString ();
			}
			else return "";
		}

		static string GetHirakanaKatakana ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.JapChars ) )
			{
				StringBuilder stringBuilder = new StringBuilder ();
				for ( int i = 0x3040; i <= 0x309F; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0x30A0; i <= 0x30FF; i++ )
					stringBuilder.Append ( ( char ) i );
				return stringBuilder.ToString ();
			}
			else return "";
		}

		static string GetLatinAlphabets ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.LatinAdditionalChars ) )
			{
				StringBuilder stringBuilder = new StringBuilder ();
				for ( int i = 0x0100; i <= 0x017F; i++ )
					stringBuilder.Append ( ( char ) i );
				return stringBuilder.ToString ();
			}
			else return "";
		}

		static string GetAsianHanja ( Charset charset )
		{
			if ( charset.HasFlag ( Charset.AsianHanja ) )
			{
				StringBuilder stringBuilder = new StringBuilder ();
				for ( int i = 0x4E00; i <= 0x9FBF; i++ )
					stringBuilder.Append ( ( char ) i );
				for ( int i = 0xF900; i <= 0xFAFF; i++ )
					stringBuilder.Append ( ( char ) i );
				return stringBuilder.ToString ();
			}
			else return "";
		}
		#endregion

		#region Utilities
		static Size GetFontSize ( Graphics tempGraphics, Font font, string testString )
		{
			SizeF imageSize;
			imageSize = tempGraphics.MeasureString ( testString, font );
			return new Size ( ( int ) imageSize.Width, ( int ) imageSize.Height );
		}

		static Point GetBoundingBox ( Bitmap charBitmap )
		{
			Point calced = new Point ();
			bool leftCalc = true, rightCalc = true;

			for ( int x = 0; x < charBitmap.Width / 2; x++ )
			{
				if ( leftCalc )
				{
					for ( int y = 0; y < charBitmap.Height; y++ )
					{
						if ( charBitmap.GetPixel ( x, y ).A != 0 )
						{
							calced.X = x - 1;
							leftCalc = false;
							break;
						}
					}
				}
				if ( rightCalc )
				{
					for ( int y = 0; y < charBitmap.Height; y++ )
					{
						if ( charBitmap.GetPixel ( charBitmap.Width - x - 1, y ).A != 0 )
						{
							calced.Y = x - 1;
							rightCalc = false;
							break;
						}
					}
				}
			}

			return calced;
		}

		static Bitmap GetCharBitmap ( Graphics tempGraphics, Font font, Brush whiteBrush, bool antialias, char ch, Color backgroundColor )
		{
			Size imageSize = GetFontSize ( tempGraphics, font, "" + ch );
			
			Bitmap charBitmap = new Bitmap ( imageSize.Width, imageSize.Height );
			Graphics charGraphics = Graphics.FromImage ( charBitmap );
			
			charGraphics.Clear ( backgroundColor );
			
			if ( antialias )
				charGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			else
				charGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
			
			charGraphics.DrawString ( String.Format ( "{0}", ch ),
				font, whiteBrush, new Rectangle ( 0, 0, imageSize.Width, imageSize.Height ),
				new StringFormat () { Alignment = StringAlignment.Center } );

			if ( ch == ' ' || ch == '	' || ch == '　' ) return charBitmap;
			
			Point measure = GetBoundingBox ( charBitmap );
			charGraphics.Dispose ();
			int cbw = charBitmap.Width, cbh = charBitmap.Height;
			charBitmap.Dispose ();

			charBitmap = new Bitmap ( cbw - ( measure.X + measure.Y ), cbh );
			charGraphics = Graphics.FromImage ( charBitmap );

			charGraphics.Clear ( Color.Transparent );

			if ( antialias )
				charGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			else
				charGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

			charGraphics.DrawString ( String.Format ( "{0}", ch ),
				font, whiteBrush, new Rectangle ( -measure.X, 0, imageSize.Width, imageSize.Height ),
				new StringFormat () { Alignment = StringAlignment.Center } );

			return charBitmap;
		}

		static void AddFileToZipArchive ( ZipArchive archive, string filename, Stream data )
		{
			ZipArchiveEntry entry = archive.CreateEntry ( filename );
			using ( Stream stream = entry.Open () )
			{
				byte [] buffer = new byte [ data.Length ];
				data.Read ( buffer, 0, ( int ) data.Length );
				stream.Write ( buffer, 0, ( int ) data.Length );
			}
			data.Dispose ();
		}
		#endregion

		static void Main ( string [] args )
		{
			if ( args.Length == 0 || args [ 0 ] == "-h" || args.Length != 7 )
			{
				#region Display help
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  lsfgen.exe dest fontname fontsize -[z|i] -[b|p] -[a|g] -[charset] chars" );
				Console.WriteLine ( "===============================================================================" );
				Console.WriteLine ( "          dest:     dest - lsf filename or foldername for save images" );
				Console.WriteLine ( "      fontname: fontname - fontname" );
				Console.WriteLine ( "      fontsize: fontsize - fontsize (integer)" );
				Console.WriteLine ( "      -[z|l|i]:        z - Save to Zip Liqueur Sprite Font File" );
				//Console.WriteLine ( "                       l - Save to Liqueur Sprite Font File" );
				Console.WriteLine ( "                       i - Save to images to folder" );
				Console.WriteLine ( "        -[b|p]:        b - Save to 32bpp Bitmap file" );
				Console.WriteLine ( "                       p - Save to Portable network graphics file" );
				Console.WriteLine ( "        -[a|g]:        a - Antialias font image" );
				Console.WriteLine ( "                       g - Gridfit font image" );
				Console.WriteLine ( "    -[charset]:  charset - preset of characters (multiselectable)" );
				Console.WriteLine ( "                           - al - lower/uppercase english alphabets" );
				Console.WriteLine ( "                           - as - ASCII special chars" );
				Console.WriteLine ( "                           - nm - numbers" );
				Console.WriteLine ( "                           - ko - korean completion alphabets" );
				Console.WriteLine ( "                           - us - unicode special chars" );
				Console.WriteLine ( "                           - ja - japanese hira/katakana alphabets" );
				Console.WriteLine ( "                           - la - latin additional alphabets" );
				Console.WriteLine ( "                           - ha - asian hanja(kanji) characters" );
				Console.WriteLine ( "         chars:    chars - other chars for generating image font" );
				#endregion
			}
			else
			{
				#region Arguments
				string destination = args [ 0 ];
				string fontname = args [ 1 ];
				string fontsize = args [ 2 ];
				string filetype = args [ 3 ];
				string formattype = args [ 4 ];
				string antialias = args [ 5 ];
				string charset = args [ 6 ];
				string chars = "? " + ( ( args.Length == 8 ) ? args [ 7 ] : "" );
				#endregion

				Console.WriteLine ( "Program Argument Analyzing..." );

				#region Argument Analyzing
				bool isBitmap = formattype == "-b";
				bool isAntialias = antialias == "-a";
				Charset processedCharset = Charset.Unknown;
				foreach ( Match charsetMatch in Regex.Matches ( charset, "[a-z][a-z]" ) )
				{
					switch ( charsetMatch.Value )
					{
						case "al": processedCharset |= Charset.EnglishAlphabet; break;
						case "as": processedCharset |= Charset.AsciiSpecialChars; break;
						case "nm": processedCharset |= Charset.Numbers; break;
						case "ko": processedCharset |= Charset.Hangul; break;
						case "us": processedCharset |= Charset.UnicodeSpecialChars; break;
						case "ja": processedCharset |= Charset.JapChars; break;
						case "la": processedCharset |= Charset.LatinAdditionalChars; break;
						case "ha": processedCharset |= Charset.AsianHanja; break;
					}
				}
				#endregion

				Console.WriteLine ( "Make character set list..." );

				#region Make char set list
				chars += GetEnglishAlphabets ( processedCharset ) + GetAsciiSpecialCharacters ( processedCharset ) +
					GetNumbers ( processedCharset ) + GetCompletionHangulCharacters ( processedCharset ) +
					GetUnicodeSpecialMarks ( processedCharset ) + GetHirakanaKatakana ( processedCharset ) +
					GetLatinAlphabets ( processedCharset ) + GetAsianHanja ( processedCharset );
				#endregion

				Console.WriteLine ( "Initializing make font image..." );

				#region Initialize font image
				Font font = new Font ( fontname, int.Parse ( fontsize ) );
				SolidBrush whiteBrush = new SolidBrush ( FontColor );
				Bitmap tempBitmap = new Bitmap ( 1, 1 );
				Graphics tempGraphics = Graphics.FromImage ( tempBitmap );
				Dictionary<char, Bitmap> charBitmapDictionary = new Dictionary<char, Bitmap> ();
				#endregion

				Console.WriteLine ( "Making font image..." );

				#region Make Font Image
				foreach ( char ch in chars.AsParallel () )
				{
					Bitmap bitmap = null;
					try
					{
						bitmap = GetCharBitmap ( tempGraphics, font, whiteBrush, isAntialias, ch, BackgroundColor );
						if ( charBitmapDictionary.ContainsKey ( ch ) ) continue;
						charBitmapDictionary.Add ( ch, bitmap );
					}
					catch ( ArgumentException ex )
					{
						if ( bitmap != null ) bitmap.Dispose ();
						Console.WriteLine ( "{0} character has invalid image.\n{1}", ch, ex );
					}
				};
				#endregion

				#region Finalize font image
				tempGraphics.Dispose ();
				tempBitmap.Dispose ();
				#endregion

				Console.WriteLine ( "Completed make font image..." );

				Console.WriteLine ( "Saving Liqueur Sprite Font file..." );

				#region Save to ZipLsf
				if ( filetype == "-z" )
				{
					string fontInformation = String.Format ( "{{ \"fontfamily\" : \"{0}\", \"fontsize\" : {1} }}",
						fontname, fontsize );
					using ( FileStream fileStream = new FileStream ( destination, FileMode.Create ) )
					{
						using ( ZipArchive archive = new ZipArchive ( fileStream, ZipArchiveMode.Create, false, Encoding.UTF8 ) )
						{
							AddFileToZipArchive ( archive, "info.json", new MemoryStream ( Encoding.UTF8.GetBytes ( fontInformation ) ) );
							foreach ( KeyValuePair<char, Bitmap> ch in charBitmapDictionary )
							{
								MemoryStream tempStream = new MemoryStream ();
								ch.Value.Save ( tempStream, isBitmap ? ImageFormat.Bmp : ImageFormat.Png );
								tempStream.Position = 0;
								AddFileToZipArchive ( archive, String.Format ( "{0}.{1}", ( int ) ch.Key, isBitmap ? "bmp" : "png" ),
									tempStream );
								tempStream.Dispose ();
							}
						}
					}
				}
				#endregion
				/*
				#region Save to Lsf
				else if ( filetype == "-l" )
				{
					using ( FileStream fileStream = new FileStream ( destination, FileMode.Create ) )
					{
						BinaryWriter header = new BinaryWriter ( fileStream );
						header.Write ( ( int ) 0xDEAD );
						using ( DeflateStream ds = new DeflateStream ( fileStream, CompressionMode.Compress ) )
						{
							using ( BinaryWriter bw = new BinaryWriter ( ds ) )
							{
								bw.Write ( fontname );
								bw.Write ( int.Parse ( fontsize ) );
								bw.Write ( charBitmapDictionary.Count );
								foreach ( KeyValuePair<char, Bitmap> v in charBitmapDictionary )
								{
									bw.Write ( v.Key );
									MemoryStream mem = new MemoryStream ();
									v.Value.Save ( mem, isBitmap ? ImageFormat.Bmp : ImageFormat.Png );
									mem.Position = 0;
									bw.Write ( ( int ) mem.Length );
									bw.Write ( mem.ToArray () );
									v.Value.Dispose ();
									mem.Dispose ();
								}
							}
						}
					}
				}
				#endregion
				*/
				#region Save to image files
				else
				{
					foreach ( KeyValuePair<char, Bitmap> ch in charBitmapDictionary )
					{
						using ( FileStream fileStream = new FileStream ( Path.Combine ( destination,
							String.Format ( "{0}.{1}", ( int ) ch.Key, isBitmap ? "bmp" : "png" ) ),
							FileMode.Create ) )
						{
							ch.Value.Save ( fileStream, isBitmap ? ImageFormat.Bmp : ImageFormat.Png );
						}
					}
				}
				#endregion

				Console.WriteLine ( "Complete." );
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Daramkun.Liqueur.Logging.LogWriters;

namespace Daramkun.Liqueur.Logging
{
	public static class Logger
	{
		public static MessageFormat MessageFormat { get; set; }
		public static IEnumerable<ILogWriter> LogWriters { get; private set; }

		public static LogLevel LogLevel { get; set; }

		static Logger ()
		{
			LogWriters = new List<ILogWriter> ();
			LogLevel = Logging.LogLevel.Level5;
		}

		public static void AddDefaultLogWriter ()
		{
			AddLogWriter ( new DebugLogWriter () );
		}

		public static void AddLogWriter ( ILogWriter logWriter )
		{
			( LogWriters as List<ILogWriter> ).Add ( logWriter );
		}

		private static bool HasFlag ( MessageFormat messageFormat )
		{
			return ( ( MessageFormat & messageFormat ) != 0 );
		}

		public static void Write ( LogLevel level, string message, params object [] args )
		{
			if ( level == LogLevel.None ) throw new ArgumentException ();
			if ( level > LogLevel ) return;

			StringBuilder builder = new StringBuilder ();

			if ( HasFlag ( MessageFormat.LogLevel ) )
				builder.Append ( String.Format ( "[{0}]", level ) );
			if ( HasFlag ( MessageFormat.CultureName ) )
				builder.Append ( String.Format ( "[{0}]", LiqueurSystem.CurrentCulture.Name ) );
			if ( HasFlag ( MessageFormat.Thread ) )
				builder.Append ( String.Format ( "[{0}]", new IntPtr ( Thread.CurrentThread.ManagedThreadId ) ) );
			if ( HasFlag ( MessageFormat.DateTime ) )
				builder.Append ( String.Format ( "[{0}]", DateTime.UtcNow.ToString ( CultureInfo.InvariantCulture.DateTimeFormat ) ) );
			if ( HasFlag ( MessageFormat.Message ) )
				builder.Append ( String.Format ( message, args ) );

			foreach ( ILogWriter logWriter in LogWriters )
				logWriter.WriteLog ( builder.ToString () );
		}
	}
}
namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal abstract class PngChunkTextVar : PngChunkMultiple
	{
		protected internal String key;
		protected internal String val;

		protected internal PngChunkTextVar ( String id, ImageInfo info )
			: base ( id, info )
		{
		}

		public const String KEY_Title = "Title";
		public const String KEY_Author = "Author";
		public const String KEY_Description = "Description";
		public const String KEY_Copyright = "Copyright";
		public const String KEY_Creation_Time = "Creation Time";
		public const String KEY_Software = "Software";
		public const String KEY_Disclaimer = "Disclaimer";
		public const String KEY_Warning = "Warning";
		public const String KEY_Source = "Source";
		public const String KEY_Comment = "Comment";

		internal class PngTxtInfo
		{/*
			public String title;
			public String author;
			public String description;
			public String creation_time;
			public String software;
			public String disclaimer;
			public String warning;
			public String source;
			public String comment;*/
		}

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.NONE;
		}

		public String GetKey ()
		{
			return key;
		}

		public String GetVal ()
		{
			return val;
		}

		public void SetKeyVal ( String key, String val )
		{
			this.key = key;
			this.val = val;
		}
	}
}

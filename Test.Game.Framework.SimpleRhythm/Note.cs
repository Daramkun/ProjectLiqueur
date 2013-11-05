using System;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur;

namespace Test.Game.Framework.SimpleRhythm
{
	public class Note : Node
	{
		public static event Action<Note> NoteUpdater;
		public static event Action<Note> NoteDrawer;

		public TimeSpan NoteTime { get; private set; }
		public int NoteType { get; set; }
		public IAudio NoteSound { get; set; }
		public object ExtendData { get; set; }

		public Note ( TimeSpan noteTime, int noteType = 0, object extendData = null, AudioInfo noteSound = null )
		{
			NoteTime = noteTime;
			NoteType = noteType;
			NoteSound = LiqueurSystem.AudioDevice.CreateAudio ( noteSound );
			ExtendData = extendData;
		}

		public override void Update ( GameTime gameTime ) 
		{
			if ( NoteUpdater != null )
				NoteUpdater ( this );
			base.Update (gameTime);
		}

		public override void Draw ( GameTime gameTime )
		{
			if ( NoteDrawer != null )
				NoteDrawer ( this );
			base.Draw (gameTime);
		}
	}
}


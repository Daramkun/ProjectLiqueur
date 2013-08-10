using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Platforms;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Daramkun.Liqueur.Audio
{
	class AudioDevice : IAudioDevice
	{
		internal AudioContext audioContext;
		internal List<IAudio> audioList;

		public AudioDevice ( IWindow window )
		{
			audioContext = new AudioContext ( AudioContext.DefaultDevice );
			audioList = new List<IAudio> ();
		}

		~AudioDevice ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				audioContext.Dispose ();
				audioContext = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void Update ()
		{
			foreach ( IAudio audio in audioList.ToArray () )
				( audio as Audio ).Update ();
		}

		public void Play ( IAudio audio )
		{
			( audio as Audio ).isPlaying = true;
			AL.SourcePlay ( ( audio as Audio ).sourceId );
		}

		public void Pause ( IAudio audio )
		{
			( audio as Audio ).isPlaying = false;
			AL.SourcePause ( ( audio as Audio ).sourceId );
		}

		public void Stop ( IAudio audio )
		{
			( audio as Audio ).isPlaying = false;
			AL.SourceStop ( ( audio as Audio ).sourceId );
		}

		public IAudio CreateAudio ( AudioInfo audioInfo )
		{
			return new Audio ( this, audioInfo );
		}
	}
}

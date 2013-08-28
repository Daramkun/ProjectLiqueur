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

		public object Handle { get { return audioContext; } }

		public AudioDevice ( IWindow window )
		{
			IList<string> availableDevices;
			try
			{
				availableDevices = AudioContext.AvailableDevices;
			}
			catch (AudioContextException e)
			{
				throw new PlatformNotSupportedException ( string.Format (
					"Audio device is not available for OpenAL: {0}", e.Message ) );
			}
			foreach ( string device in availableDevices )
			{
				try
				{
					audioContext = new AudioContext ( device );
				}
				catch
				{
					audioContext = null;
					continue;
				}
				break;
			}
			if ( audioContext == null )
				throw new PlatformNotSupportedException ( "Audio device is not available for OpenAL." );
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
				audio.Update ();
		}

		public void Play ( IAudio audio )
		{
			( audio as Audio ).isPlaying = true;
			AL.SourcePlay ( ( int ) audio.Handle );
		}

		public void Pause ( IAudio audio )
		{
			( audio as Audio ).isPlaying = false;
			AL.SourcePause ( ( int ) audio.Handle );
		}

		public void Stop ( IAudio audio )
		{
			( audio as Audio ).isPlaying = false;
			AL.SourceStop ( ( int ) audio.Handle );
		}

		public IAudio CreateAudio ( AudioInfo audioInfo )
		{
			return new Audio ( this, audioInfo );
		}
	}
}

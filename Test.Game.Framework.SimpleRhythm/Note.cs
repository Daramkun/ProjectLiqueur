using System;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur;

namespace Test.Game.Framework.SimpleRhythm
{
	public enum Difficulty
	{
		Easy,
		Normal,
		Hard,
	}

	#region Verdict Levels
	enum VerdictLevel1
	{
		Fail,
		Hit,
	}

	enum VerdictLevel2
	{
		Fail,
		Great,
		Cool,
	}

	enum VerdictLevel3
	{
		Fail,
		Good,
		Great,
		Cool,
	}

	enum VerdictLevel4
	{
		Fail,
		Miss,
		Good,
		Great,
		Cool,
	}

	enum VerdictLevel5
	{
		Fail,
		Miss,
		Well,
		Good,
		Great,
		Cool,
	}

	enum VerdictLevel6
	{
		Fail,
		Miss,
		Well,
		Soso,
		Good,
		Great,
		Cool,
	}

	enum VerdictLevel7
	{
		Fail,
		Miss,
		Well,
		Soso,
		Good,
		Great,
		Cool,
		Fantastic,
	}

	enum VerdictLevel8
	{
		Fail,
		Miss,
		Well,
		Soso,
		Good,
		Great,
		Cool,
		Fantastic,
		Bravo,
	}

	enum VerdictLevel9
	{
		Fail,
		Miss,
		Well,
		Soso,
		Good,
		Great,
		Cool,
		Fantastic,
		Bravo,
		God,
	}

	enum VerdictLevel10
	{
		_000,
		_010,
		_020,
		_030,
		_040,
		_050,
		_060,
		_070,
		_080,
		_090,
		_100,
	}
	#endregion

	public class Note : Node
	{
		private static Difficulty difficulty = Difficulty.Normal;
		private static int maximumVerdictLevel = 5;

		public static Difficulty Difficulty { get { return difficulty; } set { difficulty = value; } }
		public static int MaximumVerdictLevel
		{
			get { return maximumVerdictLevel; }
			set
			{
				if ( maximumVerdictLevel < 1 || maximumVerdictLevel > 10 )
					throw new ArgumentException ( "Maximum Verdict Level must have '1' ~ '10'." );
				maximumVerdictLevel = value;
			}
		}

		public int TryVerdict ( TimeSpan audioPosition )
		{
			float forCheck = ( float ) ( Position - audioPosition ).TotalMilliseconds;

			if ( forCheck <= -0.5f ) return 0;
			else if ( forCheck >= 1 ) return -1;

			float [] currentPositionWeight = GenerateWeightValue ( audioPosition );
			for ( int i = maximumVerdictLevel - 1; i >= 0; --i )
			{
				if ( currentPositionWeight [ i ] >= Position.TotalMilliseconds ||
					currentPositionWeight [ maximumVerdictLevel - i ] <= Position.TotalMilliseconds )
					return i;
			}

			return -1;
		}

		private float [] GenerateWeightValue ( TimeSpan audioPosition )
		{
			int [] weight = weights [ ( int ) difficulty ] [ maximumVerdictLevel - 1 ];
			float [] weightValue = new float [ maximumVerdictLevel * 2 ];
			for ( int i = 0; i < maximumVerdictLevel; ++i )
				weightValue [ i ] = ( float ) Position.TotalMilliseconds + ( weight [ i ] / 100.0f * 0.5f );
			for ( int i = maximumVerdictLevel; i <= maximumVerdictLevel * 2; ++i )
				weightValue [ i ] = ( float ) Position.TotalMilliseconds + weight [ i - maximumVerdictLevel ] / 100.0f;
			return weightValue;
		}

		#region Verdict Weight Table
		private static int [] [] [] weights = new int [ 3 ] [][];

		static Note ()
		{
			weights [ ( int ) Difficulty.Easy ] = new int [] []
			{
				new int [] { 40, 60, },
				new int [] { 20, 30, 50, },
				new int [] { 10, 20, 30, 40, },
				new int [] { 10, 15, 20, 25, 30, },
				new int [] { 6, 10, 15, 19, 23, 27, },
				new int [] { 8, 10, 12, 14, 16, 19, 21, },
				new int [] { 6, 8, 10, 12, 13, 15, 17, 19, },
				new int [] { 4, 5, 6, 8, 10, 13, 16, 18, 20, },
				new int [] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, },
			};
			weights [ ( int ) Difficulty.Normal ] = new int [] []
			{
				new int [] { 50, 50, },
				new int [] { 33, 34, 33, },
				new int [] { 25, 25, 25, 25 },
				new int [] { 20, 20, 20, 20, 20, },
				new int [] { 16, 17, 17, 17, 17, 16, },
				new int [] { 14, 14, 15, 15, 14, 14, 14, },
				new int [] { 12, 12, 13, 13, 13, 13, 12, 12, },
				new int [] { 11, 11, 11, 11, 12, 11, 11, 11, 11, },
				new int [] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, },
			};
			weights [ ( int ) Difficulty.Hard ] = new int [] []
			{
				new int [] { 60, 40, },
				new int [] { 50, 30, 20, },
				new int [] { 40, 30, 20, 10, },
				new int [] { 30, 25, 20, 15, 10, },
				new int [] { 27, 23, 19, 15, 10, 6, },
				new int [] { 21, 19, 16, 14, 12, 10, 8, },
				new int [] { 19, 17, 15, 13, 12, 10, 8, 6, },
				new int [] { 20, 18, 16, 13, 10, 8, 6, 5, 4, },
				new int [] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, },
			};
		}
		#endregion

		public static event Action<Note> NoteUpdater;
		public static event Action<Note> NoteDrawer;

		public TimeSpan Position { get; private set; }
		public int NoteType { get; set; }
		public IAudio NoteSound { get; set; }
		public object ExtendData { get; set; }

		public Note ( TimeSpan noteTime, int noteType = 0, object extendData = null, AudioInfo? noteSound = null )
		{
			Position = noteTime;
			NoteType = noteType;
			if ( noteSound != null )
				NoteSound = LiqueurSystem.AudioDevice.CreateAudio ( noteSound.Value );
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


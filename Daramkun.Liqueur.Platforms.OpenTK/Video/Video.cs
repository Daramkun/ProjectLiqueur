using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Video
{
	public sealed class Video : IVideo
	{
		VideoInfo videoInfo;
		ITexture2D texture;

		public ITexture2D Texture
		{
			get
			{
				texture.Buffer = videoInfo.GetImage ( Audio.Position ).GetPixel ( null );
				return texture;
			}
		}

		public IAudio Audio { get; private set; }

		public Video ( VideoInfo videoInfo )
		{
			this.videoInfo = videoInfo;
			Audio = LiqueurSystem.AudioDevice.CreateAudio ( videoInfo.GetAudio () );
			texture = LiqueurSystem.GraphicsDevice.CreateTexture2D ( videoInfo.Width, videoInfo.Height );
		}

		public void Dispose ()
		{
			Audio.Dispose ();
		}

		public bool Update ()
		{
			return Audio.Update ();
		}
	}
}

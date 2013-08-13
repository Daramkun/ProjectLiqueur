using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Video
{
	public interface IVideo
	{
		ITexture2D Texture { get; }
		IAudio Audio { get; }

		bool Update ();
	}
}

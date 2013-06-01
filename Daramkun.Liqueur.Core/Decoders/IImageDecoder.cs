using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Decoders
{
	/// <summary>
	/// 이미지 디코더 인터페이스
	/// </summary>
	public interface IImageDecoder : IDecoder<ImageData>
	{
		Color [] GetPixels ( ImageData imageData, Color colorKey );
	}
}

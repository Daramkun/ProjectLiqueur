using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Decoders;

namespace Daramkun.Liqueur.Graphics
{
	/// <summary>
	/// 이미지 데이터
	/// </summary>
	public struct ImageData
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public object Data { get; set; }
		public Stream ImageStream { get; internal set; }
		public IImageDecoder ImageDecoder { get; set; }
	}
}

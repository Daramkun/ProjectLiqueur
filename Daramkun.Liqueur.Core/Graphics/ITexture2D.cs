using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Graphics
{
	public enum TextureFilter
	{
		Nearest,
		Linear,
		Anisotropic,
	}

	public enum TextureAddressing
	{
		Wrap,
		Mirror,
		Clamp,
	}

	public struct TextureArgument
	{
		public string Uniform { get; set; }
		public ITexture2D Texture { get; set; }
		public TextureFilter Filter { get; set; }
		public TextureAddressing Addressing { get; set; }
		public int AnisotropicLevel { get; set; }
	}

	public interface ITexture2D : IDisposable
	{
		int Width { get; }
		int Height { get; }
		Vector2 Size { get; }

		Color [] Buffer { get; set; }

		object Handle { get; }
	}
}

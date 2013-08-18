using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Graphics
{
	[Flags]
	public enum ShaderType
	{
		Unknown = 0,

		VertexShader = 1 << 0,

		PixelShader = 1 << 1,
		FragmentShader = PixelShader,

		GeometryShader = 1 << 2,
	}

	public interface IShader : IDisposable
	{
		ShaderType ShaderType { get; }

		object Handle { get; }

		void Attach ( IEffect effect );
		void Detach ( IEffect effect );
	}

	public interface IEffect : IDisposable
	{
		object Handle { get; }

		void Dispatch ( Action<IEffect> dispatchEvent );

		T GetArgument<T> ( string parameter );
		void SetArgument<T> ( string parameter, T argument );
		void SetTextures ( params TextureArgument [] textures );
	}
}

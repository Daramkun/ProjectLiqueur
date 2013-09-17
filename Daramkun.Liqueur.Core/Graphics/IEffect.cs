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

		//GeometryShader = 1 << 2,
	}

	public class ShaderOption
	{
		public struct AttributeOrder { public string Name { get; set; } public FlexibleVertexFormat VertexType { get; set; } }
		public AttributeOrder [] AttributeOrdering { get; set; }
	}

	public interface IShader : IDisposable
	{
		ShaderType ShaderType { get; }
		ShaderOption Option { get; set; }

		object Handle { get; }
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

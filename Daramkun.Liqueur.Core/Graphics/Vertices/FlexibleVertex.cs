using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics.Vertices
{
	public interface IFlexibleVertex { }
	public interface IFlexibleVertexPositionXY : IFlexibleVertex { Vector2 Position { get; set; } }
	public interface IFlexibleVertexPositionXYZ : IFlexibleVertex { Vector3 Position { get; set; } }
	public interface IFlexibleVertexDiffuse : IFlexibleVertex { Color Diffuse { get; set; } }
	public interface IFlexibleVertexNormal : IFlexibleVertex { Vector3 Normal { get; set; } }
	public interface IFlexibleVertexTexture0 : IFlexibleVertex { Vector2 TextureUV0 { get; set; } }
	public interface IFlexibleVertexTexture1 : IFlexibleVertex { Vector2 TextureUV1 { get; set; } }
	public interface IFlexibleVertexTexture2 : IFlexibleVertex { Vector2 TextureUV2 { get; set; } }
	public interface IFlexibleVertexTexture3 : IFlexibleVertex { Vector2 TextureUV3 { get; set; } }
}

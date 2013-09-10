using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Graphics
{
	#region Render mode
	public enum CullingMode
	{
		None,
		ClockWise,
		CounterClockWise,
	}

	public enum FillMode
	{
		Point,
		Wireframe,
		Solid,
	}
	
	public enum PrimitiveType
	{
		PointList,
		LineList,
		LineStrip,
		TriangleList,
		TriangleStrip,
		TriangleFan,
	}
	#endregion

	[Flags]
	public enum BufferFormat
	{
		Unknown = 0,

		ColorBuffer24Bit = 1 << 0,
		ColorBuffer32Bit = 1 << 1,
		
		DepthBuffer16Bit = 1 << 2,
		DepthBuffer24Bit = 1 << 3,
		DepthBuffer32Bit = 1 << 4,

		StencilBuffer0Bit = 0,
		StencilBuffer4Bit = 1 << 5,
		StencilBuffer8Bit = 1 << 6,
	}

	[Flags]
	public enum ClearBuffer
	{ 
		ColorBuffer = 1 << 0,
		DepthBuffer = 1 << 1,
		StencilBuffer = 1 << 2,
		AllBuffer = ColorBuffer | DepthBuffer | StencilBuffer,
	}

	#region Viewport
	public struct Viewport
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public override string ToString ()
		{
			return string.Format ( "{{X:{0}, Y:{1}, Width:{2}, Height:{3}}}", X, Y, Width, Height );
		}
	}
	#endregion

	public enum BaseRenderer
	{
		Unknown,

		DirectX,
		XNA,
		MonoGame,

		OpenGL,
		OpenGLES,
	}

	public interface IGraphicsDevice : IDisposable
	{
		object Handle { get; }

		BaseRenderer BaseRenderer { get; }
		Version RendererVersion { get; }

		Vector2 [] AvailableScreenSize { get; }

		Vector2 ScreenSize { get; set; }
		bool FullscreenMode { get; set; }
		bool VerticalSyncMode { get; set; }

		CullingMode CullingMode { get; set; }
		FillMode FillMode { get; set; }
		//BufferFormat BufferFormat { get; set; }
		Viewport Viewport { get; set; }

		bool IsZWriteEnable { get; set; }
		bool BlendState { get; set; }
		bool StencilState { get; set; }

		BlendOperation BlendOperation { get; set; }
		StencilOperation StencilOperation { get; set; }

		IRenderBuffer RenderTarget { get; set; }

		void BeginScene ();
		void EndScene ();

		void Clear ( ClearBuffer clearBuffer, Color color );
		void SwapBuffer ();

		void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer ) where T : struct;
		void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer ) where T : struct;

		ITexture2D CreateTexture2D ( int width, int height );
		ITexture2D CreateTexture2D ( ImageInfo imageInfo, Color? colorKey = null );
		IRenderBuffer CreateRenderBuffer ( int width, int height );

		IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, int vertexCount ) where T : struct;
		IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, T [] vertices ) where T : struct;

		IIndexBuffer CreateIndexBuffer ( int indexCount );
		IIndexBuffer CreateIndexBuffer ( int [] indices );

		IShader CreateShader ( Stream stream, ShaderType shaderType );
		IShader CreateShader ( string code, ShaderType shaderType );

		IEffect CreateEffect ( params IShader [] shaders );
	}
}

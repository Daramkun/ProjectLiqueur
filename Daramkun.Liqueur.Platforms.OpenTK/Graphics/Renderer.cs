using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Graphics.Vertices;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Graphics
{
	class Renderer : IRenderer
	{
		Window window;
		Vector2 screenSize;

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> screenSizes = new List<Vector2> ();
				foreach ( OpenTK.DisplayResolution resolution in
					OpenTK.DisplayDevice.Default.AvailableResolutions )
					screenSizes.Add ( new Vector2 ( resolution.Width, resolution.Height ) );
				return screenSizes.ToArray ();
			}
		}

		public Vector2 ScreenSize
		{
			get { return screenSize; }
			set
			{
				screenSize = value;
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				window.window.ClientSize = new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y );
				GL.Ortho ( 0, screenSize.X, screenSize.Y, 0, 0.0001f, 1000.0f );
			}
		}

		public bool FullscreenMode
		{
			get
			{
				return window.window.WindowState == OpenTK.WindowState.Fullscreen;
			}
			set
			{
				window.window.WindowState = ( value ) ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;
			}
		}

		CullingMode cullMode = CullingMode.None;
		public CullingMode CullingMode
		{
			get { return cullMode; }
			set
			{
				cullMode = value;
				if ( value == Graphics.CullingMode.None ) GL.Disable ( EnableCap.CullFace );
				else GL.Enable ( EnableCap.CullFace );
				GL.FrontFace ( ( value == CullingMode.ClockWise ) ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw );
			}
		}

		FillMode fillMode = FillMode.Solid;
		public FillMode FillMode
		{
			get { return fillMode; }
			set
			{
				fillMode = value;
				GL.PolygonMode ( MaterialFace.FrontAndBack,
					( value == FillMode.Point ) ? PolygonMode.Point :
					( value == FillMode.Wireframe ) ? PolygonMode.Line : PolygonMode.Fill );
			}
		}

		Viewport viewPort = new Viewport () { X = 0, Y = 0, Width = 800, Height = 600 };
		public Viewport Viewport
		{
			get { return viewPort; }
			set
			{
				viewPort = value;
				GL.Viewport ( value.X, value.Y, value.Width, value.Height );
			}
		}

		public Renderer ( Window window )
		{
			this.window = window;
			GL.MatrixMode ( MatrixMode.Projection );
			GL.LoadIdentity ();
			GL.Ortho ( 0, 800, 600, 0, 0.000f, 1000.0f );
		}

		public void Dispose ()
		{

		}

		public void Begin2D ()
		{
			GL.MatrixMode ( MatrixMode.Modelview );
			GL.LoadIdentity ();

			GL.Enable ( EnableCap.Texture2D );
			GL.Enable ( EnableCap.Blend );

			GL.BlendFunc ( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );

			GL.EnableClientState ( ArrayCap.VertexArray );
			GL.EnableClientState ( ArrayCap.TextureCoordArray );
		}

		public void End2D ()
		{
			GL.DisableClientState ( ArrayCap.TextureCoordArray );
			GL.DisableClientState ( ArrayCap.VertexArray );

			GL.Disable ( EnableCap.Blend );
			GL.Disable ( EnableCap.Texture2D );
		}

		public void Clear ( Color color )
		{
			GL.ClearColor ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar );
			GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );
		}

		public void Present ()
		{
			window.window.SwapBuffers ();
		}

		private BeginMode ConvertPrimitiveMode ( PrimitiveType type )
		{
			switch ( type )
			{
				case PrimitiveType.PointList: return BeginMode.Points;
				case PrimitiveType.LineList: return BeginMode.Lines;
				case PrimitiveType.LineStrip: return BeginMode.LineStrip;
				case PrimitiveType.TriangleList: return BeginMode.Triangles;
				case PrimitiveType.TriangleStrip: return BeginMode.TriangleStrip;
				case PrimitiveType.TriangleFan: return BeginMode.TriangleFan;
				default: throw new Exception ();
			}
		}

		public void DrawPrimitive<T> ( IPrimitive<T> primitive ) where T : IFlexibleVertex
		{
			if ( ( primitive as Primitive<T> ).vertexArray != null )
			{
				GL.EnableClientState ( ArrayCap.VertexArray );
				GL.VertexPointer ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexPositionXY ) ) ? 2 : 3,
					VertexPointerType.Float, 0, ( primitive as Primitive<T> ).vertexArray );
			}
			if ( ( primitive as Primitive<T> ).textureArray != null )
			{
				GL.Enable ( EnableCap.Texture2D );
				GL.BindTexture ( TextureTarget.Texture2D, ( primitive.Texture as Image ).texture );

				GL.EnableClientState ( ArrayCap.TextureCoordArray );
				GL.TexCoordPointer ( 2, TexCoordPointerType.Float, 0, ( primitive as Primitive<T> ).textureArray );
			}
			if ( ( primitive as Primitive<T> ).normalArray != null )
			{
				GL.EnableClientState ( ArrayCap.NormalArray );
				GL.NormalPointer ( NormalPointerType.Float, 0, ( primitive as Primitive<T> ).normalArray );
			}
			if ( ( primitive as Primitive<T> ).colorArray != null )
			{
				GL.EnableClientState ( ArrayCap.ColorArray );
				GL.ColorPointer ( 4, ColorPointerType.Float, 0, ( primitive as Primitive<T> ).colorArray );
			}
			if ( primitive.Indices != null )
			{
				GL.EnableClientState ( ArrayCap.IndexArray );
				GL.IndexPointer ( IndexPointerType.Int, 0, primitive.Indices );
			}

			GL.DrawArrays ( ConvertPrimitiveMode ( primitive.PrimitiveType ), 0, primitive.Vertices.Length );
			{
				GL.DisableClientState ( ArrayCap.IndexArray );
				GL.DisableClientState ( ArrayCap.ColorArray );
				GL.DisableClientState ( ArrayCap.NormalArray );
				GL.DisableClientState ( ArrayCap.TextureCoordArray );
				GL.DisableClientState ( ArrayCap.VertexArray );
				GL.BindTexture ( TextureTarget.Texture2D, 0 );
				GL.Disable ( EnableCap.Texture2D );
			}
		}

		public IImage CreateImage ( ImageData imageData, Color colorKey )
		{
			return new Image ( imageData, colorKey );
		}

		public IPrimitive<T> CreatePrimitive<T> ( int vertexCount, int indexCount ) where T : IFlexibleVertex
		{
			return new Primitive<T> ( vertexCount, indexCount );
		}

		public IPrimitive<T> CreatePrimitive<T> ( T [] vertexArray, int [] indexArray ) where T : IFlexibleVertex
		{
			return new Primitive<T> ( vertexArray, indexArray );
		}
	}
}

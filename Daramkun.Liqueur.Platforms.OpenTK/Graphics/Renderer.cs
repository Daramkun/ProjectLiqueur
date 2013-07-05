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
using Daramkun.Liqueur.Math.Transforms;

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

		public Viewport Viewport
		{
			get
			{
				int [] viewport = new int [ 4 ];
				GL.GetInteger ( GetPName.Viewport, viewport );
				return new Viewport () { X = viewport [ 0 ], Y = viewport [ 1 ],
					Width = viewport [ 2 ], Height = viewport [ 3 ] };
			}
			set
			{
				GL.Viewport ( value.X, value.Y, value.Width, value.Height );
			}
		}

		public bool IsZWriteEnable
		{
			get
			{
				return GL.IsEnabled ( EnableCap.DepthTest );
			}
			set
			{
				if ( value ) GL.Enable ( EnableCap.DepthTest );
				else GL.Disable ( EnableCap.DepthTest );
			}
		}

		public bool BlendState
		{
			get
			{
				return GL.IsEnabled ( EnableCap.Blend );
			}
			set
			{
				if ( value ) GL.Enable ( EnableCap.Blend );
				else GL.Disable ( EnableCap.Blend );
			}
		}

		public bool StencilState
		{
			get
			{
				return GL.IsEnabled ( EnableCap.StencilTest );
			}
			set
			{
				if ( value ) GL.Enable ( EnableCap.StencilTest );
				else GL.Disable ( EnableCap.StencilTest );
			}
		}

		Stencil stencilParameter = new Stencil ();
		public Stencil StencilParameter
		{
			get { return stencilParameter; }
			set
			{
				stencilParameter = value;
				GL.StencilFunc ( ConvertStencilFunction ( value.Function ), value.Reference, value.Mask );
				GL.StencilOp ( ConvertStencilOperation ( value.Fail ), ConvertStencilOperation ( value.ZFail ), ConvertStencilOperation ( value.Pass ) );
			}
		}

		ITransform [] transforms = new ITransform [ 3 ];
		public ITransform this [ TransformType index ]
		{
			get
			{
				return transforms [ ( int ) index ];
			}
			set
			{
				transforms [ ( int ) index ] = value;
				if ( value is IProjection )
				{
					GL.MatrixMode ( MatrixMode.Projection );
					GL.LoadMatrix ( value.Matrix.ToArray () );
					GL.MatrixMode ( MatrixMode.Modelview );
				}
				else if ( value is View )
				{
					GL.MatrixMode ( MatrixMode.Modelview );
					GL.LoadMatrix ( value.Matrix.ToArray () );
				}
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

		public void SetBlendParameter ( BlendParameter sourceParameter, BlendParameter destinationParameter )
		{
			GL.BlendFunc ( ConvertBlendSourceFactor ( sourceParameter ), ConvertBlendDestinationFactor ( destinationParameter ) );
		}

		public void Begin2D ()
		{
			GL.MatrixMode ( MatrixMode.Modelview );
			GL.LoadIdentity ();

			GL.Enable ( EnableCap.Texture2D );

			BlendState = true;
			SetBlendParameter ( BlendParameter.SourceAlpha, BlendParameter.InvertSourceAlpha );

			GL.EnableClientState ( ArrayCap.VertexArray );
			GL.EnableClientState ( ArrayCap.TextureCoordArray );
		}

		public void End2D ()
		{
			GL.DisableClientState ( ArrayCap.TextureCoordArray );
			GL.DisableClientState ( ArrayCap.VertexArray );

			BlendState = false;
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
			if ( ( primitive as Primitive<T> ).textureArray != null && primitive.Texture != null )
			{
				GL.ActiveTexture ( TextureUnit.Texture0 );
				GL.Enable ( EnableCap.Texture2D );
				GL.BindTexture ( TextureTarget.Texture2D, ( primitive.Texture as Texture2D ).texture );
				GL.EnableClientState ( ArrayCap.TextureCoordArray );
				GL.TexCoordPointer ( 2, TexCoordPointerType.Float, 0, ( primitive as Primitive<T> ).textureArray );
			}
			if ( ( primitive as Primitive<T> ).subTextureArray != null && primitive.SubTexture != null )
			{
				GL.ActiveTexture ( TextureUnit.Texture1 );
				GL.Enable ( EnableCap.Texture2D );
				GL.BindTexture ( TextureTarget.Texture2DArray, ( primitive.SubTexture as Texture2D ).texture );
				GL.EnableClientState ( ArrayCap.TextureCoordArray );
				GL.TexCoordPointer ( 2, TexCoordPointerType.Float, 0, ( primitive as Primitive<T> ).subTextureArray );
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

			GL.PushMatrix ();
			GL.MatrixMode ( MatrixMode.Modelview );
			if ( this [ TransformType.World ] != null )
				GL.LoadMatrix ( this [ TransformType.World ].Matrix.ToArray () );
			if ( primitive.Effect != null )
			{
				primitive.Effect.Dispatch ( ( IEffect effect ) =>
				{
					GL.DrawArrays ( ConvertPrimitiveMode ( primitive.PrimitiveType ), 0, primitive.Vertices.Length );
				} );
			}
			GL.PopMatrix ();
			{
				GL.DisableClientState ( ArrayCap.IndexArray );
				GL.DisableClientState ( ArrayCap.ColorArray );
				GL.DisableClientState ( ArrayCap.NormalArray );
				GL.DisableClientState ( ArrayCap.TextureCoordArray );
				GL.DisableClientState ( ArrayCap.VertexArray );
				GL.ActiveTexture ( TextureUnit.Texture1 );
				GL.BindTexture ( TextureTarget.Texture2D, 0 );
				GL.Disable ( EnableCap.Texture2D );
				GL.ActiveTexture ( TextureUnit.Texture0 );
				GL.BindTexture ( TextureTarget.Texture2D, 0 );
				GL.Disable ( EnableCap.Texture2D );
			}
		}

		public ITexture2D CreateImage ( ImageData imageData, Color colorKey )
		{
			return new Texture2D ( imageData, colorKey );
		}

		public IPrimitive<T> CreatePrimitive<T> ( int vertexCount, int indexCount ) where T : IFlexibleVertex
		{
			return new Primitive<T> ( vertexCount, indexCount );
		}

		public IPrimitive<T> CreatePrimitive<T> ( T [] vertexArray, int [] indexArray ) where T : IFlexibleVertex
		{
			return new Primitive<T> ( vertexArray, indexArray );
		}

		private BlendingFactorSrc ConvertBlendSourceFactor ( BlendParameter sourceParameter )
		{
			switch ( sourceParameter )
			{
				case BlendParameter.Zero: return BlendingFactorSrc.Zero;
				case BlendParameter.One: return BlendingFactorSrc.One;
				case BlendParameter.SourceAlpha: return BlendingFactorSrc.SrcAlpha;
				case BlendParameter.InvertSourceAlpha: return BlendingFactorSrc.OneMinusSrcAlpha;
				case BlendParameter.DestinationColor: return BlendingFactorSrc.DstColor;
				case BlendParameter.DestinationAlpha: return BlendingFactorSrc.DstAlpha;
				case BlendParameter.InvertDestinationColor: return BlendingFactorSrc.OneMinusDstColor;
				case BlendParameter.InvertDestinationAlpha: return BlendingFactorSrc.OneMinusDstAlpha;
			}
			throw new ArgumentException ();
		}

		private BlendingFactorDest ConvertBlendDestinationFactor ( BlendParameter destinationParameter )
		{
			switch ( destinationParameter )
			{
				case BlendParameter.Zero: return BlendingFactorDest.Zero;
				case BlendParameter.One: return BlendingFactorDest.One;
				case BlendParameter.SourceColor: return BlendingFactorDest.SrcColor;
				case BlendParameter.SourceAlpha: return BlendingFactorDest.SrcAlpha;
				case BlendParameter.InvertSourceColor: return BlendingFactorDest.OneMinusSrcColor;
				case BlendParameter.InvertSourceAlpha: return BlendingFactorDest.OneMinusSrcAlpha;
				case BlendParameter.DestinationAlpha: return BlendingFactorDest.DstAlpha;
				case BlendParameter.InvertDestinationAlpha: return BlendingFactorDest.OneMinusDstAlpha;
			}
			throw new ArgumentException ();
		}

		private OpenTK.Graphics.OpenGL.StencilFunction ConvertStencilFunction ( StencilFunction stencilFunction )
		{
			switch ( stencilFunction )
			{
				case StencilFunction.Never: return OpenTK.Graphics.OpenGL.StencilFunction.Never;
				case StencilFunction.Equal: return OpenTK.Graphics.OpenGL.StencilFunction.Equal;
				case StencilFunction.Less: return OpenTK.Graphics.OpenGL.StencilFunction.Less;
				case StencilFunction.LessEqual: return OpenTK.Graphics.OpenGL.StencilFunction.Lequal;
				case StencilFunction.Greater: return OpenTK.Graphics.OpenGL.StencilFunction.Greater;
				case StencilFunction.GreaterEqual: return OpenTK.Graphics.OpenGL.StencilFunction.Gequal;
				case StencilFunction.Always: return OpenTK.Graphics.OpenGL.StencilFunction.Always;
				case StencilFunction.NotEqual: return OpenTK.Graphics.OpenGL.StencilFunction.Notequal;
			}
			throw new ArgumentException ();
		}

		private StencilOp ConvertStencilOperation ( StencilOperation stencilOperation )
		{
			switch ( stencilOperation )
			{
				case StencilOperation.Zero: return StencilOp.Zero;
				case StencilOperation.Keep: return StencilOp.Keep;
				case StencilOperation.Replace: return StencilOp.Replace;
				case StencilOperation.Invert: return StencilOp.Invert;
				case StencilOperation.Increase: return StencilOp.Incr;
				case StencilOperation.IncreaseSAT: return StencilOp.IncrWrap;
				case StencilOperation.Decrease: return StencilOp.Decr;
				case StencilOperation.DecreaseSAT: return StencilOp.DecrWrap;
			}
			throw new ArgumentException ();
		}
	}
}

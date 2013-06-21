using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Platforms;
#if OPENTK
using OpenTK.Graphics.OpenGL;
using Daramkun.Liqueur.Graphics.Vertices;
#elif XNA
using Microsoft.Xna.Framework.Graphics;
#endif

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

		CullingMode cullMode = CullingMode.CounterClockWise;
		public CullingMode CullingMode
		{
			get { return cullMode; }
			set
			{
				cullMode = value;
				GL.CullFace ( ( value == CullingMode.None ) ? CullFaceMode.FrontAndBack :
					( value == CullingMode.ClockWise ) ? CullFaceMode.Back : CullFaceMode.Front );
			}
		}

		public Renderer ( Window window )
		{
			this.window = window;
			window.window.Resize += ( object sender, EventArgs e ) =>
			{
				GL.MatrixMode ( MatrixMode.Projection );
				GL.LoadIdentity ();
				GL.Ortho ( 0, 800, 600, 0, 0.001f, 1000.0f );
			};
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
			GL.Clear ( ClearBufferMask.ColorBufferBit );
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

		public void DrawPrimitive<T> ( Primitive<T> primitive )
		{
			GL.Begin ( ConvertPrimitiveMode ( primitive.PrimitiveType ) );
			foreach ( T point in primitive.Vertices )
			{
				if ( point is IFlexibleVertexPositionXY )
				{
					Vector2 position = ( point as IFlexibleVertexPositionXY ).Position;
					GL.Vertex2 ( position.X, position.Y );
				}
				else if ( point is IFlexibleVertexPositionXYZ )
				{
					Vector3 position = ( point as IFlexibleVertexPositionXYZ ).Position;
					GL.Vertex3 ( position.X, position.Y, position.Z );
				}

				if ( point is IFlexibleVertexNormal )
				{
					Vector3 normal = ( point as IFlexibleVertexNormal ).Normal;
					GL.Normal3 ( normal.X, normal.Y, normal.Z );
				}

				if ( point is IFlexibleVertexDiffuse )
				{
					Color diffuse = ( point as IFlexibleVertexDiffuse ).Diffuse;
					GL.Color4 ( diffuse.RedValue, diffuse.GreenValue, diffuse.BlueValue, diffuse.AlphaValue );
				}

				if ( point is IFlexibleVertexTexture1 )
				{
					Vector2 uv = ( point as IFlexibleVertexTexture1 ).TextureUV1;
					GL.TexCoord2 ( uv.X, uv.Y );
				}
			}
			if ( primitive.IsIndexPrimitive )
				foreach ( int index in primitive.Indices )
					GL.Index ( index );
			GL.End ();

		}

		public IImage CreateImage ( ImageData imageData, Color colorKey )
		{
			return new Image ( imageData, colorKey );
		}
	}
}

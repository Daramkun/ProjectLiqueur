﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class GraphicsDevice : IGraphicsDevice
	{
		IWindow window;
		Vector2 screenSize;

		public Version RendererVersion
		{
			get
			{
				string versionString = GL.GetString ( StringName.Version );
				return new Version ( versionString.Substring ( 0, versionString.IndexOf ( ' ' ) ).Trim () );
			}
		}

		public Vector2 [] AvailableScreenSize
		{
			get
			{
				List<Vector2> screenSizes = new List<Vector2> ();
				foreach ( OpenTK.DisplayResolution resolution in OpenTK.DisplayDevice.Default.AvailableResolutions )
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
				if ( window != null )
					( window.Handle as OpenTK.GameWindow ).ClientSize =
						new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y );
				GL.Ortho ( 0, screenSize.X, screenSize.Y, 0, 0.0001f, 1000.0f );
			}
		}

		public bool FullscreenMode
		{
			get
			{
				return ( window.Handle as OpenTK.GameWindow ).WindowState == OpenTK.WindowState.Fullscreen;
			}
			set
			{
				( window.Handle as OpenTK.GameWindow ).WindowState = value ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;
			}
		}

		public bool VerticalSyncMode
		{
			get
			{
				return ( window.Handle as OpenTK.GameWindow ).VSync == OpenTK.VSyncMode.On;
			}
			set
			{
				( window.Handle as OpenTK.GameWindow ).VSync = value ? OpenTK.VSyncMode.On : OpenTK.VSyncMode.Off;
			}
		}

		CullingMode cullMode = CullingMode.None;
		public CullingMode CullingMode
		{
			get { return cullMode; }
			set
			{
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
				return new Viewport ()
				{
					X = viewport [ 0 ],
					Y = viewport [ 1 ],
					Width = viewport [ 2 ],
					Height = viewport [ 3 ]
				};
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

		BlendOperation blendOperation = new BlendOperation ()
		{
			SourceParameter = BlendParameter.One, 
			DestinationParameter = BlendParameter.One
		};
		public BlendOperation BlendOperation
		{
			get
			{
				return blendOperation;
			}
			set
			{
				blendOperation = value;
				GL.BlendFunc ( ConvertBlendSourceFactor ( value.SourceParameter ),
					ConvertBlendDestinationFactor ( value.DestinationParameter ) );
			}
		}

		StencilOperation stencilOperation = new StencilOperation ();
		public StencilOperation StencilOperation
		{
			get { return stencilOperation; }
			set
			{
				stencilOperation = value;
				GL.StencilFunc ( ConvertStencilFunction ( value.Function ), value.Reference, value.Mask );
				GL.StencilOp ( ConvertStencilOperation ( value.Fail ), ConvertStencilOperation ( value.ZFail ),
					ConvertStencilOperation ( value.Pass ) );
			}
		}

		IRenderBuffer renderBuffer;
		public IRenderBuffer RenderTarget
		{
			get { return renderBuffer; }
			set
			{
				renderBuffer = value;
				if ( renderBuffer == null )
					GL.BindFramebuffer ( FramebufferTarget.Framebuffer, 0 );
				else
					GL.BindFramebuffer ( FramebufferTarget.Framebuffer, ( value as RenderBuffer ).frameBuffer );
			}
		}

		public GraphicsDevice ( IWindow window )
		{
			this.window = window;
		}

		~GraphicsDevice ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void BeginScene ()
		{

		}

		public void EndScene ()
		{

		}

		public void Clear ( ClearBuffer clearBuffer, Color color )
		{
			ClearBufferMask bufferMask = ( ClearBufferMask ) 0;
			if ( ( clearBuffer & ClearBuffer.ColorBuffer ) != 0 ) bufferMask |= ClearBufferMask.ColorBufferBit;
			if ( ( clearBuffer & ClearBuffer.DepthBuffer ) != 0 ) bufferMask |= ClearBufferMask.DepthBufferBit;
			if ( ( clearBuffer & ClearBuffer.StencilBuffer ) != 0 ) bufferMask |= ClearBufferMask.StencilBufferBit;

			GL.ClearColor ( color.RedScalar, color.GreenScalar, color.BlueScalar, color.AlphaScalar );
			GL.Clear ( bufferMask );
		}

		public void SwapBuffer ()
		{
			( window.Handle as OpenTK.GameWindow ).SwapBuffers ();
		}

		private void SettingVertexBuffer<T> ( IVertexBuffer<T> vertexBuffer ) where T : struct
		{
			GL.BindBuffer ( BufferTarget.ArrayBuffer, ( vertexBuffer as VertexBuffer<T> ).vertexBuffer );

			int offset = 0, index = 0;
			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.PositionXY ) != 0 )
			{
				GL.EnableVertexAttribArray ( index );
				GL.VertexAttribPointer ( index, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf ( typeof ( T ) ), offset );
				offset += sizeof ( float ) * 2;
				index++;
			}
			else if ( ( vertexBuffer.FVF & FlexibleVertexFormat.PositionXYZ ) != 0 )
			{
				GL.EnableVertexAttribArray ( index );
				GL.VertexAttribPointer ( index, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf ( typeof ( T ) ), offset );
				offset += sizeof ( float ) * 3;
				index++;
			}

			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Normal ) != 0 )
			{
				GL.EnableVertexAttribArray ( index );
				GL.VertexAttribPointer ( index, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf ( typeof ( T ) ), offset );
				offset += sizeof ( float ) * 3;
				index++;
			}

			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Diffuse ) != 0 )
			{
				GL.EnableVertexAttribArray ( index );
				GL.VertexAttribPointer ( index, 4, VertexAttribPointerType.Float, false, Marshal.SizeOf ( typeof ( T ) ), offset );
				offset += sizeof ( float ) * 4;
				index++;
			}

			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV1 ) != 0 )
			{
				GL.EnableVertexAttribArray ( index );
				GL.VertexAttribPointer ( index, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf ( typeof ( T ) ), offset );
				offset += sizeof ( float ) * 2;
				index++;
			}
		}

		private void UnsettingVertexBuffer<T> ( IVertexBuffer<T> vertexBuffer ) where T : struct
		{
			int index = 0;

			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.PositionXY ) != 0 ||
				( vertexBuffer.FVF & FlexibleVertexFormat.PositionXYZ ) != 0 )
				index++;
			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Normal ) != 0 )
				index++;
			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Diffuse ) != 0 )
				index++;
			if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV1 ) != 0 )
				index++;

			for ( ; index >= 0; index-- )
				GL.DisableVertexAttribArray ( index );

			GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
		}

		private void UnsettingTextures ( int length )
		{
			for ( int i = 0; i < length; i++ )
			{
				GL.ActiveTexture ( TextureUnit.Texture0 + i );
				GL.BindTexture ( TextureTarget.Texture2D, 0 );
			}
		}

		public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer ) where T : struct
		{
			SettingVertexBuffer<T> ( vertexBuffer );

			GL.DrawArrays ( ConvertPrimitiveMode ( primitiveType ), 0, vertexBuffer.Length );

			UnsettingVertexBuffer<T> ( vertexBuffer );
		}

		public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer ) where T : struct
		{
			SettingVertexBuffer<T> ( vertexBuffer );

			GL.BindBuffer ( BufferTarget.ElementArrayBuffer, ( indexBuffer as IndexBuffer ).indexBuffer );
			GL.IndexPointer ( IndexPointerType.Int, 0, 0 );

			GL.DrawElements ( ConvertPrimitiveMode ( primitiveType ), indexBuffer.Length, DrawElementsType.UnsignedInt, 0 );

			GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
			UnsettingVertexBuffer<T> ( vertexBuffer );
		}

		public ITexture2D CreateTexture2D ( ImageInfo imageInfo, Color? colorKey = null )
		{
			return new Texture2D ( this, imageInfo, colorKey );
		}

		public IRenderBuffer CreateRenderBuffer ( int width, int height )
		{
			return new RenderBuffer ( this, width, height );
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, int vertexCount ) where T : struct
		{
			return new VertexBuffer<T> ( fvf, vertexCount );
		}

		public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, T [] vertices ) where T : struct
		{
			return new VertexBuffer<T> ( fvf, vertices );
		}

		public IIndexBuffer CreateIndexBuffer ( int indexCount )
		{
			return new IndexBuffer ( indexCount );
		}

		public IIndexBuffer CreateIndexBuffer ( int [] indices )
		{
			return new IndexBuffer ( indices );
		}

		public IShader CreateShader ( Stream stream, ShaderType shaderType )
		{
			return new Shader ( this, stream, shaderType );
		}

		public IShader CreateShader ( string code, ShaderType shaderType )
		{
			return new Shader ( this, code, shaderType );
		}

		public IEffect CreateEffect ( params IShader [] shaders )
		{
			return new Effect ( this, shaders );
		}

		#region Utilities
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

		private StencilOp ConvertStencilOperation ( StencilOperator stencilOperation )
		{
			switch ( stencilOperation )
			{
				case StencilOperator.Zero: return StencilOp.Zero;
				case StencilOperator.Keep: return StencilOp.Keep;
				case StencilOperator.Replace: return StencilOp.Replace;
				case StencilOperator.Invert: return StencilOp.Invert;
				case StencilOperator.Increase: return StencilOp.Incr;
				case StencilOperator.IncreaseSAT: return StencilOp.IncrWrap;
				case StencilOperator.Decrease: return StencilOp.Decr;
				case StencilOperator.DecreaseSAT: return StencilOp.DecrWrap;
			}
			throw new ArgumentException ();
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
		#endregion
	}
}
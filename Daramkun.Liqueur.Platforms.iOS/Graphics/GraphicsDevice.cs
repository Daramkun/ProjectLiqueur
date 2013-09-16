using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;
using OpenTK.Graphics.ES20;

namespace Daramkun.Liqueur.Graphics
{
    class GraphicsDevice : IGraphicsDevice
    {
        IWindow window;
        Vector2 screenSize;

        internal OpenTK.DisplayResolution originalResolution;

		public object Handle { get { return ( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).GraphicsContext; } }

        public BaseRenderer BaseRenderer { get { return Graphics.BaseRenderer.OpenGLES; } }

        public Version RendererVersion
        {
            get
            {
                string versionString = GL.GetString ( All.Version );
                int index = versionString.IndexOf ( ' ' );
                if ( index <= -1 ) index = versionString.IndexOf ( '-' );
                if ( index <= -1 ) index = versionString.Length;
                return new Version ( versionString.Substring ( 0, index ).Trim () );
            }
        }

        public Vector2 [] AvailableScreenSize
        {
            get
            {
                List<Vector2> screenSizes = new List<Vector2> ();
                return screenSizes.ToArray ();
            }
        }

        private Vector2 ChangeToVector ( System.Drawing.RectangleF frame )
        {
            return new Vector2 ( frame.Width, frame.Height );
        }

        public Vector2 ScreenSize
        {
			get { return ( FullscreenMode ) ? screenSize : ChangeToVector ( ( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).Frame ); }
            set
            {
                screenSize = value;
				( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).Frame = new System.Drawing.RectangleF (
					new System.Drawing.PointF (),
                    new System.Drawing.Size ( ( int ) value.X, ( int ) value.Y )
				);
                Viewport = new Viewport () { X = 0, Y = 0, Width = ( int ) value.X, Height = ( int ) value.Y };
            }
        }

        public bool FullscreenMode
        {
            get
            {
				return ( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).WindowState == OpenTK.WindowState.Fullscreen;
            }
            set
            {
				( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).WindowState = value ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;
            }
        }

        public bool VerticalSyncMode { get { return false; } set { } }

        CullingMode cullMode = CullingMode.None;
        public CullingMode CullingMode
        {
            get { return cullMode; }
            set
            {
                if ( value == Graphics.CullingMode.None ) GL.Disable ( All.CullFace );
                else GL.Enable ( All.CullFace );
                GL.FrontFace ( ( value == CullingMode.ClockWise ) ? All.Cw : All.Ccw );
            }
        }

        FillMode fillMode = FillMode.Solid;
        public FillMode FillMode
        {
            get { return fillMode; }
            set
            {
				/*
                fillMode = value;
                GL.PolygonMode ( MaterialFace.FrontAndBack,
                                ( value == FillMode.Point ) ? PolygonMode.Point :
                                ( value == FillMode.Wireframe ) ? PolygonMode.Line : PolygonMode.Fill );
				*/
            }
        }

        public Viewport Viewport
        {
            get
            {
                int [] viewport = new int [ 4 ];
                GL.GetInteger ( All.Viewport, viewport );
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
            get { return GL.IsEnabled ( All.DepthTest ); }
            set
            {
                if ( value ) GL.Enable ( All.DepthTest );
                else GL.Disable ( All.DepthTest );
            }
        }

        public bool BlendState
        {
            get { return GL.IsEnabled ( All.Blend ); }
            set
            {
                if ( value ) GL.Enable ( All.Blend );
                else GL.Disable ( All.Blend );
            }
        }

        public bool StencilState
        {
            get { return GL.IsEnabled ( All.StencilTest ); }
            set
            {
                if ( value ) GL.Enable ( All.StencilTest );
                else GL.Disable ( All.StencilTest );
            }
        }

        BlendOperation blendOperation = new BlendOperation ()
        {
            Operator = BlendOperator.Add,
            SourceParameter = BlendParameter.One, 
            DestinationParameter = BlendParameter.One
        };
        public BlendOperation BlendOperation
        {
            get { return blendOperation; }
            set
            {
                blendOperation = value;
                GL.BlendFunc ( ConvertBlendSourceFactor ( value.SourceParameter ),
                              ConvertBlendDestinationFactor ( value.DestinationParameter ) );
                GL.BlendEquation ( ConvertBlendEquationMode ( value.Operator ) );
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
        Viewport mainViewport;
        public IRenderBuffer RenderTarget
        {
            get { return renderBuffer; }
            set
            {
                if ( value == null )
                {
                    renderBuffer = null;
                    GL.BindFramebuffer ( All.Framebuffer, 0 );
                    Viewport = mainViewport;
                }
                else
                {
                    if ( renderBuffer == null )
                        mainViewport = Viewport;
                    renderBuffer = value;
                    GL.BindFramebuffer ( All.Framebuffer, ( value as RenderBuffer ).frameBuffer );
                    Viewport = new Viewport () { X = 0, Y = 0, Width = value.Width, Height = value.Height };
                }
            }
        }

        public GraphicsDevice ( IWindow window )
        {
            this.window = window;

			( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).Resize += ( object sender, EventArgs e ) =>
            {
                Viewport = new Viewport () { X = 0, Y = 0, Width = ( int ) window.ClientSize.X, Height = ( int ) window.ClientSize.Y };
            };
        }

        ~GraphicsDevice ()
        {
            Dispose ( false );
        }

        protected virtual void Dispose ( bool isDisposing )
        {
            if ( isDisposing )
            {

            }
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
			( window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView ).SwapBuffers ();
        }

        private void SettingVertexBuffer<T> ( IVertexBuffer<T> vertexBuffer ) where T : struct
        {
            GL.BindBuffer ( All.ArrayBuffer, ( vertexBuffer as VertexBuffer<T> ).vertexBuffer );

            int offset = 0, index = 0;
            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.PositionXY ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 2, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 2; 
                index++;
            }
            else if ( ( vertexBuffer.FVF & FlexibleVertexFormat.PositionXYZ ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 3, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 3;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Diffuse ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 4, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 4;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.Normal ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 3, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 3;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV1 ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 2, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 2;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV2 ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 2, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 2;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV3 ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 2, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 2;
                index++;
            }

            if ( ( vertexBuffer.FVF & FlexibleVertexFormat.TextureUV4 ) != 0 )
            {
                GL.EnableVertexAttribArray ( index );
                GL.VertexAttribPointer ( index, 2, All.Float, false, Marshal.SizeOf ( typeof ( T ) ), new IntPtr ( offset ) );
                offset += sizeof ( float ) * 2;
                index++;
            }
        }

        private void UnsettingVertexBuffer<T> ( FlexibleVertexFormat fvf ) where T : struct
        {
            int index = 0;

            if ( ( fvf & FlexibleVertexFormat.PositionXY ) != 0 ||
                ( fvf & FlexibleVertexFormat.PositionXYZ ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.Normal ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.Diffuse ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.TextureUV1 ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.TextureUV2 ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.TextureUV3 ) != 0 )
            index++;
            if ( ( fvf & FlexibleVertexFormat.TextureUV4 ) != 0 )
            index++;

            for ( ; index >= 0; index-- )
            GL.DisableVertexAttribArray ( index );

            GL.BindBuffer ( All.ArrayBuffer, 0 );
        }

        private void UnsettingTextures ( int length )
        {
            for ( int i = 0; i < length; i++ )
            {
                GL.ActiveTexture ( All.Texture0 + i );
                GL.BindTexture ( All.Texture2D, 0 );
            }
        }

        public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer ) where T : struct
        {
            SettingVertexBuffer<T> ( vertexBuffer );

            GL.DrawArrays ( ConvertPrimitiveMode ( primitiveType ), 0, vertexBuffer.Length );

            UnsettingVertexBuffer<T> ( vertexBuffer.FVF );
        }

        public void Draw<T> ( PrimitiveType primitiveType, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer ) where T : struct
        {
            SettingVertexBuffer<T> ( vertexBuffer );

            GL.BindBuffer ( All.ElementArrayBuffer, ( indexBuffer as IndexBuffer ).indexBuffer );

            GL.DrawElements ( ConvertPrimitiveMode ( primitiveType ), indexBuffer.Length, All.UnsignedInt, new IntPtr ( 0 ) );

            GL.BindBuffer ( All.ElementArrayBuffer, 0 );
            UnsettingVertexBuffer<T> ( vertexBuffer.FVF );
        }

        public ITexture2D CreateTexture2D ( int width, int height )
        {
            return new Texture2D ( this, width, height );
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
            return new VertexBuffer<T> ( this, fvf, vertexCount );
        }

        public IVertexBuffer<T> CreateVertexBuffer<T> ( FlexibleVertexFormat fvf, T [] vertices ) where T : struct
        {
            return new VertexBuffer<T> ( this, fvf, vertices );
        }

        public IIndexBuffer CreateIndexBuffer ( int indexCount )
        {
            return new IndexBuffer ( indexCount );
        }

        public IIndexBuffer CreateIndexBuffer ( int [] indices )
        {
            return new IndexBuffer ( this, indices );
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
        private All ConvertBlendSourceFactor ( BlendParameter sourceParameter )
        {
            switch ( sourceParameter )
            {
                case BlendParameter.Zero: return All.Zero;
                    case BlendParameter.One: return All.One;
                    case BlendParameter.SourceAlpha: return All.SrcAlpha;
                    case BlendParameter.InvertSourceAlpha: return All.OneMinusSrcAlpha;
                    case BlendParameter.DestinationColor: return All.DstColor;
                    case BlendParameter.DestinationAlpha: return All.DstAlpha;
                    case BlendParameter.InvertDestinationColor: return All.OneMinusDstColor;
                    case BlendParameter.InvertDestinationAlpha: return All.OneMinusDstAlpha;
            }
            throw new ArgumentException ();
        }

        private All ConvertBlendDestinationFactor ( BlendParameter destinationParameter )
        {
            switch ( destinationParameter )
            {
                case BlendParameter.Zero: return All.Zero;
                    case BlendParameter.One: return All.One;
                    case BlendParameter.SourceColor: return All.SrcColor;
                    case BlendParameter.SourceAlpha: return All.SrcAlpha;
                    case BlendParameter.InvertSourceColor: return All.OneMinusSrcColor;
                    case BlendParameter.InvertSourceAlpha: return All.OneMinusSrcAlpha;
                    case BlendParameter.DestinationAlpha: return All.DstAlpha;
                    case BlendParameter.InvertDestinationAlpha: return All.OneMinusDstAlpha;
            }
            throw new ArgumentException ();
        }

        private All ConvertBlendEquationMode ( BlendOperator blendOperator )
        {
            switch ( blendOperator )
            {
                case BlendOperator.Add: return All.FuncAdd;
                    case BlendOperator.Subtract: return All.FuncSubtract;
                    case BlendOperator.ReverseSubtract: return All.FuncReverseSubtract;
                    case BlendOperator.Minimum: return All.MinExt;
                    case BlendOperator.Maximum: return All.MaxExt;
            }
            return ( All ) ( -1 );
        }

		private All ConvertStencilFunction ( StencilFunction stencilFunction )
        {
            switch ( stencilFunction )
            {
                case StencilFunction.Never: return All.Never;
				case StencilFunction.Equal: return All.Equal;
				case StencilFunction.Less: return All.Less;
				case StencilFunction.LessEqual: return All.Lequal;
				case StencilFunction.Greater: return All.Greater;
				case StencilFunction.GreaterEqual: return All.Gequal;
				case StencilFunction.Always: return All.Always;
				case StencilFunction.NotEqual: return All.Notequal;
            }
            throw new ArgumentException ();
        }

        private All ConvertStencilOperation ( StencilOperator stencilOperation )
        {
            switch ( stencilOperation )
            {
                case StencilOperator.Zero: return All.Zero;
                    case StencilOperator.Keep: return All.Keep;
                    case StencilOperator.Replace: return All.Replace;
                    case StencilOperator.Invert: return All.Invert;
                    case StencilOperator.Increase: return All.Incr;
                    case StencilOperator.IncreaseSAT: return All.IncrWrap;
                    case StencilOperator.Decrease: return All.Decr;
                    case StencilOperator.DecreaseSAT: return All.DecrWrap;
            }
            throw new ArgumentException ();
        }

        private All ConvertPrimitiveMode ( PrimitiveType type )
        {
            switch ( type )
            {
                case PrimitiveType.PointList: return All.Points;
                    case PrimitiveType.LineList: return All.Lines;
                    case PrimitiveType.LineStrip: return All.LineStrip;
                    case PrimitiveType.TriangleList: return All.Triangles;
                    case PrimitiveType.TriangleStrip: return All.TriangleStrip;
                    case PrimitiveType.TriangleFan: return All.TriangleFan;
                    default: throw new Exception ();
            }
        }
        #endregion
    }
}

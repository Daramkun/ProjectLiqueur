Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Threading.Tasks
Imports Daramkun.Liqueur
Imports Daramkun.Liqueur.Common
Imports Daramkun.Liqueur.Contents
Imports Daramkun.Liqueur.Contents.FileSystems
Imports Daramkun.Liqueur.Contents.Loaders
Imports Daramkun.Liqueur.Decoders
Imports Daramkun.Liqueur.Decoders.Images
Imports Daramkun.Liqueur.Decoders.Sounds
Imports Daramkun.Liqueur.Math
Imports Daramkun.Liqueur.Graphics
Imports Daramkun.Liqueur.Graphics.Fonts
Imports Daramkun.Liqueur.Inputs
Imports Daramkun.Liqueur.Inputs.Devices
Imports Daramkun.Liqueur.Inputs.States
Imports Daramkun.Liqueur.Nodes
Imports Daramkun.Liqueur.Platforms
Imports Daramkun.Liqueur.Scenes
Imports Daramkun.Walnut
Imports Daramkun.Walnut.Nodes
Imports Daramkun.Walnut.Scripts
Imports Daramkun.Liqueur.Graphics.Vertices

Namespace Test
	Structure MyVertex
		Implements IFlexibleVertexPositionXY
		Implements IFlexibleVertexDiffuse

		Public Property Position As Vector2 Implements IFlexibleVertexPositionXY.Position
		Public Property Diffuse As Color Implements IFlexibleVertexDiffuse.Diffuse
	End Structure

	Class MyScene
		Inherits Scene
		Private fpsCalc As FpsCalculator
		Private primitive As IPrimitive(Of MyVertex)
		Private sprite As Sprite

		Public Sub New()
		End Sub

		Public Overrides Sub OnInitialize()
			LiqueurSystem.Window.Title = "Test Window in Visual Basic.NET"

			fpsCalc = New FpsCalculator()
			AddChild(fpsCalc)

			primitive = LiqueurSystem.Renderer.CreatePrimitive(Of MyVertex)(3, 0)
			primitive.Vertices(0) = New MyVertex() With {
				.Position = New Vector2(200, 100),
				.Diffuse = Color.Red
			}
			primitive.Vertices(1) = New MyVertex() With {
				.Position = New Vector2(100, 300),
				.Diffuse = Color.Green
			}
			primitive.Vertices(2) = New MyVertex() With {
				.Position = New Vector2(300, 300),
				.Diffuse = Color.Blue
			}
			primitive.PrimitiveType = PrimitiveType.TriangleList
			primitive.PrimitiveCount = 1

			AddChild(New Sprite(WalnutSystem.MainContents.Load(Of ITexture2D)("a02.bmp", Color.Magenta)) With {
				.Position = New Vector2(20, 20),
				.SourceRectangle = New Rectangle(New Vector2(20, 20), New Vector2(100, 100))
			})
			AddChild(New Sprite(WalnutSystem.MainContents.Load(Of ITexture2D)("a02.bmp", Color.Magenta)) With {
				.Position = New Vector2(140, 20),
				.OverlayColor = New Color(127, 0, 0, 127)
			})
			sprite = TryCast(AddChild(New Sprite(Nothing) With {
				.Position = New Vector2(340, 100)
			}), Sprite)

			AddChild(New Label(WalnutSystem.MainContents.Load(Of BaseFont)("test.lsf")) With {
				.Text = "Test한글도 잘 나옴★ひらかなもキラン☆0123漢字`!@#$%?.,ⓕ\" & vbLf + "LSF 폰트 파일 로드가 좀 느리네" &
				vbLf + "ZIPLSF 파일 로드 엄청 빨라짐" & vbLf + "뷁뷝뿗颬",
				.Position = New Vector2(10, 380),
				.ForeColor = Color.White
			})
			AddHandler AddChild(New Label(WalnutSystem.MainContents.Load(Of BaseFont)("test.lsf")) With {
				.Position = New Vector2(10, 590),
				.ForeColor = Color.Cyan,
				.ObjectOffset = ObjectOffset.BottomLeft
			}).Update, Sub(sender As Object, e As GameTimeEventArgs)
									LiqueurSystem.Renderer.BlendState = True
									TryCast(sender, Label).Text = [String].Format("Update FPS: {0}" & vbLf & "Render FPS: {1}" & vbLf &
																				  "Blend State: {2}" & vbLf & "Stencil State:{3}" & vbLf &
																				  "Viewport:{4}", fpsCalc.UpdateFPS, fpsCalc.DrawFPS,
																				  LiqueurSystem.Renderer.BlendState,
																				  LiqueurSystem.Renderer.StencilState,
																				  LiqueurSystem.Renderer.Viewport)
								End Sub
			MyBase.OnInitialize()
		End Sub

		Public Overrides Sub OnFinalize()
			MyBase.OnFinalize()
		End Sub

		Public Overrides Sub OnUpdate(gameTime As GameTime)
			sprite.Image = WalnutSystem.MainContents.Load(Of ITexture2D)("square.png")
			MyBase.OnUpdate(gameTime)
		End Sub

		Public Overrides Sub OnDraw(gameTime As GameTime)
			LiqueurSystem.Renderer.Clear(Color.Black)
			LiqueurSystem.Renderer.BlendState = True
			LiqueurSystem.Renderer.DrawPrimitive(Of MyVertex)(primitive)
			MyBase.OnDraw(gameTime)
		End Sub
	End Class

	Module Program
		<STAThread>
		Sub Main()
			WalnutSystem.SetupDecoders()
			WalnutSystem.SetupFixedLogicTimeStep(TimeSpan.FromTicks(166666), TimeSpan.FromTicks(166666))
			WalnutSystem.SetupInputDevices(Of Keyboard, Mouse, GamePad, TouchPanel, Accelerometer)()
			WalnutSystem.Run(Of Launcher, NoneScriptEngine, MyScene)(New LocalFileSystem())
		End Sub
	End Module
End Namespace
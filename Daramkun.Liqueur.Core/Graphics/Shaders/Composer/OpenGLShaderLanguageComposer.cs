using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders.Composer
{
	public class OpenGLShaderLanguageComposer : IShaderComposer
	{
		private string ConvertDataType ( DataType dataType )
		{
			switch ( dataType )
			{
				case DataType.Void: return "void";
				case DataType.Boolean: return "bool";
				case DataType.Integer: return "int";
				case DataType.UnsignedInteger: return "uint";

				case DataType.Vector2: return "vec2";
				case DataType.Vector3: return "vec3";
				case DataType.Vector4: return "vec4";

				case DataType.Matrix2: return "mat2";
				case DataType.Matrix3: return "mat3";
				case DataType.Matrix4: return "mat4";

				case DataType.Sampler1D: return "sampler1D";
				case DataType.Sampler2D: return "sampler2D";
				case DataType.Sampler3D: return "sampler3D";
				case DataType.SamplerCube: return "samplerCube";

				default: return "";
			}
		}

		private string ConvertVariables ( IEnumerable<Variable> e )
		{
			string param = "";
			foreach ( var v in e )
			{
				param += string.Format ( "{0} {1}, ", ConvertDataType ( v.VariableType ), v.Name );
			}
			return param.Substring ( 0, param.Length - 2 );
		}

		public string Compose ( Shader shader )
		{
			List<string> shaderCode = new List<string> ();
			Parameter inputParameter, outputParameter;

			foreach ( var param in shader.Parameters )
			{
				switch ( param.ParameterType )
				{
					case ParameterType.Uniform:
						{
							foreach ( Variable var in param.Variables )
								shaderCode.Add ( string.Format ( "uniform {0} {1}", ConvertDataType ( var.VariableType ),
									var.Name ) );
						}
						break;
					case ParameterType.Input:
						inputParameter = param;
						break;
					case ParameterType.Output:
						outputParameter = param;
						break;
				}
			}

			foreach ( var func in shader.Functions )
			{
				shaderCode.Add ( string.Format ( "{0} {1} ( {2} )", ConvertDataType ( func.ReturnType ),
					func.Name, ConvertVariables ( func.Parameters ) ) );
				shaderCode.Add ( "{" );
				foreach ( var op in func.Operations )
				{

				}
				shaderCode.Add ( "}" );
			}

			return string.Join ( Environment.NewLine, shaderCode.ToArray () );
		}
	}
}

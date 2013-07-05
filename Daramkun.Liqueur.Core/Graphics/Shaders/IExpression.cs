using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public interface IExpression
	{
	}

	public struct MakeVarExpression : IExpression
	{
		public string VariableName { get; set; }
		public DataType VariableType { get; set; }
	}

	public struct Argument : IExpression
	{
		public DataType ArgumentType { get; set; }
		public object ArgumentData { get; set; }
	}

	public enum BasicFunction
	{
		Vector2,
		Vector3,
		Vector4,
		Normalize,
		Absolute,
		Acosine,
		All,
		Any,
		Asine,
		Atangent,
		Atangent2,
		Ceil,
		Clamp,
		Clip,
		Cosine,
		CosineH,
		Cross,
		ddx,
		ddy,
		Degrees,
		Determinant,
		Distance,
		Dot,
		Exp,
		Exp2,
		FaceForward,
		Floor,
		Fmod,
		Frac,
		Frc,
		Frexp,
		Fwidth,
		IsFinate,
		isInf,
		isNan,
		ldexp,
		Length,
		Lerp,
		lit,
		Log,
		Log10,
		Log2,
		Max,
		Min,
		Multiply,
		Noise,
		Power,
		Radians,
		Reflect,
		Refract,
		Round,
		Rsqrt,
		Saturate,
		Sign,
		Sine,
		Sinecosine,
		SineH,
		SmoothStep,
		Sqrt,
		Step,
		Tangent,
		TangentH,
		Texture1D,
		Texture2D,
		Transpose,
	}

	public struct FuncCallExpression : IExpression
	{
		public string FunctionName { get; set; }
		public List<Argument> Arguments { get; set; }

		public FuncCallExpression ( BasicFunction basic )
			: this ()
		{
		
		}
	}

	public enum OperationType
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulation,
		
		LeftShift,
		RightShift,
		BitAND,
		BitOR,
		BitNOT,
		BitXOR,

		Assignment,
		LogicalAND,
		LogicalOR,
		LogicalNOT,

		ParameterAccess,
	}

	public struct OperationExpression : IExpression
	{
		public Argument Operand1 { get; set; }
		public Argument Operand2 { get; set; }
		public OperationType OperationType { get; set; }
	}

	public struct BreakExpression : IExpression
	{
		
	}

	public struct IfExpression : IExpression
	{
		public OperationExpression Operation { get; set; }
		public List<IExpression> TrueExpressions { get; set; }
		public List<IExpression> FalseExpressions { get; set; }
	}

	public struct WhileExpression : IExpression
	{
		public OperationExpression Operation { get; set; }
		public List<IExpression> Expressions { get; set; }
	}

	public struct ForExpression : IExpression
	{
		public OperationExpression Initializer { get; set; }
		public OperationExpression Operation { get; set; }
		public OperationExpression Processor { get; set; }
		public List<IExpression> Expressions { get; set; }
	}

	public struct ReturnExpression : IExpression
	{
		public Argument ReturnValue { get; set; }
	}
}
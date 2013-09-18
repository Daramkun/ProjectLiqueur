float4x4 projectionMatrix;
float4x4 worldMatrix;

struct VS_INPUT
{
	float3 i_position : POSITION;
	float4 i_overlay : COLOR;
	float2 i_texture : TEXCOORD0;
};

struct VS_OUTPUT
{
	float4 o_position : POSITION;
	float4 o_overlay;
	float2 o_texture;
};

VS_OUTPUT vs_main(VS_INPUT input)
{
	VS_OUTPUT output;
	output.o_position = float4(input.i_position, 1);
	output.o_position = worldMatrix * output.o_position;
	output.o_position = projectionMatrix * output.o_position;
	output.o_overlay = input.i_overlay;
	output.o_texture = input.i_texture;
	return output;
}
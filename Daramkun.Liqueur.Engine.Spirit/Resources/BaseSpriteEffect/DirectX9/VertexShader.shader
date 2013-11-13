float4x4 projectionMatrix;
float4x4 worldMatrix;

struct VS_INPUT
{
   float2 i_position : POSITION;
   float4 i_overlay : COLOR;
   float2 i_texture : TEXCOORD0;
};

struct PS_INPUT
{
   float4 o_position : POSITION;
   float4 o_overlay : COLOR;
   float2 o_texture : TEXCOORD0;
};

PS_INPUT vs_main(VS_INPUT input)
{
   PS_INPUT output;
   output.o_position = float4(input.i_position, 1, 1);
   output.o_position = mul(output.o_position, worldMatrix);
   output.o_position = mul(output.o_position, projectionMatrix);
   output.o_overlay = input.i_overlay;
   output.o_texture = input.i_texture;
   return output;
}
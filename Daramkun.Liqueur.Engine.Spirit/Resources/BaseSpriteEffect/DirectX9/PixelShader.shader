sampler2D texture0;

struct PS_INPUT
{
   float4 o_position : POSITION;
   float4 o_overlay : COLOR;
   float2 o_texture : TEXCOORD0;
};

float4 ps_main(PS_INPUT input) : COLOR
{
   return tex2D ( texture0, input.o_texture ) * input.o_overlay;
}
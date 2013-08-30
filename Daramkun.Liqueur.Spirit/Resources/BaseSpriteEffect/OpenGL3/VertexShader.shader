#version 130
layout(location = 0) in vec3 i_position;
layout(location = 1) in vec4 i_overlay;
layout(location = 2) in vec2 i_texture;

uniform mat4 projectionMatrix;
uniform mat4 worldMatrix;

out vec4 o_overlay;
out vec2 o_texture;

void main () {
	vec4 pos = vec4(i_position, 1);
	pos = worldMatrix * pos;
	pos = projectionMatrix * pos;

	gl_Position = pos;

	o_overlay = i_overlay;
	o_texture = i_texture;
}
#version 120
attribute vec3 i_position;
attribute vec4 i_overlay;
attribute vec2 i_texture;

uniform mat4 projectionMatrix;
uniform mat4 worldMatrix;

varying vec4 o_overlay;
varying vec2 o_texture;

void main () {
	vec4 pos = vec4(i_position, 1);
	pos = worldMatrix * pos;
	pos = projectionMatrix * pos;

	gl_Position = pos;

	o_overlay = i_overlay;
	o_texture = i_texture;
}
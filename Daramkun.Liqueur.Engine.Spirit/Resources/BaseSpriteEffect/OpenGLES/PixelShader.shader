varying vec4 o_overlay;
varying vec2 o_texture;

uniform sampler2D texture0;

void main () {
	gl_FragColor = texture2D ( texture0, o_texture.st ) * o_overlay;
}
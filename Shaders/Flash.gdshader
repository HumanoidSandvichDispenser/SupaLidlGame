shader_type canvas_item;

uniform vec4 color = vec4(1.0);
uniform float intensity : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
    tex.rgb = mix(tex.rgb, color.rgb, intensity);
    COLOR = tex;
}
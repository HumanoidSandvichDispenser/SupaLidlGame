shader_type canvas_item;
render_mode blend_mix;

uniform float speed = 16;
uniform vec2 direction = vec2(1, 0);
uniform vec4 tint_color : source_color;

void fragment() {
    vec4 color = texture(TEXTURE, UV  - direction * TIME * speed);
    COLOR = color * tint_color;
}

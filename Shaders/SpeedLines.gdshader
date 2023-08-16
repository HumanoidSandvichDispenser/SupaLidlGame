shader_type canvas_item;

uniform float value_threshold : hint_range(0, 1, 0.1) = 0.5;
uniform vec4 color_max : source_color = vec4(1);
uniform vec4 color_min : source_color = vec4(0);
uniform float speed = 16;
uniform float stretch_factor = 256;
uniform float end_alpha_x = 0;
uniform float end_alpha_y = 0;

void fragment() {
    vec4 tex = texture(TEXTURE, vec2((UV.x + TIME * speed) / stretch_factor, UV.y));
    //vec4 tex = texture(TEXTURE, UV + vec2(4, 0) * TIME * speed);
    vec4 color = tex;
    if (tex.r < value_threshold) {
        color = vec4(0);
    } else {
        color = mix(color_min, color_max, tex.r);
        color.a = mix(end_alpha_x, color.a, UV.x);
        color.a = mix(color.a, end_alpha_y, abs(0.5 - UV.y) * 2.0);
    }
    COLOR = color;
}

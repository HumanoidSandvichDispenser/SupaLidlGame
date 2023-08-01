shader_type canvas_item;

uniform float level : hint_range(0.0, 1.0) = 0.5; 
uniform vec4 water_albedo : source_color = vec4(0.26, 0.23, 0.73, 1.0);
uniform float alpha : hint_range(0.0, 1.0) = 0.75;
uniform float water_speed = 0.05;
uniform float wave_distortion = 0.2;
uniform sampler2D noise_texture : repeat_enable;
uniform sampler2D noise_texture2 : repeat_enable;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    vec2 scroll1 = vec2(water_speed);
    vec2 scroll2 = vec2(-water_speed);
    float depth = texture(noise_texture, UV + scroll1 * TIME).r *
        texture(noise_texture2, UV + scroll2 * TIME).r;
    vec4 screen = texture(SCREEN_TEXTURE, SCREEN_UV + wave_distortion * vec2(depth));
    COLOR = screen;
    COLOR.a = alpha;
}
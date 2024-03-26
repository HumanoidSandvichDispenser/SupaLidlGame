shader_type canvas_item;

uniform float grayscale_intensity : hint_range(0.0, 1.0) = 0.0;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    vec4 color = texture(SCREEN_TEXTURE, SCREEN_UV);
    float average = (color.r + color.g + color.b) / 3.0;
    COLOR = vec4(vec3(average), grayscale_intensity);
}

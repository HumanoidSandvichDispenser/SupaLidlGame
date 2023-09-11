/*
 * This shader is based on the Godrays shader by pend00
 * (https://godotshaders.com/shader/god-rays/), but heavily modified to be
 * compatible with both the Forward+ (Vulkan) and Compatibility (OpenGL)
 * renderers.
 */

shader_type canvas_item;

uniform vec4 tint_color : source_color;
uniform float alpha : hint_range(0.0, 1.0) = 0.75;
uniform sampler2D noise_texture : repeat_enable;
uniform float speed : hint_range(0.0, 1.0) = 0.25;
uniform float spread = 1;
uniform float opposite_ray_mult = 1.0;
uniform float negative_intensity = 1.0;
uniform float negative_ray_speed : hint_range(0.0, 1.0) = 0;
uniform float cutoff : hint_range(0.0, 1.0) = 0;
uniform float smooth_cutoff : hint_range(0.0, 1.0) = 0;
uniform float y_cutoff : hint_range(0.0, 1.0) = 0;
uniform float y_smooth_cutoff : hint_range(0.0, 1.0) = 0;
uniform float angle : hint_range(-3.14, 3.14);

float noise(vec2 uv) {
    return texture(noise_texture, uv).r;
}

mat2 rotate(float _angle){
    return mat2(vec2(cos(_angle), -sin(_angle)),
                vec2(sin(_angle), cos(_angle)));
}

void fragment() {
    vec2 t_uv = UV * rotate(angle) / ((UV.y + spread) - (UV.y * spread));
    
    vec2 ray1 = vec2(t_uv.x + sin(TIME * speed / 10.0));
    vec2 ray2 = vec2(-t_uv.x + sin(TIME * speed / 15.0));
    vec2 ray3 = vec2(t_uv.x + cos(TIME * negative_ray_speed / 10.0));
    float a;
    
    // if t_uv.x exceeds cut value then do not show ray (multiply by 0)
    float cut = step(cutoff, t_uv.x) * step(cutoff, 1.0 - UV.x);
    cut *= smoothstep(cutoff, cutoff + smooth_cutoff, t_uv.x) *
        smoothstep(cutoff, cutoff + smooth_cutoff, 1.0 - t_uv.x);
    float y_cut = step(y_cutoff, 1.0 - t_uv.y) * smoothstep(y_cutoff, y_cutoff + y_smooth_cutoff, 1.0 - UV.y);
    ray1 *= cut;
    ray2 *= cut;
    ray3 *= cut;
    a = cut * y_cut;
    
    float rays;
    rays = clamp(noise(ray1) + opposite_ray_mult * noise(ray2) - negative_intensity * noise(ray3), 0.0, 1.0);
    
    
    //shine = screen(texture(SCREEN_TEXTURE, SCREEN_UV), vec4(color)).rgb;
	COLOR = vec4(vec3(1.0), rays * a) * tint_color;
}

shader_type canvas_item;

uniform float a: hint_range(0.0, 1.0, 0.1) = 0;
uniform float b: hint_range(0.0, 1.0, 0.1) = 0;

void fragment() {
    if (UV.x < a) {
        if (UV.x > b) {
            COLOR.a = 1.0;
        } else {
            COLOR.a = 0.0;
        }
    } else {
        COLOR.a = 0.0;
    }
}
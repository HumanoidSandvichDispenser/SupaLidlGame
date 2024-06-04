shader_type canvas_item;

uniform float amount : hint_range(0.5, 1.0, 0.01) = 1;

void fragment() {
    COLOR.a *= step(UV.x, amount) * step(1.0 - amount, UV.x);
}

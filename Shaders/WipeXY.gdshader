shader_type canvas_item;

uniform float x_amount : hint_range(0.5, 1.0, 0.01) = 1;
uniform float y_amount : hint_range(0.5, 1.0, 0.01) = 0.5;

void fragment() {
    COLOR.a *= step(UV.x, x_amount) * step(1.0 - x_amount, UV.x);
    //COLOR.a *= step(1.0 - y_amount, UV.y) * step(UV.y, y_amount);
    COLOR.a *= 1.0 - (step(1.0 - y_amount, UV.y) * step(UV.y, y_amount));
}

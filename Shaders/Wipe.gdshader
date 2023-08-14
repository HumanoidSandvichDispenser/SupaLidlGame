shader_type canvas_item;

uniform float amount : hint_range(0.5, 1.0, 0.01) = 1;

void fragment() {
	// Place fragment code here.
    //COLOR = texture(TEXTURE, UV);
    if (UV.x > amount || UV.x < 1.0 - amount) {
        COLOR.a = 0.0;
    }
}

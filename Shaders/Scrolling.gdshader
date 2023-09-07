shader_type canvas_item;

uniform float speed = 16;
uniform vec2 direction = vec2(1, 0);

void fragment() {
    COLOR = texture(TEXTURE, UV  - direction * TIME * speed);
}

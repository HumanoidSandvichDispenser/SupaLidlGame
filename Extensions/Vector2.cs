using Godot;
using System.Linq;

namespace SupaLidlGame.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Midpoint(this Vector2 vector, Vector2 other)
        {
            return new Vector2((vector.x + other.x) / 2,
                               (vector.y + other.y) / 2);
        }

        public static Vector2 Midpoints(params Vector2[] vectors)
        {
            int length = vectors.Length;
            float x = 0;
            float y = 0;

            for (int i = 0; i < length; i++)
            {
                x += vectors[i].x;
                y += vectors[i].y;
            }

            return new Vector2(x / length, y / length);
        }

        public static Vector2 Counterclockwise90(this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        public static Vector2 Clockwise90(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }
    }
}

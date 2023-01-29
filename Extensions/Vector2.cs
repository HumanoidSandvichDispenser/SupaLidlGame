using Godot;

namespace SupaLidlGame.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Midpoint(this Vector2 vector, Vector2 other)
        {
            return new Vector2((vector.X + other.X) / 2,
                               (vector.Y + other.Y) / 2);
        }

        public static Vector2 Midpoints(params Vector2[] vectors)
        {
            int length = vectors.Length;
            float x = 0;
            float y = 0;

            for (int i = 0; i < length; i++)
            {
                x += vectors[i].X;
                y += vectors[i].Y;
            }

            return new Vector2(x / length, y / length);
        }

        public static Vector2 Counterclockwise90(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 Clockwise90(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }
    }
}

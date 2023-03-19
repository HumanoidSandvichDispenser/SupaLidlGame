using Godot;

namespace SupaLidlGame
{
    public static class AudioStreamPlayer2DExtensions
    {
        public static void PlayOn(this AudioStreamPlayer2D audio, Node parent)
        {
            var clone = audio.Duplicate() as AudioStreamPlayer2D;
            parent.AddChild(clone);
            clone.Play();
            clone.Finished += () =>
            {
                clone.QueueFree();
            };
        }

        public static void PlayOnRoot(this AudioStreamPlayer2D audio)
        {
            var root = audio.GetTree().Root.GetChild(0);
            audio.PlayOn(root);
        }
    }
}

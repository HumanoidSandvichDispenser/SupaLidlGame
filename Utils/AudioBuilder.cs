using Godot;

namespace SupaLidlGame.Utils
{
    public class AudioBuilder
    {
        private AudioStreamPlayer2D _audio;

        public AudioBuilder(AudioStreamPlayer2D baseAudio)
        {
            _audio = baseAudio;
        }
    }
}

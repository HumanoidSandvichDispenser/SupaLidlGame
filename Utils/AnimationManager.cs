using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.Utils;

public partial class AnimationManager : Node
{
    public List<AnimationPlayer> Channels { get; set; }

    private List<AnimationHold> _holds = new List<AnimationHold>();

    private struct AnimationHold
    {
        public Animation PriorityAnim; // channel to be prioritized
        public Animation HoldAnim; // channel with disabled track
        public int TrackIndex; // track idx on HoldAnim
    }

    public override void _Ready()
    {
        Channels = new List<AnimationPlayer>();
        foreach (var node in GetChildren())
        {
            if (node is AnimationPlayer player)
            {
                Channels.Add(player);
            }
        }

        // lower position = lower priority
        for (int _p1Index = 0; _p1Index < Channels.Count; _p1Index++)
        {
            // apparently _p1Index is "static" when referencing it inside the
            // anonymous functions, so we declare a temp variable so it stays
            // the same value
            var p1Index = _p1Index;
            var p1 = Channels[p1Index];

            p1.AnimationStarted += (StringName name) =>
            {
                var anim = p1.GetAnimation(name);
                RemoveAllHolds(anim);
                for (int p2Index = 0; p2Index < Channels.Count; p2Index++)
                {
                    var p2 = Channels[p2Index];
                    if (p1 != p2)
                    {
                        CheckConflicts(p1, p1Index, p2, p2Index);
                    }
                }
            };

            p1.AnimationChanged += (StringName oldName, StringName newName) =>
            {
                var anim = p1.GetAnimation(oldName);
                GD.Print(oldName + "resolve");
                ResolveConflicts(anim);
                RemoveAllHolds(anim);
            };

            p1.AnimationFinished += (StringName name) =>
            {
                var anim = p1.GetAnimation(name);
                ResolveConflicts(anim);
                RemoveAllHolds(anim);
            };
        }
    }

    private string NormPath(string path)
    {
        return path.Replace("../", "").Replace("./", "");
    }

    private void CheckConflicts(AnimationPlayer p1, int p1Priority,
                                AnimationPlayer p2, int p2Priority)
    {
        if (p1Priority == p2Priority)
        {
            throw new System.InvalidOperationException();
        }
        if (p1.IsPlaying() && p2.IsPlaying())
        {
            var anim1 = p1.GetAnimation(p1.CurrentAnimation);
            var anim2 = p2.GetAnimation(p2.CurrentAnimation);

            // compare each track to see if they conflict nodepaths
            for (int p1Track = 0; p1Track < anim1.GetTrackCount(); p1Track++)
            {
                string anim1Node = NormPath(anim1.TrackGetPath(p1Track));
                for (int p2Track = 0; p2Track < anim2.GetTrackCount(); p2Track++)
                {
                    string anim2Node = NormPath(anim2.TrackGetPath(p2Track));
                    if (anim1Node == anim2Node)
                    {
                        var hold = new AnimationHold();
                        if (p1Priority > p2Priority)
                        {
                            hold.PriorityAnim = anim1;
                            hold.HoldAnim = anim2;
                            hold.TrackIndex = p2Track;
                            anim2.TrackSetEnabled(p2Track, false);
                        }
                        else
                        {
                            hold.PriorityAnim = anim2;
                            hold.HoldAnim = anim1;
                            hold.TrackIndex = p1Track;
                            anim1.TrackSetEnabled(p1Track, false);
                        }
                        GD.Print($"{hold.PriorityAnim.ResourceName} > {hold.HoldAnim.ResourceName}");
                        _holds.Add(hold);
                        break;
                    }
                }
            }
        }
    }

    private void ResolveConflicts(Animation anim)
    {
        // loop through all holds
        for (int i = 0; i < _holds.Count; i++)
        {
            var hold = _holds[i];

            // if the animation is a priority animation, remove it
            // if there are no other holds on the track, then set the track to
            // be enabled
            if (hold.PriorityAnim == anim)
            {
                GD.Print($"{anim.ResourceName} was holding ${hold.HoldAnim.ResourceName}");
                _holds.RemoveAt(i);
                i--;

                // if can't find hold with same track index, then it is safe to
                // enable the track
                if (_holds.FindIndex(h => h.HoldAnim == hold.HoldAnim && h.TrackIndex == hold.TrackIndex) < 0)
                {
                    GD.Print($"{hold.HoldAnim.ResourceName} is now free");
                    hold.HoldAnim.TrackSetEnabled(hold.TrackIndex, true);
                }
            }
        }

        GD.Print("There are currently " + _holds.Count + " holds");
    }

    private void RemoveAllHolds(Animation anim)
    {
        // remove all holds where the Animation is being held
        for (int i = 0; i < _holds.Count; i++)
        {
            var hold = _holds[i];

            // if the animation is a priority animation, remove it
            // if there are no other holds on the track, then set the track to
            // be enabled
            if (hold.HoldAnim == anim)
            {
                _holds.RemoveAt(i);
                i--;
                hold.HoldAnim.TrackSetEnabled(hold.TrackIndex, true);
            }
        }
    }
}

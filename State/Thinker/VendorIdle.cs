using Godot;
using GodotUtilities;

namespace SupaLidlGame.State.Thinker;

public partial class VendorIdle : ThinkerState
{
    public override ThinkerState Think()
    {
        var bestNeutral = NPC.FindBestNeutral();
        if (bestNeutral is not null)
        {
            NPC.Target = bestNeutral.Position - NPC.Position;
        }
        return null;
    }
}

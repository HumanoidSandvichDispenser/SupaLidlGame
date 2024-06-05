//using Godot;
//using SupaLidlGame.BoundingBoxes;
//using SupaLidlGame.Extensions;
//
//namespace SupaLidlGame.Characters;
//
//public partial class Vendor : NPC
//{
//    [Export]
//    public InteractionTrigger InteractionTrigger { get; set; }
//
//    [Export(PropertyHint.File, "*.dialogue")]
//    public Resource DialogueResource { get; set; }
//
//    [Export]
//    public string DialogueTitle { get; set; }
//
//    public override void _Ready()
//    {
//        InteractionTrigger.Interaction += OnInteraction;
//    }
//
//    private void OnInteraction()
//    {
//        this.GetWorld().DialogueBalloon.Start(DialogueResource, DialogueTitle);
//    }
//}

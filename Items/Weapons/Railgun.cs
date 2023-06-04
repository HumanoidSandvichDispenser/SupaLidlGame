using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Items.Weapons;

public partial class Railgun : Ranged
{
    public override void Attack()
    {
        // create projectile
        PackedScene scene = GD.Load<PackedScene>("res://Entities/RailBeam.tscn");
        GD.Print("lol");
        var projectile = scene.Instantiate<Entities.Projectile>();
        projectile.Hitbox.Faction = Character.Faction;
        projectile.Direction = Character.Target;
        projectile.GlobalPosition = GlobalPosition;
        projectile.GlobalRotation = projectile.Direction.Angle();
        this.GetAncestor<SupaLidlGame.Scenes.Map>()
            .Entities.AddChild(projectile);
    }
}

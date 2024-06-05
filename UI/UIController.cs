using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.UI;

public partial class UIController : Control
{
    [Export]
    public TextureProgressBar PlayerHealthBar { get; set; }

    [Export]
    public BossBar BossBar { get; set; }

    private Stack<IModal> _modals;

    public override void _Ready()
    {
        Events.EventBus.Instance.EnterShop += (string path) =>
        {
            var shop = ResourceLoader.Load<Items.Shop>(path);
            var shopMenu = GetNode<Inventory.ShopMenu>("%ShopMenu");
            shopMenu.Source = shop;
            shopMenu.ShowModal();
        };
    }
}

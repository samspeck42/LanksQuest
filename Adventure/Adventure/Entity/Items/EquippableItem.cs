using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public interface EquippableItem
    {
        Texture2D InventoryScreenIcon { get; }
        Point InventoryScreenPoint { get; }
        bool IsDoneBeingUsed { get; }

        void StartBeingUsed();
    }
}

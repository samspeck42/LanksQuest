using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public interface PickupDropper
    {
        float PickupDropChance { get; }

        Vector2 PickupDropPosition { get; }
    }
}

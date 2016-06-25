﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure.Entities.Items
{
    public interface EquippableItem
    {
        string InventoryScreenId { get; }

        void StartUsing(int itemButtonNumber);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    public interface PickupDropper
    {
        float DropChance
        {
            get;
        }

        Pickup SpawnPickup();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Adventure.Entities.Items
{
    public class Bow : EquippableItem
    {
        Player player;

        public Bow(Player player)
        {
            this.player = player;
        }

        public string InventoryScreenId
        {
            get { return "bow"; }
        }

        public void StartUsing(int itemButtonNumber)
        {
            player.StartUsingBow(itemButtonNumber);
        }
    }
}

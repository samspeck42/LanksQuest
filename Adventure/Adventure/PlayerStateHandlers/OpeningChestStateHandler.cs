﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Adventure.Entities.Items;
using Adventure.Entities.Environment;
using Adventure.Entities;

namespace Adventure.PlayerStateHandlers
{
    public class OpeningChestStateHandler : PlayerStateHandler
    {
        public override PlayerState State { get { return PlayerState.OpeningChest; } }

        public override bool CanWalk { get { return false; } }

        public override bool CanGetHurt { get { return false; } }

        public override bool CanLeaveArea { get { return false; } }

        public override bool CanStartAttacking { get { return false; } }

        public override bool CanInteract { get { return false; } }

        public override bool CanChangeFaceDirection { get { return false; } }

        public override bool CanStartUsingEquippableItem(EquippableItem item) { return false; }


        private Chest chest;

        public OpeningChestStateHandler(Player player, Chest chest)
            : base(player) 
        {
            this.chest = chest;
        }

        public override void Start()
        {
            chest.StartOpening();
        }

        public override void Update(GameTime gameTime)
        {
            if (chest.IsOpened)
            {
                if (chest.Treasure != null)
                {
                    Entity treasure = chest.Treasure;

                    if (treasure is Pickup)
                        player.CollectPickup((Pickup)treasure);
                    if (treasure is EquippableItem)
                        player.Inventory.CollectEquippableItem((EquippableItem)treasure);
                }

                player.EnterState(new NormalStateHandler(player));
            }
        }
    }
}

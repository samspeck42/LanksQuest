using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class OpeningChestStateHandler : PlayerStateHandler
    {
        public override PlayerState State { get { return PlayerState.OpeningChest; } }

        public override bool CanMove { get { return false; } }

        public override bool CanGetHurt { get { return false; } }

        public override bool CanLeaveArea { get { return false; } }

        public override bool CanStartAttacking { get { return false; } }

        public override bool CanStartUsingEquippableItem(EquippableItem item) { return false; }

        private Chest chest;

        public OpeningChestStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            chest = (Chest)associatedEntity;
            chest.StartOpening();
        }

        public override void Update(GamePadState gamepadState, GamePadState previousGamepadState)
        {
            if (chest.IsOpened)
            {
                End(PlayerState.Normal);
                player.EnterState(PlayerState.Normal);
            }
        }

        public override void End(PlayerState newState)
        {
            if (chest.Treasure != null)
            {
                Entity treasure = chest.Treasure;

                if (treasure is Pickup)
                    player.Inventory.CollectPickup((Pickup)treasure);
                else if (treasure is EquippableItem)
                    player.Inventory.CollectEquippableItem((EquippableItem)treasure);
            }
        }

        public override bool CanInteract
        {
            get { throw new NotImplementedException(); }
        }
    }
}

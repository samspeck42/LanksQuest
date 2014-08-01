using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class UsingBowStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.UsingBow; }
        }

        public override bool CanWalk
        {
            get { return true; }
        }

        public override bool CanGetHurt
        {
            get { return true; }
        }

        public override bool CanLeaveArea
        {
            get { return true; }
        }

        public override bool CanStartAttacking
        {
            get { return false; }
        }

        public override bool CanInteract
        {
            get { return false; }
        }

        public override bool CanChangeFaceDirection
        {
            get { return false; }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }

        Bow bow;

        public UsingBowStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            bow = (Bow)associatedEntity;
            bow.StartBeingUsed();
            player.IsAiming = true;
        }

        public override void Update(GameTime gameTime)
        {
            bow.Update(gameTime);

            if (bow.IsDoneBeingUsed)
            {
                player.IsAiming = false;
                bow = null;
                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            player.IsAiming = false;
            bow = null;
        }
    }
}

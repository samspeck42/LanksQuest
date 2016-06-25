using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adventure.Entities.Items;

namespace Adventure.PlayerStateHandlers
{
    public class NormalStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.Normal; }
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
            get { return true; }
        }

        public override bool CanInteract
        {
            get { return true; }
        }

        public override bool CanChangeFaceDirection
        {
            get { return true; }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return true;
        }

        public NormalStateHandler(Player player)
            : base(player) { }

        public override void Update(GameTime gameTime)
        {
            // update startToPushTimer if player is pushing against wall and check if player is ready to push
            //if (player.JustCollidedWithWall)
            //{
            //    if ((player.Velocity.Y == 0f && player.PreviousVelocity.Y == 0f && player.Velocity.X != 0f && player.Velocity.X == player.PreviousVelocity.X) ||
            //        (player.Velocity.X == 0f && player.PreviousVelocity.X == 0f && player.Velocity.Y != 0f && player.Velocity.Y == player.PreviousVelocity.Y))
            //        player.StartToPushTimer++;
            //    else
            //        player.StartToPushTimer = 0;
            //}
            //else
            //{
            //    player.StartToPushTimer = 0;
            //}

            //player.IsReadyToPush = player.StartToPushTimer >= Player.START_TO_PUSH_TIME ? true : false;
            //player.PreviousVelocity = new Vector2(player.Velocity.X, player.Velocity.Y);
        }
    }
}

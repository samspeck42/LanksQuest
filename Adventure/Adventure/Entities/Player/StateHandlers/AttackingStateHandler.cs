using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class AttackingStateHandler : PlayerStateHandler
    {
        public override PlayerState State
        {
            get { return PlayerState.Attacking; }
        }

        public override bool CanWalk
        {
            get { return false; }
        }

        public override bool CanGetHurt
        {
            get { return true; }
        }

        public override bool CanLeaveArea
        {
            get { return false; }
        }

        public override bool CanStartAttacking
        {
            get { return true; }
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

        public AttackingStateHandler(Player player)
            : base(player) { }

        public override void Start()
        {
            player.SpriteHandler.SetSpriteAttacking();
            player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = true;
            player.PlaySwordSwingSound();
        }

        public override void Update(GameTime gameTime)
        {
            //attackTimer++;
            //if (attackTimer >= Player.ATTACK_TIME)
            if (player.SpriteHandler.IsCurrentSpriteAttacking() && player.SpriteHandler.IsCurrentSpriteDoneAnimating)
            {
                //attackTimer = 0;
                //player.SwordHitPoint = Vector2.Zero;
                player.SpriteHandler.SetSpriteStill();
                player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = false;
                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            if (player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive)
                player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = false;
        }
    }
}

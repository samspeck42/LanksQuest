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

        public override void Start(Entity associatedEntity)
        {
            //attackTimer = 0;
            player.Velocity = Vector2.Zero;

            //if (player.FaceDirection == Directions4.Down)
            //{
            //    player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.ForwardSwordOrigin.X - Player.SWORD_LENGTH, player.BoundingBox.ActualY + player.ForwardSwordOrigin.Y);
            //}
            //else if (player.FaceDirection == Directions4.Up)
            //{
            //    player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.BackwardSwordOrigin.X + Player.SWORD_LENGTH, player.BoundingBox.ActualY + player.BackwardSwordOrigin.Y);
            //}
            //else if (player.FaceDirection == Directions4.Left)
            //{
            //    player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.LeftSwordOrigin.X, player.BoundingBox.ActualY + player.LeftSwordOrigin.Y - Player.SWORD_LENGTH);
            //}
            //else if (player.FaceDirection == Directions4.Right)
            //{
            //    player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.RightSwordOrigin.X, player.BoundingBox.ActualY + player.LeftSwordOrigin.Y - Player.SWORD_LENGTH);
            //}

            player.SpriteHandler.SetSpriteAttacking();
            player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = true;
            player.SwordSwingSound.Play(0.75f, 0f, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            //attackTimer++;
            //if (attackTimer >= Player.ATTACK_TIME)
            if (player.SpriteHandler.IsCurrentSpriteAttacking() && player.SpriteHandler.IsCurrentSpriteDoneAnimating())
            {
                //attackTimer = 0;
                //player.SwordHitPoint = Vector2.Zero;
                player.SpriteHandler.SetSpriteStill();
                player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = false;
                player.EnterState(new NormalStateHandler(player));
            }
            //else
            //{
            //    float angle = ((float)attackTimer / (float)(Player.ATTACK_TIME - 1)) * MathHelper.PiOver2;
            //    if (player.FaceDirection == Directions4.Left || player.FaceDirection == Directions4.Right)
            //        angle = MathHelper.PiOver2 - angle;
            //    int x = (int)(Math.Cos(angle) * Player.SWORD_LENGTH);
            //    int y = (int)(Math.Sin(angle) * Player.SWORD_LENGTH);

            //    if (player.FaceDirection == Directions4.Down)
            //    {
            //        player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.ForwardSwordOrigin.X - x, player.BoundingBox.ActualY + player.ForwardSwordOrigin.Y + y);
            //    }
            //    else if (player.FaceDirection == Directions4.Up)
            //    {
            //        player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.BackwardSwordOrigin.X + x, player.BoundingBox.ActualY + player.BackwardSwordOrigin.Y - y);
            //    }
            //    else if (player.FaceDirection == Directions4.Left)
            //    {
            //        player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.LeftSwordOrigin.X - x, player.BoundingBox.ActualY + player.LeftSwordOrigin.Y - y);
            //    }
            //    else if (player.FaceDirection == Directions4.Right)
            //    {
            //        player.SwordHitPoint = new Vector2(player.BoundingBox.ActualX + player.RightSwordOrigin.X + x, player.BoundingBox.ActualY + player.LeftSwordOrigin.Y - y);
            //    }

            //}
        }

        public override void End(PlayerState newState)
        {
            if (player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive)
                player.GetHitBoxById(Player.SWORD_HITBOX_ID).IsActive = false;
        }
    }
}

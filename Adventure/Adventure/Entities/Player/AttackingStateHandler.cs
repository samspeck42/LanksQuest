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

        private int attackTimer = 0;

        public AttackingStateHandler(Player player)
            : base(player) { }

        public override void Start(Entity associatedEntity)
        {
            attackTimer = 0;
            player.Velocity = Vector2.Zero;

            if (player.FaceDirection == Directions.Down)
            {
                player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.ForwardSwordOrigin.X - Player.SWORD_LENGTH, player.HitBoxPosition.Y + player.ForwardSwordOrigin.Y);
            }
            else if (player.FaceDirection == Directions.Up)
            {
                player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.BackwardSwordOrigin.X + Player.SWORD_LENGTH, player.HitBoxPosition.Y + player.BackwardSwordOrigin.Y);
            }
            else if (player.FaceDirection == Directions.Left)
            {
                player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.LeftSwordOrigin.X, player.HitBoxPosition.Y + player.LeftSwordOrigin.Y - Player.SWORD_LENGTH);
            }
            else if (player.FaceDirection == Directions.Right)
            {
                player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.RightSwordOrigin.X, player.HitBoxPosition.Y + player.LeftSwordOrigin.Y - Player.SWORD_LENGTH);
            }

            player.CurrentSprite = player.GetAttackingSprite(player.FaceDirection);

            player.CurrentSprite.ResetAnimation();
            player.SwordSwingSound.Play(0.75f, 0f, 0f);
        }

        public override void Update(GamePadState gamepadState, GamePadState previousGamepadState)
        {
            attackTimer++;
            if (attackTimer >= Player.ATTACK_TIME)
            {
                attackTimer = 0;
                player.SwordHitPoint = Vector2.Zero;
                player.CurrentSprite = player.GetStandingSprite(player.FaceDirection);
                player.EnterState(new NormalStateHandler(player));
            }
            else
            {
                float angle = ((float)attackTimer / (float)(Player.ATTACK_TIME - 1)) * MathHelper.PiOver2;
                if (player.FaceDirection == Directions.Left || player.FaceDirection == Directions.Right)
                    angle = MathHelper.PiOver2 - angle;
                int x = (int)(Math.Cos(angle) * Player.SWORD_LENGTH);
                int y = (int)(Math.Sin(angle) * Player.SWORD_LENGTH);

                if (player.FaceDirection == Directions.Down)
                {
                    player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.ForwardSwordOrigin.X - x, player.HitBoxPosition.Y + player.ForwardSwordOrigin.Y + y);
                }
                else if (player.FaceDirection == Directions.Up)
                {
                    player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.BackwardSwordOrigin.X + x, player.HitBoxPosition.Y + player.BackwardSwordOrigin.Y - y);
                }
                else if (player.FaceDirection == Directions.Left)
                {
                    player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.LeftSwordOrigin.X - x, player.HitBoxPosition.Y + player.LeftSwordOrigin.Y - y);
                }
                else if (player.FaceDirection == Directions.Right)
                {
                    player.SwordHitPoint = new Vector2(player.HitBoxPosition.X + player.RightSwordOrigin.X + x, player.HitBoxPosition.Y + player.LeftSwordOrigin.Y - y);
                }

            }
        }
    }
}

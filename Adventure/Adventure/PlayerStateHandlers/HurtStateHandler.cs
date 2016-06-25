using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adventure.Entities.MovementHandlers;
using Adventure.Entities;
using Adventure.Entities.Items;

namespace Adventure.PlayerStateHandlers
{
    public class HurtStateHandler : PlayerStateHandler
    {

        public override PlayerState State { get { return PlayerState.Hurt; } }

        public override bool CanWalk { get { return false; } }
        public override bool CanGetHurt { get { return false; } }
        public override bool CanLeaveArea { get { return false; } }
        public override bool CanStartAttacking { get { return false; } }
        public override bool CanInteract { get { return false; } }
        public override bool CanChangeFaceDirection { get { return false; } }


        private Entity damagingEntity;
        private KnockBackType knockBackType;

        public HurtStateHandler(Player player, Entity damagingEntity, KnockBackType knockBackType)
            : base(player) 
        {
            this.damagingEntity = damagingEntity;
            this.knockBackType = knockBackType;
        }

        public override void Start()
        {
            player.SpriteHandler.SetSpriteStill();
            player.StartInvincibility();
            player.PlayHitSound();

            if (knockBackType == KnockBackType.None)
            {
                player.EnterState(new NormalStateHandler(player));
            }
            else
            {
                Vector2 direction = player.Center - damagingEntity.Center;
                float angle = (float)Math.Atan2(direction.Y, direction.X);
                player.StartMovement(new StraightMovementHandler(player, 400, angle, 40));
            }
        }

        public override bool CanStartUsingEquippableItem(EquippableItem item)
        {
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (player.MovementHandler.IsFinished)
            {
                player.StartMovement(new PlayerMovementHandler(player));
                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            player.StartMovement(new PlayerMovementHandler(player));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adventure.Entities.Items;

namespace Adventure.PlayerStateHandlers
{
    public class UsingBowStateHandler : PlayerStateHandler
    {
        private const int CHARGE_TIME = 400;

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

        public override string StillSpritesId
        {
            get
            {
                return PlayerSpriteHandler.BOW_STILL_SPRITES;
            }
        }

        public override string WalkingSpritesId
        {
            get
            {
                return PlayerSpriteHandler.BOW_WALKING_SPRITES;
            }
        }

        public override float WalkSpeed
        {
            get
            {
                return 100f;
            }
        }


        int itemButtonNumber;
        int chargeTimer = 0;

        public UsingBowStateHandler(Player player, int itemButtonNumber)
            : base(player) 
        {
            this.itemButtonNumber = itemButtonNumber;
        }

        public override void Update(GameTime gameTime)
        {
            chargeTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (chargeTimer >= CHARGE_TIME && player.Input.IsEquippedItemButtonUp(itemButtonNumber))
            {
                chargeTimer = 0;
                Arrow arrow = new Arrow(player.GameWorld, player.GameWorld.CurrentMap, player.GameWorld.CurrentArea);
                arrow.LoadContent();
                if (player.FaceDirection == Directions4.Up)
                {
                    arrow.Position = new Vector2(
                        player.Position.X,
                        player.Position.Y - 10);
                }
                else if (player.FaceDirection == Directions4.Down)
                {
                    arrow.Position = new Vector2(
                        player.Position.X,
                        player.Position.Y - 14);
                }
                else if (player.FaceDirection == Directions4.Left)
                {
                    arrow.Position = new Vector2(
                        player.Position.X - 6,
                        player.Position.Y - 16);
                }
                else if (player.FaceDirection == Directions4.Right)
                {
                    arrow.Position = new Vector2(
                        player.Position.X + 6,
                        player.Position.Y - 16);
                }
                arrow.Fire(player.FaceDirection);
                player.PlayArrowFireSound();

                player.EnterState(new NormalStateHandler(player));
            }
        }

        public override void End(PlayerState newState)
        {
            chargeTimer = 0;
        }
    }
}

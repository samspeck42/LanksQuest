using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    public class GrabbingStateHandler : PlayerStateHandler
    {
        private const int START_TO_PUSH_TIME = 500;
        private const int START_TO_PULL_TIME = 500;

        public override PlayerState State
        {
            get { return PlayerState.Grabbing; }
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
                return PlayerSpriteHandler.GRABBING_SPRITES_ID;
            }
        }


        private MovableBlock movableBlock;
        private Directions4 pushDirection;
        private Directions4 pullDirection;
        private int startToPushTimer = 0;
        private int startToPullTimer = 0;

        public GrabbingStateHandler(Player player, MovableBlock movableBlock)
            : base(player)
        {
            this.movableBlock = movableBlock;
            this.pushDirection = player.FaceDirection;
            this.pullDirection = DirectionsHelper.Opposite(player.FaceDirection);
        }

        public override void Start()
        {
            player.SpriteHandler.SetSpriteStill();
        }

        public override void Update(GameTime gameTime)
        {
            bool isPushing = false;
            bool isPulling = false;
            if (player.Input.InputDirection == DirectionsHelper.ToDirections(pushDirection))
            {
                // player is trying to push block
                startToPushTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!player.SpriteHandler.IsCurrentSprite(PlayerSpriteHandler.PUSHING_SPRITES_ID))
                    player.SpriteHandler.SetSprite(PlayerSpriteHandler.PUSHING_SPRITES_ID);
                isPushing = true;
            }
            else
            {
                startToPushTimer = 0;
            }

            if (player.Input.InputDirection == DirectionsHelper.ToDirections(pullDirection))
            {
                // player is trying to pull block
                startToPullTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!player.SpriteHandler.IsCurrentSprite(PlayerSpriteHandler.PUSHING_SPRITES_ID))
                    player.SpriteHandler.SetSprite(PlayerSpriteHandler.PUSHING_SPRITES_ID);
                isPulling = true;
            }
            else
            {
                startToPullTimer = 0;
            }

            if (!(isPulling || isPushing) && !player.SpriteHandler.IsCurrentSpriteStill())
                player.SpriteHandler.SetSpriteStill();

            if (startToPushTimer >= START_TO_PUSH_TIME)
            {
                bool startedPush = movableBlock.TryToStartMoving(pushDirection);
                if (startedPush)
                {
                    startToPushTimer = 0;
                    startToPullTimer = 0;
                    player.EnterState(new PushingStateHandler(player, pushDirection));
                }
            }
            if (startToPullTimer >= START_TO_PULL_TIME)
            {
                bool startedPull = movableBlock.TryToStartMoving(pullDirection);
                if (startedPull)
                {
                    startToPushTimer = 0;
                    startToPullTimer = 0;
                    player.EnterState(new PullingStateHandler(player, pullDirection));
                }
            }
        }

        public override void End(PlayerState newState)
        {
            startToPushTimer = 0;
            startToPullTimer = 0;
        }

        public override void OnInteractButtonReleased()
        {
            startToPushTimer = 0;
            startToPullTimer = 0;
            player.EnterState(new NormalStateHandler(player));
        }
    }
}

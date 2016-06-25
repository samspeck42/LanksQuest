using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TileEngine;
using Adventure.Maps;
using Adventure.Entities.MovementHandlers;

namespace Adventure.Entities.Enemies
{
    public class BlobEnemy : Enemy, Triggerable
    {
        private const string NORMAL_SPRITE_ID = "normal_sprite";
        private const string OBSTACLE_COLLISION_BOX_ID = "obstacle_collision_box";

        private const float WALK_SPEED = 14;
        private const int WALK_ANIMATION_DELAY = 100;
        private const int JUMP_WAIT_TIME = 2000;

        public override HitBox ObstacleCollisionBox { get { return GetHitBoxById(OBSTACLE_COLLISION_BOX_ID); } }
        public override int Damage { get { return 1; } }
        public override bool IsBlockedByObstacleEntities { get { return true; } }
        public override int MaxHealth { get { return 2; } }
        public override TileCollision ObstacleTileCollisions
        {
            get
            {
                return TileCollision.Wall | TileCollision.Doorway;
            }
        }

        private bool isJumping { get { return movementHandler is BounceMovementHandler; } }
        private bool canJump = true;
        private int jumpWaitTimer = 0;

        public BlobEnemy(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            BoundingBox.RelativeX = -11;
            BoundingBox.RelativeY = -12;
            BoundingBox.Width = 22;
            BoundingBox.Height = 24;

            HitBox obstacleCollisionBox = new HitBox(this, OBSTACLE_COLLISION_BOX_ID);
            obstacleCollisionBox.RelativeX = -11;
            obstacleCollisionBox.RelativeY = -12;
            obstacleCollisionBox.Width = 22;
            obstacleCollisionBox.Height = 24;
            obstacleCollisionBox.IsActive = true;
            HitBoxes.Add(obstacleCollisionBox);

            Vector2 origin = new Vector2(13, 14);
            Sprite sprite = new Sprite("Sprites/Enemies/blob_enemy", this, origin, 8, WALK_ANIMATION_DELAY);
            spriteHandler.AddSprite(NORMAL_SPRITE_ID, sprite);
            spriteHandler.SetSprite(NORMAL_SPRITE_ID);

            movementHandler = new ChaseMovementHandler(this, WALK_SPEED, game.Player);
            movementHandler.Start();
        }

        public override bool DamagesPlayer(HitBox thisHitBox, out KnockBackType knockBackType)
        {
            knockBackType = KnockBackType.HitAngle;
            return enemyState == EnemyState.Normal && thisHitBox.IsId(BOUNDING_BOX_ID);
        }

        public override bool TakesDamageFromPlayerSword(HitBox thisHitBox)
        {
            return thisHitBox.IsId(BOUNDING_BOX_ID);
        }

        public override bool TakesDamageFromPot(HitBox thisHitBox)
        {
            return thisHitBox.IsId(BOUNDING_BOX_ID);
        }

        public override bool TakesDamageFromArrow(HitBox thisHitBox)
        {
            return thisHitBox.IsId(BOUNDING_BOX_ID);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void updateAI(GameTime gameTime)
        {
            if (!isJumping)
            {
                if (canJump)
                {
                    Vector2 directionToPlayer = gameWorld.Player.Center - this.Center;
                    float distanceToPlayer = directionToPlayer.Length();

                    if (distanceToPlayer < 100)
                    {
                        // start jumping
                        movementHandler = new BounceMovementHandler(this,
                            160,
                            (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X),
                            240, 0, 0, 0, 0);
                        movementHandler.Start();
                    }
                }
                else
                {
                    jumpWaitTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (jumpWaitTimer >= JUMP_WAIT_TIME)
                        canJump = true;
                }
            }
            else if (isJumping && movementHandler.IsFinished)
            {
                // done jumping
                canJump = false;
                jumpWaitTimer = 0;

                movementHandler = new ChaseMovementHandler(this, WALK_SPEED, gameWorld.Player);
                movementHandler.Start();
            }

            //moveTimer++;
            //if (moveTimer >= MOVE_TIME)
            //{
            //    moveTimer = 0;
            //    Velocity = Vector2.Zero;

            //    currentMove++;
            //    if (currentMove >= moves.Count())
            //    {
            //        currentMove = 0;
            //        List<int> temp = new List<int>();
            //        foreach (int k in moves)
            //            temp.Add(k);
            //        for (int k = 0; k < moves.Count(); k++)
            //        {
            //            int r = GameWorld.Random.Next(temp.Count);
            //            moves[k] = temp.ElementAt(r);
            //            temp.RemoveAt(r);
            //        }

            //    }

            //    int n = moves[currentMove];
            //    switch (n)
            //    {
            //        case 0:
            //            Velocity.Y += WALK_SPEED;
            //            break;
            //        case 1:
            //            Velocity.Y -= WALK_SPEED;
            //            break;
            //        case 2:
            //            Velocity.X += WALK_SPEED;
            //            break;
            //        case 3:
            //            Velocity.X -= WALK_SPEED;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        protected override void onHurtRecovery()
        {
            if (movementHandler is BounceMovementHandler)
            {
                BounceMovementHandler bounceMovementHandler = (BounceMovementHandler)movementHandler;
                bounceMovementHandler.GroundVelocityX = 0;
                bounceMovementHandler.GroundVelocityY = 0;
            }
        }

        public void TriggerOn()
        {
            isVisible = true;
        }

        public void TriggerOff()
        {
            
        }
    }
}

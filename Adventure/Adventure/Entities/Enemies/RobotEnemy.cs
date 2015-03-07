using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TileEngine;

namespace Adventure
{
    public class RobotEnemy : Enemy, Triggerable
    {
        private const string ASLEEP_SPRITE_ID = "asleep_sprite";
        private const string WAKING_SPRITE_ID = "waking_sprite";
        private const string AWAKE_SPRITES_ID = "awake_sprites";
        private const string OBSTACLE_COLLISION_BOX_ID = "obstacle_collision_box";
        private const string EYE_HIT_BOX_ID = "eye_hit_box";
        private const int JUMP_WAIT_TIME = 500;

        public override HitBox ObstacleCollisionBox { get { return GetHitBoxById(OBSTACLE_COLLISION_BOX_ID); } }
        public override int Damage { get { return 1; } }
        public override int MaxHealth { get { return 4; } }
        public override bool IsBlockedByObstacleEntities { get { return true; } }
        public override TileCollision ObstacleTileCollisions
        {
            get
            {
                return TileCollision.Wall | TileCollision.Doorway;
            }
        }
        public override bool IsObstacle
        {
            get
            {
                return robotEnemyState == RobotEnemyState.Asleep;
            }
        }

        const int WAKE_ANIMATION_DELAY = 112;
        const int MOVE_TIME = 50;
        const int FORWARD_EYE_OFFSET_X = 3;
        const int FORWARD_EYE_OFFSET_Y = 3;
        const int LEFT_EYE_OFFSET_X = 0;
        const int LEFT_EYE_OFFSET_Y = 3;
        const int RIGHT_EYE_OFFSET_X = 13;
        const int RIGHT_EYE_OFFSET_Y = 3;
        const int BACKWARD_EYE_OFFSET_X = 3;
        const int BACKWARD_EYE_OFFSET_Y = 0;
        const int MAX_EYE_OPEN_TIME = 200;
        const int MIN_EYE_OPEN_TIME = 150;
        const int MAX_EYE_CLOSED_TIME = 120;
        const int MIN_EYE_CLOSED_TIME = 70;
        const int ALERT_RADIUS = 100;

        private bool isJumping { get { return movementHandler is BounceMovementHandler; } }
        private int jumpWaitTimer = 0;
        //private Vector2 currentEyeOffset;
        //private bool isEyeOpened = false;
        //private int eyeTimer = 0;
        //private bool isAsleep = false;
        private bool isAlert = false;
        private RobotEnemyState robotEnemyState = RobotEnemyState.Asleep;
        //private bool isWaking = false;
        //private int eyeClosedTime = 0;
        //private int eyeOpenTime = 0;

        //private Sprite forwardSprite;
        //private Sprite backwardSprite;
        //private Sprite leftSprite;
        //private Sprite rightSprite;
        //private Sprite leftEyeSprite;
        //private Sprite rightEyeSprite;
        //private Sprite forwardEyeSprite;
        //private Sprite leftEyeSpriteClosed;
        //private Sprite rightEyeSpriteClosed;
        //private Sprite forwardEyeSpriteClosed;
        //private Sprite currentEyeSprite;
        //private Sprite asleepSprite;
        //private Sprite wakingSprite;

        //public Rectangle EyeHitBox
        //{
        //    get
        //    {
        //        return new Rectangle((int)Math.Round(Position.X + currentEyeOffset.X), (int)Math.Round(Position.Y + currentEyeOffset.Y),
        //          currentEyeSprite.Texture.Width, currentEyeSprite.Texture.Height);
        //    }
        //}

        public RobotEnemy(GameWorld game, Area area)
            : base(game, area)
        {
            BoundingBox.RelativeX = -11;
            BoundingBox.RelativeY = -28;
            BoundingBox.Width = 22;
            BoundingBox.Height = 32;

            HitBox obstacleCollisionBox = new HitBox(this, OBSTACLE_COLLISION_BOX_ID);
            obstacleCollisionBox.RelativeX = -11;
            obstacleCollisionBox.RelativeY = -18;
            obstacleCollisionBox.Width = 22;
            obstacleCollisionBox.Height = 22;
            obstacleCollisionBox.IsActive = true;
            HitBoxes.Add(obstacleCollisionBox);

            HitBox eyeHitBox = new HitBox(this, EYE_HIT_BOX_ID);
            eyeHitBox.RelativeX = -9;
            eyeHitBox.RelativeY = -19;
            eyeHitBox.Width = 18;
            eyeHitBox.Height = 12;
            eyeHitBox.IsActive = false;
            HitBoxes.Add(eyeHitBox);

            Vector2 origin = new Vector2(16, 28);
            Sprite sprite = new Sprite("Sprites/Enemies/robot_enemy_asleep", this, origin);
            spriteHandler.AddSprite(ASLEEP_SPRITE_ID, sprite);

            sprite = new Sprite("Sprites/Enemies/robot_enemy_waking", this, origin, 4, WAKE_ANIMATION_DELAY, 1);
            spriteHandler.AddSprite(WAKING_SPRITE_ID, sprite);

            SpriteSet spriteSet = new SpriteSet();
            sprite = new Sprite("Sprites/Enemies/robot_enemy_backward2", this, origin);
            spriteSet.SetSprite(Directions4.Up, sprite);
            sprite = new Sprite("Sprites/Enemies/robot_enemy_forward2", this, origin);
            spriteSet.SetSprite(Directions4.Down, sprite);
            sprite = new Sprite("Sprites/Enemies/robot_enemy_left2", this, origin);
            spriteSet.SetSprite(Directions4.Left, sprite);
            sprite = new Sprite("Sprites/Enemies/robot_enemy_right2", this, origin);
            spriteSet.SetSprite(Directions4.Right, sprite);
            spriteHandler.AddSpriteSet(AWAKE_SPRITES_ID, spriteSet);

            spriteHandler.SetSprite(ASLEEP_SPRITE_ID);
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            if (dataDict.ContainsKey("isAlert"))
                isAlert = bool.Parse(dataDict["isAlert"]);
        }

        public override bool DamagesPlayer(HitBox thisHitBox, out KnockBackType knockBackType)
        {
            knockBackType = KnockBackType.HitAngle;
            return robotEnemyState == RobotEnemyState.Awake && 
                enemyState == EnemyState.Normal &&
                thisHitBox.IsId(BOUNDING_BOX_ID);
        }

        public override bool TakesDamageFromArrow(HitBox thisHitBox)
        {
            return robotEnemyState == RobotEnemyState.Awake && thisHitBox.IsId(EYE_HIT_BOX_ID);
        }

        public override bool TakesDamageFromPlayerSword(HitBox thisHitBox)
        {
            return robotEnemyState == RobotEnemyState.Awake && thisHitBox.IsId(EYE_HIT_BOX_ID);
        }

        public override bool TakesDamageFromPot(HitBox thisHitBox)
        {
            return robotEnemyState == RobotEnemyState.Awake && thisHitBox.IsId(EYE_HIT_BOX_ID);
        }

        protected override void updateAI(GameTime gameTime)
        {
            if (robotEnemyState == RobotEnemyState.Asleep)
            {
                if (isAlert)
                {
                    float playerDistance = Vector2.Distance(game.Player.Center, this.Center);

                    if (playerDistance <= ALERT_RADIUS)
                    {
                        robotEnemyState = RobotEnemyState.Waking;
                        spriteHandler.SetSprite(WAKING_SPRITE_ID);
                    }
                }
            }
            else if (robotEnemyState == RobotEnemyState.Waking)
            {
                if (spriteHandler.IsCurrentSpriteDoneAnimating)
                {
                    robotEnemyState = RobotEnemyState.Awake;
                    spriteHandler.SetSprite(AWAKE_SPRITES_ID);
                    GetHitBoxById(EYE_HIT_BOX_ID).IsActive = true;
                }
            }
            else if (robotEnemyState == RobotEnemyState.Awake)
            {
                if (!isJumping)
                {
                    jumpWaitTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (jumpWaitTimer >= JUMP_WAIT_TIME)
                    {
                        Vector2 directionToPlayer = game.Player.Center - this.Center;
                        float angleToPlayer = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);

                        if (angleToPlayer >= -(3 * MathHelper.PiOver4) && angleToPlayer <= -MathHelper.PiOver4)
                            FaceDirection = Directions4.Up;
                        else if (angleToPlayer >= -MathHelper.PiOver4 && angleToPlayer <= MathHelper.PiOver4)
                            FaceDirection = Directions4.Right;
                        else if (angleToPlayer >= MathHelper.PiOver4 && angleToPlayer <= (3 * MathHelper.PiOver4))
                            FaceDirection = Directions4.Down;
                        else
                            FaceDirection = Directions4.Left;

                        movementHandler = new BounceMovementHandler(this,
                            100,
                            angleToPlayer,
                            180, 0, 0, 0, 0);
                        movementHandler.Start();
                    }
                }
                else if (isJumping && movementHandler.IsFinished)
                {
                    jumpWaitTimer = 0;
                    movementHandler = null;
                }
            }

            //if (!isAsleep)
            //{
            //    moveTimer++;
            //    if (moveTimer >= MOVE_TIME)
            //    {
            //        moveTimer = 0;
            //        Velocity = Vector2.Zero;

            //        Directions4 dir = Directions4.Down;

            //        float playerDistanceX = game.Player.Center.X - this.Center.X;
            //        float playerDistanceY = game.Player.Center.Y - this.Center.Y;

            //        if ((Math.Abs(playerDistanceX) >= Math.Abs(playerDistanceY)) && playerDistanceX >= 0)
            //            dir = Directions4.Right;
            //        else if ((Math.Abs(playerDistanceX) >= Math.Abs(playerDistanceY)) && playerDistanceX < 0)
            //            dir = Directions4.Left;
            //        else if ((Math.Abs(playerDistanceX) < Math.Abs(playerDistanceY)) && playerDistanceY >= 0)
            //            dir = Directions4.Down;
            //        else if ((Math.Abs(playerDistanceX) < Math.Abs(playerDistanceY)) && playerDistanceY < 0)
            //            dir = Directions4.Up;

            //        changeDirection(dir);
            //    }

            //    eyeTimer++;
            //    if (isEyeOpened && eyeTimer >= eyeOpenTime)
            //        closeEye();
            //    else if (!isEyeOpened && eyeTimer >= eyeClosedTime)
            //        openEye();
            //}

            //else if (isAlert)
            //{
            //    float playerDistance = Vector2.Distance(game.Player.Center, this.Center);

            //    if (playerDistance <= ALERT_RADIUS)
            //    {
            //        isAlert = false;
            //        startWaking();
            //    }
            //}
            //else if (isWaking)
            //{
            //    if (CurrentSprite.IsDoneAnimating)
            //    {
            //        isWaking = false;
            //        wake();
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

        //private void startWaking()
        //{
        //    isWaking = true;
        //    CurrentSprite = wakingSprite;
        //    CurrentSprite.ResetAnimation();
        //}

        //private void changeDirection(Directions4 direction)
        //{
        //    FaceDirection = direction;

        //    switch (direction)
        //    {
        //        case Directions4.Down:
        //            Velocity.Y += WALK_SPEED;
        //            CurrentSprite = forwardSprite;
        //            if (isEyeOpened)
        //                currentEyeSprite = forwardEyeSprite;
        //            else
        //                currentEyeSprite = forwardEyeSpriteClosed;
        //            currentEyeOffset = new Vector2(FORWARD_EYE_OFFSET_X, FORWARD_EYE_OFFSET_Y);
        //            break;
        //        case Directions4.Up:
        //            Velocity.Y -= WALK_SPEED;
        //            CurrentSprite = backwardSprite;
        //            if (isEyeOpened)
        //                currentEyeSprite = forwardEyeSprite;
        //            else
        //                currentEyeSprite = forwardEyeSpriteClosed;
        //            currentEyeOffset = new Vector2(BACKWARD_EYE_OFFSET_X, BACKWARD_EYE_OFFSET_Y);
        //            break;
        //        case Directions4.Right:
        //            Velocity.X += WALK_SPEED;
        //            CurrentSprite = rightSprite;
        //            if (isEyeOpened)
        //                currentEyeSprite = rightEyeSprite;
        //            else
        //                currentEyeSprite = rightEyeSpriteClosed;
        //            currentEyeOffset = new Vector2(RIGHT_EYE_OFFSET_X, RIGHT_EYE_OFFSET_Y);
        //            break;
        //        case Directions4.Left:
        //            Velocity.X -= WALK_SPEED;
        //            CurrentSprite = leftSprite;
        //            if (isEyeOpened)
        //                currentEyeSprite = leftEyeSprite;
        //            else
        //                currentEyeSprite = leftEyeSpriteClosed;
        //            currentEyeOffset = new Vector2(LEFT_EYE_OFFSET_X, LEFT_EYE_OFFSET_Y);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void openEye()
        //{
        //    isEyeOpened = true;
        //    eyeTimer = 0;
        //    eyeOpenTime = GameWorld.Random.Next(MIN_EYE_OPEN_TIME, MAX_EYE_OPEN_TIME);

        //    if (FaceDirection == Directions4.Up || FaceDirection == Directions4.Down)
        //        currentEyeSprite = forwardEyeSprite;
        //    else if (FaceDirection == Directions4.Left)
        //        currentEyeSprite = leftEyeSprite;
        //    else if (FaceDirection == Directions4.Right)
        //        currentEyeSprite = rightEyeSprite;
        //}

        //private void closeEye()
        //{
        //    isEyeOpened = false;
        //    eyeTimer = 0;
        //    eyeClosedTime = GameWorld.Random.Next(MIN_EYE_CLOSED_TIME, MAX_EYE_CLOSED_TIME);

        //    if (FaceDirection == Directions4.Up || FaceDirection == Directions4.Down)
        //        currentEyeSprite = forwardEyeSpriteClosed;
        //    else if (FaceDirection == Directions4.Left)
        //        currentEyeSprite = leftEyeSpriteClosed;
        //    else if (FaceDirection == Directions4.Right)
        //        currentEyeSprite = rightEyeSpriteClosed;
        //}

        //private void wake()
        //{
        //    isAsleep = false;
        //    CurrentSprite = forwardSprite;
        //    FaceDirection = Directions4.Down;
        //    openEye();
        //    IsPassable = true;
        //    doesDamageOnContact = true;
        //}

        //public override void OnEntityCollision(Entity other)
        //{
        //    if (other is Arrow)
        //    {
        //        Arrow arrow = (Arrow)other;
        //        if ((arrow.FaceDirection == Directions4.Up && this.FaceDirection == Directions4.Down) ||
        //            (arrow.FaceDirection == Directions4.Down && this.FaceDirection == Directions4.Up) ||
        //            (arrow.FaceDirection == Directions4.Left && this.FaceDirection == Directions4.Right) ||
        //            (arrow.FaceDirection == Directions4.Right && this.FaceDirection == Directions4.Left))
        //        {
        //            //if (!IsHurt 
        //            if (enemyState == EnemyState.Normal && arrow.IsFired && eyeHitBoxContains(arrow.TipPosition))
        //            {
        //                if (isEyeOpened && !isAsleep)
        //                {
        //                    takeDamageFrom(arrow);
        //                    closeEye();
        //                }
        //                arrow.HitEntity(this);
        //            }
        //        }
        //        else if (arrow.IsFired && this.Contains(arrow.TipPosition))
        //        {
        //            arrow.HitEntity(this);
        //        }
        //    }
        //}

        //private bool eyeHitBoxContains(Vector2 position)
        //{
        //    return this.EyeHitBox.Contains(new Point((int)Math.Round(position.X), (int)Math.Round(position.Y)));
        //}

        //public override void ReactToSwordHit(Player player)
        //{
        //    player.KnockBack(this);
        //}

        //public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        //{
        //    base.Draw(spriteBatch, changeColorsEffect);

        //    if (!isAsleep && FaceDirection != Directions4.Up)
        //        currentEyeSprite.Draw(spriteBatch, new Vector2(EyeHitBox.X, EyeHitBox.Y));
        //}

        public void TriggerOn()
        {
            isAlert = true;
        }

        public void TriggerOff()
        {
            isAlert = false;
        }

        private enum RobotEnemyState
        {
            Asleep,
            Waking,
            Awake
        }
    }
}

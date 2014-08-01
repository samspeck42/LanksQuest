﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class RobotEnemy : Enemy, PickupDropper, Activatable
    {
        private const float PICKUP_DROP_CHANCE = 0.75f;

        const float WALK_SPEED = 1f;
        const int WALK_ANIMATION_DELAY = 6;
        const int WAKE_ANIMATION_DELAY = 7;
        const int MOVE_TIME = 50;
        const int MAX_HEALTH = 6;
        const int DAMAGE = 1;
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

        private int moveTimer = MOVE_TIME;
        private Vector2 currentEyeOffset;
        private bool isEyeOpened = false;
        private int eyeTimer = 0;
        private bool isAsleep = false;
        private bool isAlert = false;
        private bool isWaking = false;
        private int eyeClosedTime = 0;
        private int eyeOpenTime = 0;

        private Sprite forwardSprite;
        private Sprite backwardSprite;
        private Sprite leftSprite;
        private Sprite rightSprite;
        private Sprite leftEyeSprite;
        private Sprite rightEyeSprite;
        private Sprite forwardEyeSprite;
        private Sprite leftEyeSpriteClosed;
        private Sprite rightEyeSpriteClosed;
        private Sprite forwardEyeSpriteClosed;
        private Sprite currentEyeSprite;
        private Sprite asleepSprite;
        private Sprite wakingSprite;

        public Rectangle EyeHitBox
        {
            get
            {
                return new Rectangle((int)Math.Round(Position.X + currentEyeOffset.X), (int)Math.Round(Position.Y + currentEyeOffset.Y),
                  currentEyeSprite.Texture.Width, currentEyeSprite.Texture.Height);
            }
        }

        public RobotEnemy(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 22;
            hitBoxHeight = 32;

            Vector2 origin = new Vector2(5, 8);
            forwardSprite = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            backwardSprite = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            leftSprite = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            rightSprite = new Sprite(origin, 4, WALK_ANIMATION_DELAY);
            asleepSprite = new Sprite(origin);
            wakingSprite = new Sprite(origin, 4, WAKE_ANIMATION_DELAY, 1);

            origin = new Vector2(0, 0);
            forwardEyeSprite = new Sprite(origin);
            forwardEyeSpriteClosed = new Sprite(origin);

            origin = new Vector2(0, 0);
            leftEyeSprite = new Sprite(origin);
            leftEyeSpriteClosed = new Sprite(origin);
            rightEyeSprite = new Sprite(origin);
            rightEyeSpriteClosed = new Sprite(origin);

            CurrentSprite = forwardSprite;
            currentEyeSprite = forwardEyeSprite;
            currentEyeOffset = new Vector2(FORWARD_EYE_OFFSET_X, FORWARD_EYE_OFFSET_Y);

            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;

            CanLeaveArea = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            forwardSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_forward");
            backwardSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_backward");
            leftSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_left");
            rightSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_right");
            forwardEyeSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_forward");
            leftEyeSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_left");
            rightEyeSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_right");
            forwardEyeSpriteClosed.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_forward_closed");
            leftEyeSpriteClosed.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_left_closed");
            rightEyeSpriteClosed.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_eye_right_closed");
            asleepSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_asleep");
            wakingSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/robot_enemy_waking");
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("isAsleep"))
                isAsleep = bool.Parse(dataDict["isAsleep"]);

            if (isAsleep)
            {
                CurrentSprite = asleepSprite;
                IsPassable = false;
                doesDamageOnContact = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        protected override void updateAI()
        {
            if (!isAsleep)
            {
                moveTimer++;
                if (moveTimer >= MOVE_TIME)
                {
                    moveTimer = 0;
                    Velocity = Vector2.Zero;

                    Directions4 dir = Directions4.Down;

                    float playerDistanceX = game.Player.Center.X - this.Center.X;
                    float playerDistanceY = game.Player.Center.Y - this.Center.Y;

                    if ((Math.Abs(playerDistanceX) >= Math.Abs(playerDistanceY)) && playerDistanceX >= 0)
                        dir = Directions4.Right;
                    else if ((Math.Abs(playerDistanceX) >= Math.Abs(playerDistanceY)) && playerDistanceX < 0)
                        dir = Directions4.Left;
                    else if ((Math.Abs(playerDistanceX) < Math.Abs(playerDistanceY)) && playerDistanceY >= 0)
                        dir = Directions4.Down;
                    else if ((Math.Abs(playerDistanceX) < Math.Abs(playerDistanceY)) && playerDistanceY < 0)
                        dir = Directions4.Up;

                    changeDirection(dir);
                }

                eyeTimer++;
                if (isEyeOpened && eyeTimer >= eyeOpenTime)
                    closeEye();
                else if (!isEyeOpened && eyeTimer >= eyeClosedTime)
                    openEye();
            }

            else if (isAlert)
            {
                float playerDistance = Vector2.Distance(game.Player.Center, this.Center);

                if (playerDistance <= ALERT_RADIUS)
                {
                    isAlert = false;
                    startWaking();
                }
            }
            else if (isWaking)
            {
                if (CurrentSprite.IsDoneAnimating)
                {
                    isWaking = false;
                    wake();
                }
            }
        }

        private void startWaking()
        {
            isWaking = true;
            CurrentSprite = wakingSprite;
            CurrentSprite.ResetAnimation();
        }

        private void changeDirection(Directions4 direction)
        {
            FaceDirection = direction;

            switch (direction)
            {
                case Directions4.Down:
                    Velocity.Y += WALK_SPEED;
                    CurrentSprite = forwardSprite;
                    if (isEyeOpened)
                        currentEyeSprite = forwardEyeSprite;
                    else
                        currentEyeSprite = forwardEyeSpriteClosed;
                    currentEyeOffset = new Vector2(FORWARD_EYE_OFFSET_X, FORWARD_EYE_OFFSET_Y);
                    break;
                case Directions4.Up:
                    Velocity.Y -= WALK_SPEED;
                    CurrentSprite = backwardSprite;
                    if (isEyeOpened)
                        currentEyeSprite = forwardEyeSprite;
                    else
                        currentEyeSprite = forwardEyeSpriteClosed;
                    currentEyeOffset = new Vector2(BACKWARD_EYE_OFFSET_X, BACKWARD_EYE_OFFSET_Y);
                    break;
                case Directions4.Right:
                    Velocity.X += WALK_SPEED;
                    CurrentSprite = rightSprite;
                    if (isEyeOpened)
                        currentEyeSprite = rightEyeSprite;
                    else
                        currentEyeSprite = rightEyeSpriteClosed;
                    currentEyeOffset = new Vector2(RIGHT_EYE_OFFSET_X, RIGHT_EYE_OFFSET_Y);
                    break;
                case Directions4.Left:
                    Velocity.X -= WALK_SPEED;
                    CurrentSprite = leftSprite;
                    if (isEyeOpened)
                        currentEyeSprite = leftEyeSprite;
                    else
                        currentEyeSprite = leftEyeSpriteClosed;
                    currentEyeOffset = new Vector2(LEFT_EYE_OFFSET_X, LEFT_EYE_OFFSET_Y);
                    break;
                default:
                    break;
            }
        }

        private void openEye()
        {
            isEyeOpened = true;
            eyeTimer = 0;
            eyeOpenTime = GameWorld.Random.Next(MIN_EYE_OPEN_TIME, MAX_EYE_OPEN_TIME);

            if (FaceDirection == Directions4.Up || FaceDirection == Directions4.Down)
                currentEyeSprite = forwardEyeSprite;
            else if (FaceDirection == Directions4.Left)
                currentEyeSprite = leftEyeSprite;
            else if (FaceDirection == Directions4.Right)
                currentEyeSprite = rightEyeSprite;
        }

        private void closeEye()
        {
            isEyeOpened = false;
            eyeTimer = 0;
            eyeClosedTime = GameWorld.Random.Next(MIN_EYE_CLOSED_TIME, MAX_EYE_CLOSED_TIME);

            if (FaceDirection == Directions4.Up || FaceDirection == Directions4.Down)
                currentEyeSprite = forwardEyeSpriteClosed;
            else if (FaceDirection == Directions4.Left)
                currentEyeSprite = leftEyeSpriteClosed;
            else if (FaceDirection == Directions4.Right)
                currentEyeSprite = rightEyeSpriteClosed;
        }

        public float DropChance
        {
            get { return PICKUP_DROP_CHANCE; }
        }

        public void Activate()
        {
            isAlert = true;
        }

        private void wake()
        {
            isAsleep = false;
            CurrentSprite = forwardSprite;
            FaceDirection = Directions4.Down;
            openEye();
            IsPassable = true;
            doesDamageOnContact = true;
        }

        public void Deactivate()
        {
        }

        public Pickup SpawnPickup()
        {
            List<PickupType> possibleTypes = new List<PickupType>();
            possibleTypes.Add(PickupType.BronzeCoin);
            possibleTypes.Add(PickupType.SilverCoin);
            possibleTypes.Add(PickupType.GoldCoin);
            possibleTypes.Add(PickupType.Heart);

            Pickup pickup = new Pickup(game, area, possibleTypes.ElementAt(GameWorld.Random.Next(possibleTypes.Count)), true);
            pickup.Center = this.Center;
            return pickup;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Arrow)
            {
                Arrow arrow = (Arrow)other;
                if ((arrow.FaceDirection == Directions4.Up && this.FaceDirection == Directions4.Down) ||
                    (arrow.FaceDirection == Directions4.Down && this.FaceDirection == Directions4.Up) ||
                    (arrow.FaceDirection == Directions4.Left && this.FaceDirection == Directions4.Right) ||
                    (arrow.FaceDirection == Directions4.Right && this.FaceDirection == Directions4.Left))
                {
                    //if (!IsHurt 
                    if (state == EnemyState.Normal && arrow.IsFired && eyeHitBoxContains(arrow.TipPosition))
                    {
                        if (isEyeOpened && !isAsleep)
                        {
                            takeDamageFrom(arrow);
                            closeEye();
                        }
                        arrow.HitEntity(this);
                    }
                }
                else if (arrow.IsFired && this.Contains(arrow.TipPosition))
                {
                    arrow.HitEntity(this);
                }
            }
        }

        private bool eyeHitBoxContains(Vector2 position)
        {
            return this.EyeHitBox.Contains(new Point((int)Math.Round(position.X), (int)Math.Round(position.Y)));
        }

        public override void ReactToSwordHit(Player player)
        {
            player.KnockBack(this);
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            base.Draw(spriteBatch, changeColorsEffect);

            if (!isAsleep && FaceDirection != Directions4.Up)
                currentEyeSprite.Draw(spriteBatch, new Vector2(EyeHitBox.X, EyeHitBox.Y));
        }
    }
}

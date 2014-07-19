using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class BossHead : Enemy
    {
        private const float MOVE_SPEED = 1.4f;
        private const int EYE_OPEN_ANIMATION_DELAY = 6;
        private const int EYE_CLOSE_ANIMATION_DELAY = 6;
        private const int EYE_STUNNED_ANIMATION_DELAY = 2;
        private const int EYE_OFFSET_X = 28;
        private const int EYE_OFFSET_Y = 32;
        private const int MOUTH_OFFSET_X = 18;
        private const int MOUTH_OFFSET_Y = 63;
        private const int MAX_EYE_OPEN_TIME = 180;
        private const int MIN_EYE_OPEN_TIME = 60;
        private const int MAX_EYE_CLOSED_TIME = 240;
        private const int MIN_EYE_CLOSED_TIME = 60;
        private const int MOVEMENT_REGION_WIDTH = Area.TILE_WIDTH * 4;
        private const int MOVEMENT_REGION_HEIGHT = Area.TILE_HEIGHT * 5;
        private const int MOVE_WAIT_TIME = 120;
        private const int MIN_FIRE_BULLETS_WAIT_TIME = 180;
        private const int MAX_FIRE_BULLETS_WAIT_TIME = 480;
        private const int DAMAGE = 1;
        private const int KNOCKBACK_TIME = 10;
        private const int HURT_TIME = 30;
        private const float KNOCKBACK_SPEED = 7.0f;
        private const int STUNNED_POSITION_OFFSET_Y = 96;
        private const int STUNNED_POSITION_OFFSET_X = 32;
        private const int STUNNED_TIME = 300;

        private Sprite headSprite;
        private Sprite eyeOpenSprite;
        private Sprite eyeOpeningSprite;
        private Sprite eyeClosedSprite;
        private Sprite eyeClosingSprite;
        private Sprite eyeStunnedSprite;
        private Sprite mouthSprite;

        private Sprite currentEyeSprite;

        private SoundEffect hitSound;

        private Rectangle movementRegion;
        private Vector2 destinationPosition = Vector2.Zero;
        private Boss boss;
        private Vector2 stunnedPosition = Vector2.Zero;
        private bool reachedStunnedPosition = false;
        private Vector2 mouthPosition = Vector2.Zero;

        public EyeState EyeState;
        private int eyeTimer = 0;
        private int eyeOpenTime = 0;
        private int eyeClosedTime = 0;
        private int eyeHitBoxWidth = 0;
        private int eyeHitBoxHeight = 0;
        private int moveWaitTimer = 0;
        private int stunnedTimer = 0;
        private int fireBulletsWaitTime = 0;
        private int fireBulletsWaitTimer = 0;
        private bool isStunned = false;
        public bool IsStunned { get { return isStunned; } }

        public Rectangle EyeHitBox
        {
            get
            {
                return new Rectangle((int)Math.Round(Origin.X + EYE_OFFSET_X), (int)Math.Round(Origin.Y + EYE_OFFSET_Y),
                  eyeHitBoxWidth, eyeHitBoxHeight);
            }
        }

        public BossHead(GameWorld game, Area area, Vector2 centerPosition, Boss boss)
            : base(game, area)
        {
            hitBoxOffset = new Vector2(0, 0);
            hitBoxWidth = 100;
            hitBoxHeight = 95;

            Vector2 origin = new Vector2(14, 16);
            headSprite = new Sprite(origin);

            eyeHitBoxWidth = 44;
            eyeHitBoxHeight = 20;
            origin = new Vector2(0, 0);
            eyeOpenSprite = new Sprite(origin);
            eyeOpeningSprite = new Sprite(origin, 5, EYE_OPEN_ANIMATION_DELAY, 1);
            eyeClosedSprite = new Sprite(origin);
            eyeClosingSprite = new Sprite(origin, 5, EYE_CLOSE_ANIMATION_DELAY, 1);
            eyeStunnedSprite = new Sprite(origin);

            origin = new Vector2(0, 0);
            mouthSprite = new Sprite(origin);

            CurrentSprite = headSprite;
            currentEyeSprite = eyeOpenSprite;

            Center = centerPosition;
            this.boss = boss;
            IsAffectedByWallCollisions = false;
            mouthPosition.X = Origin.X + MOUTH_OFFSET_X;
            mouthPosition.Y = Origin.Y + MOUTH_OFFSET_Y;
            EyeState = EyeState.Open;
            MaxHealth = 1;
            Health = 1;
            Damage = DAMAGE;
            movementRegion = new Rectangle((int)Center.X - (MOVEMENT_REGION_WIDTH / 2), (int)Center.Y - 64,
                MOVEMENT_REGION_WIDTH, MOVEMENT_REGION_HEIGHT);
            knockBackTime = KNOCKBACK_TIME;
            knockBackSpeed = KNOCKBACK_SPEED;
            hurtTime = HURT_TIME;
            stunnedPosition = new Vector2(Center.X, Center.Y - STUNNED_POSITION_OFFSET_Y);
            if (boss.Origin.X > this.Origin.X)
                stunnedPosition.X -= STUNNED_POSITION_OFFSET_X;
            else
                stunnedPosition.X += STUNNED_POSITION_OFFSET_X;
            fireBulletsWaitTime = GameWorld.Random.Next(MIN_FIRE_BULLETS_WAIT_TIME, MAX_FIRE_BULLETS_WAIT_TIME);
            shouldBeDrawnByArea = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            headSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_head");
            eyeOpenSprite.Texture= game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_open");
            eyeOpeningSprite.Texture= game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_opening");
            eyeClosedSprite.Texture= game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_closed");
            eyeClosingSprite.Texture= game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_closing");
            eyeStunnedSprite.Texture= game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_stunned");
            mouthSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_mouth_open");
            hitSound = game.Content.Load<SoundEffect>("Audio/boss_hit");
        }

        public override void Update()
        {
            currentEyeSprite.UpdateAnimation();

            base.Update();

            mouthPosition.X = Origin.X + MOUTH_OFFSET_X;
            mouthPosition.Y = Origin.Y + MOUTH_OFFSET_Y;
        }

        protected override void updateAI()
        {
            if (!isStunned && EyeState == EyeState.Stunned)
            {
                isStunned = true;
                stunnedTimer = 0;
                reachedStunnedPosition = false;
            }

            if (isStunned)
            {
                if (boss.EyeState == EyeState.Closed && !boss.IsStunned)
                {
                    stunnedTimer++;

                    if (stunnedTimer >= STUNNED_TIME)
                    {
                        Unstun();
                    }
                }

                if (!reachedStunnedPosition)
                {
                    destinationPosition = stunnedPosition;
                    moveTowardsDestinationPosition();
                }
            }
            else if (!isStunned)
            {
                if (EyeState == EyeState.Open || EyeState == EyeState.Closed)
                {
                    eyeTimer++;
                    if (EyeState == EyeState.Open && eyeTimer >= eyeOpenTime)
                        closeEye();
                    else if (EyeState == EyeState.Closed && eyeTimer >= eyeClosedTime)
                        openEye();
                }
                else if (EyeState == EyeState.Opening && currentEyeSprite.IsDoneAnimating)
                {
                    EyeState = EyeState.Open;
                    currentEyeSprite = eyeOpenSprite;
                }
                else if (EyeState == EyeState.Closing && currentEyeSprite.IsDoneAnimating)
                {
                    EyeState = EyeState.Closed;
                    currentEyeSprite = eyeClosedSprite;
                }

                if (Velocity.X == 0 && Velocity.Y == 0 && EyeState != EyeState.Stunned)
                {
                    moveWaitTimer++;
                    if (moveWaitTimer >= MOVE_WAIT_TIME)
                    {
                        moveWaitTimer = 0;
                        destinationPosition.X = GameWorld.Random.Next(movementRegion.X, movementRegion.X + movementRegion.Width);
                        destinationPosition.Y = GameWorld.Random.Next(movementRegion.Y, movementRegion.Y + movementRegion.Height);

                        moveTowardsDestinationPosition();
                    }
                }
                else if (EyeState != EyeState.Stunned)
                {
                    moveTowardsDestinationPosition();
                }

                fireBulletsWaitTimer++;
                if (fireBulletsWaitTimer >= fireBulletsWaitTime)
                {
                    fireBulletsWaitTimer = 0;
                    fireBulletsWaitTime = GameWorld.Random.Next(MIN_FIRE_BULLETS_WAIT_TIME, MAX_FIRE_BULLETS_WAIT_TIME);
                    fireBullets();
                }
            }
        }

        private void fireBullets()
        {
            Vector2 firePosition = new Vector2(mouthPosition.X + (mouthSprite.Texture.Width / 2),
                mouthPosition.Y + (mouthSprite.Texture.Height / 2) + 10);
            float angle = MathHelper.Pi / 3.0f;
            EnemyProjectile bullet;

            for (int i = 0; i < 3; i++)
            {
                bullet = new EnemyProjectile(game, area, EnemyProjectileType.Bullet);
                bullet.LoadContent();
                bullet.Center = firePosition;
                bullet.Fire(angle);
                area.Entities.Add(bullet);
                angle += MathHelper.Pi / 6.0f;
            }
        }

        public void Unstun()
        {
            closeEye();
            isStunned = false;
            stunnedTimer = 0;
        }

        private void moveTowardsDestinationPosition()
        {
            Vector2 direction = destinationPosition - Center;

            if (direction.Length() <= MOVE_SPEED)
            {
                Center = new Vector2(destinationPosition.X, destinationPosition.Y);
                Velocity = Vector2.Zero;

                if (isStunned && destinationPosition == stunnedPosition)
                {
                    reachedStunnedPosition = true;
                }

                return;
            }

            double angle = Math.Atan2(direction.Y, direction.X);
            Velocity.X = (float)Math.Cos(angle) * MOVE_SPEED;
            Velocity.Y = (float)Math.Sin(angle) * MOVE_SPEED;
        }

        private void openEye()
        {
            EyeState = EyeState.Opening;
            eyeTimer = 0;
            eyeOpenTime = GameWorld.Random.Next(MIN_EYE_OPEN_TIME, MAX_EYE_OPEN_TIME);

            currentEyeSprite = eyeOpeningSprite;
            currentEyeSprite.ResetAnimation();
        }

        private void closeEye()
        {
            EyeState = EyeState.Closing;
            eyeTimer = 0;
            eyeClosedTime = GameWorld.Random.Next(MIN_EYE_CLOSED_TIME, MAX_EYE_CLOSED_TIME);

            currentEyeSprite = eyeClosingSprite;
            currentEyeSprite.ResetAnimation();
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Arrow)
            {
                Arrow arrow = (Arrow)other;
                if (arrow.IsFired && this.Contains(arrow.TipPosition))
                {
                    if (arrow.FaceDirection == Directions.Up)
                    {
                        if ((EyeState == EyeState.Open || EyeState == EyeState.Opening || EyeState == EyeState.Closing) && 
                            RectangleContains(EyeHitBox, arrow.TipPosition))
                        {
                            takeDamage();
                            arrow.HitEntity(this);
                        }
                        else if (arrow.TipPosition.Y < EyeHitBox.Bottom - 1)
                        {
                            arrow.HitEntity(this);
                        }
                    }
                    else
                    {
                        arrow.HitEntity(this);
                    }
                }
            }
        }

        private void takeDamage()
        {
            //isHurt = true;
            state = EnemyState.Hurt;
            Velocity.Y = -knockBackSpeed;
            EyeState = EyeState.Stunned;
            currentEyeSprite = eyeStunnedSprite;

            hitSound.Play(0.75f, 0, 0);
        }

        public override void StartDying()
        {
            Velocity = Vector2.Zero;
            EyeState = EyeState.Stunned;
            currentEyeSprite = eyeStunnedSprite;
            state = EnemyState.Dying;
        }

        public override void ReactToSwordHit(Player player)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            //if (!IsDying)
            if (state != EnemyState.Dying)
            {
                bool increaseRed = false;
                bool increaseBlue = false;
                //if (IsHurt)
                if (state == EnemyState.Hurt)
                {
                    int n = hurtTimer % (BLINK_DELAY * 3);
                    if (n < BLINK_DELAY)
                        increaseRed = true;
                    else if (n < (BLINK_DELAY * 2))
                        increaseBlue = true;
                }

                changeColorsEffect.Parameters["red"].SetValue(increaseRed);
                changeColorsEffect.Parameters["blue"].SetValue(increaseBlue);

                CurrentSprite.Draw(spriteBatch, Origin);
                currentEyeSprite.Draw(spriteBatch, new Vector2(EyeHitBox.X, EyeHitBox.Y));
                mouthSprite.Draw(spriteBatch, mouthPosition);

                changeColorsEffect.Parameters["red"].SetValue(false);
                changeColorsEffect.Parameters["blue"].SetValue(false);
            }
            //else if (IsDying)
            else if (state == EnemyState.Dying)
            {
                CurrentSprite.Draw(spriteBatch, Origin);
                currentEyeSprite.Draw(spriteBatch, new Vector2(EyeHitBox.X, EyeHitBox.Y));
                mouthSprite.Draw(spriteBatch, mouthPosition);
            }

            //spriteBatch.Draw(game.SquareTexture, movementRegion, Color.Yellow);
            
        }
    }
}

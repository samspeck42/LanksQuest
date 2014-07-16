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
    public class Boss : Enemy
    {
        private const int HEAD_OFFSET_X = 160;
        private const int HEAD_OFFSET_Y = 96;
        private const int EYE_STUNNED_ANIMATION_DELAY = 2;
        private const int EYE_OPEN_ANIMATION_DELAY = 8;
        private const int EYE_CLOSE_ANIMATION_DELAY = 8;
        private const int EYE_OFFSET_X = 22;
        private const int EYE_OFFSET_Y = 56;
        private const int EYE_OPEN_TIME = 180;
        private const int STUNNED_TIME = 200;
        private const int MAX_HEALTH = 38;
        private const int HURT_TIME = 60;
        private const int MOUTH_MOVE_DISTANCE = 32;
        private const int MOUTH_OFFSET_Y = 74;
        private const float MOUTH_MOVE_SPEED = 0.4f;
        private const int MOUTH_OPEN_TIME = 30;
        private const int MOUTH_CLOSED_TIME = 240;
        private const int NUM_NECK_SEGMENTS = 10;
        private const int NECK_OFFSET_X = 50;
        private const int NECK_OFFSET_Y = 10;
        private const int DYING_TIME = 180;
        private const int DAMAGE = 1;
        private const int SWORD_DAMAGE_INCREASE = 2;

        private AnimatedSprite bodySprite;
        private AnimatedSprite eyeOpenSprite;
        private AnimatedSprite eyeOpeningSprite;
        private AnimatedSprite eyeClosedSprite;
        private AnimatedSprite eyeClosingSprite;
        private AnimatedSprite eyeStunnedSprite;
        private AnimatedSprite mouthSprite;
        private AnimatedSprite neckSegmentSprite;

        private AnimatedSprite currentEyeSprite;

        public EyeState EyeState;

        private BossHead[] heads  = new BossHead[2];
        private int eyeOpenTimer = 0;
        private int stunnedTimer = 0;
        private int dyingTimer = 0;
        private Vector2 mouthPosition = Vector2.Zero;
        private Vector2 mouthVelocity = Vector2.Zero;
        private float minMouthYPosition = 0;
        private float maxMouthYPosition = 0;
        private int mouthTimer = 0;
        private bool isMouthClosed = true;
        private bool isMouthOpen = false;
        private bool isStunned = false;
        public bool IsStunned { get { return isStunned; } }
        private Vector2[] neckSegmentPositions = new Vector2[NUM_NECK_SEGMENTS];

        public Rectangle EyeHitBox
        {
            get
            {
                return new Rectangle((int)Math.Round(Position.X + EYE_OFFSET_X), (int)Math.Round(Position.Y + EYE_OFFSET_Y),
                  currentEyeSprite.Bounds.Width, currentEyeSprite.Bounds.Height);
            }
        }

        public Boss(GameWorld game, Area area)
            : base(game, area)
        {
            Rectangle bounds = new Rectangle(16, 16, 132, 111);
            bodySprite = new AnimatedSprite(bounds);
            bounds = new Rectangle(0, 0, 88, 60);
            eyeOpenSprite = new AnimatedSprite(bounds);
            eyeOpeningSprite = new AnimatedSprite(bounds, 5, EYE_OPEN_ANIMATION_DELAY, 1);
            eyeClosedSprite = new AnimatedSprite(bounds);
            eyeClosingSprite = new AnimatedSprite(bounds, 5, EYE_CLOSE_ANIMATION_DELAY, 1);
            eyeStunnedSprite = new AnimatedSprite(bounds, 8, EYE_STUNNED_ANIMATION_DELAY);
            bounds = new Rectangle(0, 0, 120, 50);
            mouthSprite = new AnimatedSprite(bounds);
            bounds = new Rectangle(0, 0, 65, 65);
            neckSegmentSprite = new AnimatedSprite(bounds);

            CurrentSprite = bodySprite;
            currentEyeSprite = eyeClosedSprite;
            IsAffectedByWallCollisions = false;
            EyeState = EyeState.Closed;
            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;
            hurtTime = HURT_TIME;
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            initHeads();

            mouthPosition = new Vector2(Center.X - (mouthSprite.Bounds.Width / 2), Position.Y + MOUTH_OFFSET_Y);
            minMouthYPosition = mouthPosition.Y;
            maxMouthYPosition = mouthPosition.Y + MOUTH_MOVE_DISTANCE;
        }

        private void initHeads()
        {
            heads = new BossHead[2];
            heads[0] = new BossHead(game, area, new Vector2(this.Center.X - HEAD_OFFSET_X, this.Center.Y + HEAD_OFFSET_Y), this);
            heads[1] = new BossHead(game, area, new Vector2(this.Center.X + HEAD_OFFSET_X, this.Center.Y + HEAD_OFFSET_Y), this);

            foreach (BossHead head in heads)
                area.Entities.Add(head);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            bodySprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_head_big");
            eyeOpenSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_open_big");
            eyeOpeningSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_opening_big");
            eyeClosedSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_closed_big");
            eyeClosingSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_closing_big");
            eyeStunnedSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_eye_stunned_big");
            mouthSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_mouth_big");
            neckSegmentSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/Boss/boss_neck_segment");

            foreach (BossHead head in heads)
                head.LoadContent();
        }

        public override void Update()
        {
            currentEyeSprite.UpdateAnimation();

            base.Update();

            //if (!IsDying)
            if (state != EnemyState.Dying)
            {
                if (isStunned)
                {
                    stunnedTimer++;

                    if (stunnedTimer >= STUNNED_TIME)
                    {
                        closeEye();
                        isStunned = false;
                        stunnedTimer = 0;
                        foreach (BossHead head in heads)
                            head.Unstun();
                    }
                }
            }
            
            //if (IsDying)
            if (state == EnemyState.Dying)
            {
                dyingTimer++;

                if (dyingTimer >= DYING_TIME)
                {
                    Die();

                    foreach (BossHead head in heads)
                        head.Die();

                    ((Dungeon)area.Map).OnBossDeath();
                }
            }
        }

        protected override void updateAI()
        {
            mouthPosition += mouthVelocity;
            if (mouthPosition.Y > maxMouthYPosition)
            {
                // mouth open
                mouthPosition.Y = maxMouthYPosition;
                mouthVelocity = Vector2.Zero;
                isMouthClosed = false;
                isMouthOpen = true;

                fireFireball();
            }
            if (mouthPosition.Y < minMouthYPosition)
            {
                // mouth closed
                mouthPosition.Y = minMouthYPosition;
                mouthVelocity = Vector2.Zero;
                isMouthOpen = false;
                isMouthClosed = true;
            }

            if (!isStunned && EyeState == EyeState.Stunned)
            {
                isStunned = true;
                stunnedTimer = 0;
                closeMouth();
            }
            if (!isStunned)
            {
                if (EyeState == EyeState.Opening && currentEyeSprite.IsDoneAnimating)
                {
                    EyeState = EyeState.Open;
                    currentEyeSprite = eyeOpenSprite;
                }
                else if (EyeState == EyeState.Closing && currentEyeSprite.IsDoneAnimating)
                {
                    EyeState = EyeState.Closed;
                    currentEyeSprite = eyeClosedSprite;
                }
                else if (EyeState == EyeState.Closed && heads[0].IsStunned && heads[1].IsStunned)
                {
                    openEye();
                }
                else if (EyeState == EyeState.Open)
                {
                    eyeOpenTimer++;
                    if (eyeOpenTimer >= EYE_OPEN_TIME)
                    {
                        closeEye();
                        foreach (BossHead head in heads)
                            head.Unstun();
                    }
                }

                if (isMouthOpen || isMouthClosed)
                {
                    mouthTimer++;
                    if (isMouthOpen && mouthTimer >= MOUTH_OPEN_TIME)
                    {
                        closeMouth();
                        mouthTimer = 0;
                    }
                    else if (isMouthClosed && mouthTimer >= MOUTH_CLOSED_TIME)
                    {
                        openMouth();
                        mouthTimer = 0;
                    }
                }
            }
        }

        private void fireFireball()
        {
            Vector2 firePosition = new Vector2(mouthPosition.X + (mouthSprite.Bounds.Width / 2) - 16,
                mouthPosition.Y + (mouthSprite.Bounds.Height / 2));
            EnemyProjectile fireball = new EnemyProjectile(game, area, EnemyProjectileType.Fireball);
            fireball.LoadContent();
            fireball.Center = firePosition;
            fireball.Fire(MathHelper.PiOver2);
            area.Entities.Add(fireball);

            firePosition = new Vector2(firePosition.X + 32, firePosition.Y);
            fireball = new EnemyProjectile(game, area, EnemyProjectileType.Fireball);
            fireball.LoadContent();
            fireball.Center = firePosition;
            fireball.Fire(MathHelper.PiOver2);
            area.Entities.Add(fireball);
        }

        private void closeMouth()
        {
            mouthVelocity.Y = -MOUTH_MOVE_SPEED;
            isMouthClosed = false;
            isMouthOpen = false;
        }

        private void openMouth()
        {
            mouthVelocity.Y = MOUTH_MOVE_SPEED;
            isMouthClosed = false;
            isMouthOpen = false;
        }

        private void closeEye()
        {
            EyeState = EyeState.Closing;

            currentEyeSprite = eyeClosingSprite;
            currentEyeSprite.ResetAnimation();
        }

        private void openEye()
        {
            EyeState = EyeState.Opening;
            eyeOpenTimer = 0;

            currentEyeSprite = eyeOpeningSprite;
            currentEyeSprite.ResetAnimation();
        }

        public override void OnEntityCollision(Entity other)
        {
            //if (!IsDying)
            if (state != EnemyState.Dying)
            {
                if (other is Arrow)
                {
                    Arrow arrow = (Arrow)other;
                    if (arrow.IsFired && this.Contains(arrow.TipPosition))
                    {
                        if (arrow.FaceDirection == Directions.Up)
                        {
                            //if (!IsHurt &&
                            if (state == EnemyState.Normal &&
                                (EyeState == EyeState.Open || EyeState == EyeState.Stunned) &&
                                RectangleContains(EyeHitBox, arrow.TipPosition))
                            {
                                takeDamageFrom(arrow);
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
        }

        protected override void takeDamageFrom(Entity entity)
        {
            //isHurt = true;
            state = EnemyState.Hurt;

            if (entity is Arrow)
            {
                if (EyeState == EyeState.Open)
                {
                    EyeState = EyeState.Stunned;
                    currentEyeSprite = eyeStunnedSprite;
                }
                Health -= entity.Damage;
            }
            if (entity is Player)
            {
                Health -= entity.Damage + SWORD_DAMAGE_INCREASE;
            }

            hitSound.Play(0.75f, 0, 0);
        }

        public override void ReactToSwordHit(Player player)
        {
            if (isStunned && state == EnemyState.Normal)
            {
                if (EyeHitBox.Contains(player.SwordHitBox) || EyeHitBox.Intersects(player.SwordHitBox))
                {
                    takeDamageFrom(player);
                }
            }
        }

        public override void StartDying()
        {
            //isDying = true;
            //isHurt = false;
            state = EnemyState.Dying;

            foreach (BossHead head in heads)
                head.StartDying();
        }

        private void updateNeckSegmentPositions()
        {
            Vector2 neckStartPosition = new Vector2(Center.X - NECK_OFFSET_X, Center.Y + NECK_OFFSET_Y);
            Vector2 neckVector = heads[0].Center - neckStartPosition;
            float neckAngle = (float)Math.Atan2(neckVector.Y, neckVector.X);
            float neckLength = neckVector.Length();
            float neckSegmentDistance = neckLength / (NUM_NECK_SEGMENTS / 2);
            float distance = 0f;
            for (int i = 0; i < NUM_NECK_SEGMENTS / 2; i++)
            {
                neckSegmentPositions[i] = new Vector2(neckStartPosition.X + ((float)Math.Cos(neckAngle) * distance),
                    neckStartPosition.Y + ((float)Math.Sin(neckAngle) * distance));
                distance += neckSegmentDistance;
            }

            neckStartPosition = new Vector2(Center.X + NECK_OFFSET_X, Center.Y + NECK_OFFSET_Y);
            neckVector = heads[1].Center - neckStartPosition;
            neckAngle = (float)Math.Atan2(neckVector.Y, neckVector.X);
            neckLength = neckVector.Length();
            neckSegmentDistance = neckLength / (NUM_NECK_SEGMENTS / 2);
            distance = 0f;
            for (int i = NUM_NECK_SEGMENTS / 2; i < NUM_NECK_SEGMENTS; i++)
            {
                neckSegmentPositions[i] = new Vector2(neckStartPosition.X + ((float)Math.Cos(neckAngle) * distance),
                    neckStartPosition.Y + ((float)Math.Sin(neckAngle) * distance));
                distance += neckSegmentDistance;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            //if (!IsDying)
            if (state != EnemyState.Dying)
            {
                drawNecks(spriteBatch);

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

                drawBody(spriteBatch);

                changeColorsEffect.Parameters["red"].SetValue(false);
                changeColorsEffect.Parameters["blue"].SetValue(false);

                drawHeads(spriteBatch, changeColorsEffect);
            }
            //else if (IsDying)
            else if (state == EnemyState.Dying)
            {
                bool increaseRed = false;
                bool increaseBlue = false;
                int n = dyingTimer % (BLINK_DELAY * 3);
                if (n < BLINK_DELAY)
                    increaseRed = true;
                else if (n < (BLINK_DELAY * 2))
                    increaseBlue = true;

                changeColorsEffect.Parameters["red"].SetValue(increaseRed);
                changeColorsEffect.Parameters["blue"].SetValue(increaseBlue);
                drawNecks(spriteBatch);
                drawBody(spriteBatch);
                drawHeads(spriteBatch, changeColorsEffect);
                changeColorsEffect.Parameters["red"].SetValue(false);
                changeColorsEffect.Parameters["blue"].SetValue(false);
            }

            //spriteBatch.DrawString(game.Font, "Health: " + Health, new Vector2(Center.X - 50, Center.Y - 70), Color.White);
            //spriteBatch.DrawString(game.Font, "isStunned: " + isStunned.ToString(), new Vector2(Center.X - 50, Center.Y - 50), Color.White);
            //spriteBatch.DrawString(game.Font, "EyeState: " + EyeState.ToString(), new Vector2(Center.X - 50, Center.Y - 30), Color.White);
        }

        private void drawHeads(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            foreach (BossHead head in heads)
                head.Draw(spriteBatch, changeColorsEffect);
        }

        private void drawBody(SpriteBatch spriteBatch)
        {
            mouthSprite.Draw(spriteBatch, mouthPosition);
            CurrentSprite.Draw(spriteBatch, Position);
            currentEyeSprite.Draw(spriteBatch, new Vector2(EyeHitBox.X, EyeHitBox.Y));
        }

        private void drawNecks(SpriteBatch spriteBatch)
        {
            updateNeckSegmentPositions();
            foreach (Vector2 pos in neckSegmentPositions)
            {
                neckSegmentSprite.Draw(spriteBatch, new Vector2(pos.X - (neckSegmentSprite.Bounds.Width / 2),
                    pos.Y - (neckSegmentSprite.Bounds.Height / 2)));
            }
        }
    }

    public enum EyeState { Open, Opening, Closed, Closing, Stunned }
}

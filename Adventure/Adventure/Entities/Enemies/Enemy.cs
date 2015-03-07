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
    public abstract class Enemy : Mob, PickupDropper
    {
        protected const int BLINK_DELAY = 130;
        protected const int HURT_TIME = 500;

        public EnemyState EnemyState { get { return enemyState; } }
        public float PickupDropChance { get { return 0.75f; } }
        public Vector2 PickupDropPosition { get { return this.Center; } }

        protected EnemyState enemyState = EnemyState.Normal;
        protected MovementHandler movementHandler = null;
        private MovementHandler oldMovementHandler;
        private int hurtTimer = 0;

        protected virtual float knockBackSpeed { get { return 400; } }
        protected virtual float knockBackDistance { get { return 48; } }

        protected SoundEffect hitSound;

        public Enemy(GameWorld game, Area area)
            : base(game, area) 
        {
            setHealth(MaxHealth);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            hitSound = game.Content.Load<SoundEffect>("Audio/enemy_hit");
        }

        public virtual bool TakesDamageFromPlayerSword(HitBox thisHitBox)
        {
            return false;
        }

        public virtual bool TakesDamageFromPot(HitBox thisHitBox)
        {
            return false;
        }

        public virtual bool TakesDamageFromArrow(HitBox thisHitBox)
        {
            return false;
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            if (dataDict.ContainsKey("isActive"))
            {
                isVisible = bool.Parse(dataDict["isActive"]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (movementHandler != null)
                movementHandler.Update(gameTime);

            if (enemyState == EnemyState.Normal)
            {
                updateAI(gameTime);
            }
            else if (enemyState == EnemyState.Hurt)
            {
                hurtTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (hurtTimer >= HURT_TIME)
                {
                    // recover from being hurt
                    enemyState = EnemyState.Normal;
                    hurtTimer = 0;

                    if (Health <= 0)
                    {
                        StartDying();
                    }
                    else
                    {
                        // restore old movement handler enemy was using before it was hurt
                        movementHandler = oldMovementHandler;

                        onHurtRecovery();
                    }
                }
            }
            else if (enemyState == EnemyState.Dying)
            {
                Die();
            }

            spriteHandler.Update(gameTime);
        }

        protected abstract void updateAI(GameTime gameTime);

        protected virtual void onHurtRecovery() { }

        public virtual void StartDying()
        {
            movementHandler = null;
            enemyState = EnemyState.Dying;
        }

        public override void Die()
        {
            tryToTriggerActivations();

            base.Die();

            area.SpawnPickup(this);
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            bool increaseRed = false;
            bool increaseBlue = false;
            if (enemyState == EnemyState.Hurt)
            {
                int n = hurtTimer % (BLINK_DELAY * 3);
                if (n < BLINK_DELAY)
                    increaseRed = true;
                else if (n < (BLINK_DELAY * 2))
                    increaseBlue = true;
            }
            if (increaseRed)
                changeColorsEffect.Parameters["red"].SetValue(0.6f);
            if (increaseBlue)
                changeColorsEffect.Parameters["blue"].SetValue(0.6f);
            base.Draw(spriteBatch, changeColorsEffect);
            changeColorsEffect.Parameters["red"].SetValue(0f);
            changeColorsEffect.Parameters["blue"].SetValue(0f);
        }

        public override void TakeDamage(Entity other, KnockBackType knockBackType)
        {
            if (enemyState == EnemyState.Hurt || enemyState == EnemyState.Dying)
                return;

            base.TakeDamage(other, knockBackType);

            enemyState = EnemyState.Hurt;

            float angle = 0f;
            float distance = 0f;
            if (knockBackType == KnockBackType.HitAngle || knockBackType == KnockBackType.FaceDirection)
            {
                Vector2 direction = Vector2.Zero;

                if (knockBackType == KnockBackType.HitAngle)
                    direction = this.Center - other.Center;
                else if (knockBackType == KnockBackType.FaceDirection)
                    direction = DirectionsHelper.GetDirectionVector(other.FaceDirection);

                angle = (float)Math.Atan2(direction.Y, direction.X);
                distance = knockBackDistance;
            }

            oldMovementHandler = movementHandler;
            movementHandler = new StraightMovementHandler(this, knockBackSpeed, angle, distance);
            movementHandler.Start();

            hitSound.Play(0.9f, 0, 0);
        }
    }

    public enum EnemyState
    {
        Normal,
        Hurt,
        Dying
    }
}

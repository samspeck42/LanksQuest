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
    public abstract class Enemy : ActivatingEntity
    {
        protected const int BLINK_DELAY = 8;
        protected const int DEFAULT_KNOCKBACK_TIME = 10;
        protected const int DEFAULT_HURT_TIME = 30;
        protected const float DEFAULT_KNOCKBACK_SPEED = 10.0f;

        //protected bool isHurt = false;
        //public bool IsHurt { get { return isHurt; } }
        //protected bool isDying = false;
        //public bool IsDying { get { return isDying; } }

        protected EnemyState state = EnemyState.Normal;
        public EnemyState State { get { return state; } }
        protected bool doesDamageOnContact = true;
        public bool DoesDamageOnContact { get { return doesDamageOnContact; } }

        protected int hurtTimer = 0;
        protected int knockBackTime = DEFAULT_KNOCKBACK_TIME;
        protected int hurtTime = DEFAULT_HURT_TIME;
        protected float knockBackSpeed = DEFAULT_KNOCKBACK_SPEED;

        protected SoundEffect hitSound;

        public Enemy(GameWorld game, Area area)
            : base(game, area)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            hitSound = game.Content.Load<SoundEffect>("Audio/enemy_hit");
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("isActive"))
            {
                isActive = bool.Parse(dataDict["isActive"]);
            }
        }

        public override void Update()
        {
            base.Update();

            //if (IsHurt && !IsDying)
            if(state == EnemyState.Hurt)
            {
                hurtTimer++;
                if (hurtTimer == knockBackTime)
                {
                    Velocity = Vector2.Zero;
                }
                if (hurtTimer >= hurtTime)
                {
                    //isHurt = false;
                    state = EnemyState.Normal;
                    hurtTimer = 0;

                    if (Health <= 0)
                    {
                        StartDying();
                    }
                }
            }
            //if (!IsHurt && !IsDying)
            if (state == EnemyState.Normal)
            {
                updateAI();
            }
        }

        protected abstract void updateAI();

        public virtual void StartDying()
        {
            //isDying = true;
            //isHurt = false;
            state = EnemyState.Dying;
            Die();
        }

        public virtual void Die()
        {
            IsAlive = false;
            tryToTriggerActivations();
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
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
            base.Draw(spriteBatch, changeColorsEffect);
            changeColorsEffect.Parameters["red"].SetValue(false);
            changeColorsEffect.Parameters["blue"].SetValue(false);
        }

        protected virtual void takeDamageFrom(Entity entity)
        {
            //isHurt = true;
            state = EnemyState.Hurt;
            Health -= entity.Damage;

            Vector2 direction = this.Center - entity.Center;
            float angle = 0f;
            if (entity is Arrow)
            {
                if (entity.FaceDirection == Directions.Up)
                    angle = 3f * MathHelper.PiOver2;
                else if (entity.FaceDirection == Directions.Down)
                    angle = MathHelper.PiOver2;
                else if (entity.FaceDirection == Directions.Left)
                    angle = MathHelper.Pi;
                else if (entity.FaceDirection == Directions.Right)
                    angle = 0f;
            }
            else
            {
                angle = (float)Math.Atan2(direction.Y, direction.X);
            }
            Velocity.X = (float)Math.Cos(angle) * knockBackSpeed;
            Velocity.Y = (float)Math.Sin(angle) * knockBackSpeed;

            hitSound.Play(0.75f, 0, 0);
        }

        public abstract void ReactToSwordHit(Player player);
        
    }

    public enum EnemyState
    {
        Normal,
        Hurt,
        Dying
    }
}

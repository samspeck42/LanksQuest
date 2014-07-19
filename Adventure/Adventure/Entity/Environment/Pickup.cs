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
    public class Pickup : Entity
    {
        private const int ANIMATION_DELAY = 4;
        private const int BRONZE_COIN_VALUE = 1;
        private const int SILVER_COIN_VALUE = 5;
        private const int GOLD_COIN_VALUE = 10;
        private const int HEART_VALUE = 2;
        private const int KEY_VALUE = 1;

        private const int LIFETIME = 300;
        private const int BLINK_START_TIME = 240;
        private const int BLINK_DELAY = 2;

        private Sprite bronzeCoinSprite;
        private Sprite silverCoinSprite;
        private Sprite goldCoinSprite;
        private Sprite heartSprite;
        private Sprite keySprite;

        private SoundEffect coinCollectSound;
        private SoundEffect heartCollectSound;

        private Texture2D shadowTexture;
        private float bounceHeight;
        private float bounceGravity;
        private float bounceVelocity;
        private bool isBouncing = false;
        private bool isDropped;
        private int lifeTimer = 0;

        public PickupType Type;
        public int Value;

        public Pickup(GameWorld game, Area area)
            : base(game, area)
        {
            init(PickupType.BronzeCoin, false);
        }

        public Pickup(GameWorld game, Area area, PickupType type, bool isDropped)
            : base(game, area)
        {
            init(type, isDropped);
        }

        private void init(PickupType type, bool isDropped)
        {
            this.isDropped = isDropped;
            IsAffectedByWallCollisions = false;

            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 14;
            hitBoxHeight = 14;

            Vector2 origin = new Vector2(0, 0);
            bronzeCoinSprite = new Sprite(origin, 6, ANIMATION_DELAY);

            silverCoinSprite = new Sprite(origin, 6, ANIMATION_DELAY);

            goldCoinSprite = new Sprite(origin, 6, ANIMATION_DELAY);

            heartSprite = new Sprite(origin);

            keySprite = new Sprite(origin);

            setType(type);

            if (isDropped)
            {
                startBounce();
            }
        }


        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            setType((PickupType)int.Parse(dataDict["type"]));
        }

        public override string ToString()
        {
            return "(" + base.ToString() + ")(" + ((int)Type).ToString() + ")";
        }

        private void setType(PickupType pickupType)
        {
            this.Type = pickupType;

            if (Type == PickupType.BronzeCoin)
            {
                CurrentSprite = bronzeCoinSprite;
                Value = BRONZE_COIN_VALUE;
            }
            else if (Type == PickupType.SilverCoin)
            {
                CurrentSprite = silverCoinSprite;
                Value = SILVER_COIN_VALUE;
            }
            else if (Type == PickupType.GoldCoin)
            {
                CurrentSprite = goldCoinSprite;
                Value = GOLD_COIN_VALUE;
            }
            else if (Type == PickupType.Heart)
            {
                CurrentSprite = heartSprite;
                Value = HEART_VALUE;
            }
            else if (Type == PickupType.Key)
            {
                CurrentSprite = keySprite;
                Value = KEY_VALUE;
            }
        }

        public override void LoadContent()
        {
            bronzeCoinSprite.Texture = game.Content.Load<Texture2D>("Sprites/Pickups/coin_bronze");
            silverCoinSprite.Texture = game.Content.Load<Texture2D>("Sprites/Pickups/coin_silver");
            goldCoinSprite.Texture = game.Content.Load<Texture2D>("Sprites/Pickups/coin_gold");
            heartSprite.Texture = game.Content.Load<Texture2D>("Sprites/Pickups/heart");
            keySprite.Texture = game.Content.Load<Texture2D>("Sprites/Pickups/key");
            shadowTexture = game.Content.Load<Texture2D>("Sprites/Pickups/shadow");
            coinCollectSound = game.Content.Load<SoundEffect>("Audio/coin_collect");
            heartCollectSound = game.Content.Load<SoundEffect>("Audio/heart_collect");
        }

        private void startBounce()
        {
            bounceHeight = 20;
            bounceGravity = 0.5f;
            bounceVelocity = 1;
            isBouncing = true;
        }

        public override void Update()
        {
            base.Update();

            if (isBouncing)
            {
                doBounce();
            }

            if (isDropped)
            {
                lifeTimer++;
                if (lifeTimer >= LIFETIME)
                {
                    isAlive = false;
                }
            }
        }

        private void doBounce()
        {
            bounceVelocity -= bounceGravity;
            bounceHeight += bounceVelocity;

            if (bounceHeight <= 0)
            {
                bounceHeight = 0;

                if (Math.Abs(bounceVelocity) <= 0.1)
                {
                    endBounce();
                }
                else
                {
                    bounceVelocity *= -0.75f;
                }
            }
        }

        private void endBounce()
        {
            bounceHeight = 0;
            bounceGravity = 0;
            bounceVelocity = 0;
            isBouncing = false;
        }

        public void PlayCollectionSound()
        {
            if (this.Type == PickupType.Heart)
                heartCollectSound.Play(0.75f, 0, 0);
            else
                coinCollectSound.Play(0.75f, 0, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            spriteBatch.Draw(shadowTexture,
               new Vector2(Center.X - (shadowTexture.Width / 2), Origin.Y + Height - (shadowTexture.Height / 2)),
               Color.White);

            bool shouldDraw = true;
            if (lifeTimer >= BLINK_START_TIME)
            {
                int n = lifeTimer % (BLINK_DELAY * 2);
                if (n < BLINK_DELAY)
                    shouldDraw = false;
            }
            if (shouldDraw)
            {
                if (isBouncing)
                    CurrentSprite.Draw(spriteBatch, new Vector2(Origin.X, Origin.Y - bounceHeight));
                else
                    base.Draw(spriteBatch, changeColorsEffect);
            }
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Player)
            {
                isAlive = false;
            }
        }
    }

    public enum PickupType
    {
        BronzeCoin,
        SilverCoin,
        GoldCoin,
        Heart,
        Key
    }
}

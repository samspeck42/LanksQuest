using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Adventure.Entities.MovementHandlers;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public class Pickup : Entity, ShadowOwner
    {
        private const int ANIMATION_DELAY = 65;
        private const int BRONZE_COIN_VALUE = 1;
        private const int SILVER_COIN_VALUE = 5;
        private const int GOLD_COIN_VALUE = 10;
        private const int HEART_VALUE = 2;
        private const int KEY_VALUE = 1;

        private const int LIFETIME = 5000;
        private const int BLINK_START_TIME = 3500;

        private const string BRONZE_COIN_SPRITE_ID = "bronze_coin_sprite";
        private const string SILVER_COIN_SPRITE_ID = "silver_coin_sprite";
        private const string GOLD_COIN_SPRITE_ID = "gold_coin_sprite";
        private const string HEART_SPRITE_ID = "heart_sprite";
        private const string KEY_SPRITE_ID = "key_sprite";
        private const string OBSTACLE_COLLISION_BOX_ID = "obstacle_collision_box";

        public override HitBox ObstacleCollisionBox { get { return GetHitBoxById(OBSTACLE_COLLISION_BOX_ID); } }

        public virtual ShadowSizes ShadowSize { get { return ShadowSizes.Small; } }
        public virtual Vector2 ShadowCenter
        {
            get
            {
                return new Vector2(
                    this.ObstacleCollisionBox.Left + (this.ObstacleCollisionBox.Width / 2),
                    this.ObstacleCollisionBox.Bottom - 3);
            }
        }

        public PickupType PickupType { get { return pickupType; } }
        public int Value { get { return value; } }

        private SoundEffect coinCollectSound;
        private SoundEffect heartCollectSound;

        private PickupType pickupType;
        private int value;
        private MovementHandler movementHandler;
        private bool isDropped = false;
        private bool hasStartedDrop = false;
        private long lifeTimer = 0;
        private bool canBeCollected = false;

        public Pickup(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            init(PickupType.BronzeCoin, false);
        }

        public Pickup(GameWorld game, Map map, Area area, PickupType type, bool isDropped)
            : base(game, map, area)
        {
            init(type, isDropped);
        }

        private void init(PickupType type, bool isDropped)
        {
            this.isDropped = isDropped;

            BoundingBox.RelativeX = 0;
            BoundingBox.RelativeY = 0;
            BoundingBox.Width = 14;
            BoundingBox.Height = 14;

            HitBox obstacleCollisionBox = new HitBox(this, OBSTACLE_COLLISION_BOX_ID);
            obstacleCollisionBox.RelativeX = 0;
            obstacleCollisionBox.RelativeY = 0;
            obstacleCollisionBox.Width = 14;
            obstacleCollisionBox.Height = 14;
            obstacleCollisionBox.IsActive = true;
            HitBoxes.Add(obstacleCollisionBox);

            Vector2 origin = new Vector2(0, 0);
            Sprite sprite = new Sprite("Sprites/Pickups/coin_bronze", this, origin, 6, ANIMATION_DELAY);
            spriteHandler.AddSprite(BRONZE_COIN_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Pickups/coin_silver", this, origin, 6, ANIMATION_DELAY);
            spriteHandler.AddSprite(SILVER_COIN_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Pickups/coin_gold", this, origin, 6, ANIMATION_DELAY);
            spriteHandler.AddSprite(GOLD_COIN_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Pickups/heart", this, origin);
            spriteHandler.AddSprite(HEART_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Pickups/key", this, origin);
            spriteHandler.AddSprite(KEY_SPRITE_ID, sprite);

            setType(type);

            movementHandler = new BounceMovementHandler(this, Vector2.Zero, 100, 240, 0.75f, 20, 4);
        }


        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            setType((PickupType)int.Parse(dataDict["type"]));
        }

        public override string ToString()
        {
            return "(" + base.ToString() + ")(" + ((int)PickupType).ToString() + ")";
        }

        private void setType(PickupType pickupType)
        {
            this.pickupType = pickupType;

            if (PickupType == PickupType.BronzeCoin)
            {
                spriteHandler.SetSprite(BRONZE_COIN_SPRITE_ID);
                value = BRONZE_COIN_VALUE;
            }
            else if (PickupType == PickupType.SilverCoin)
            {
                spriteHandler.SetSprite(SILVER_COIN_SPRITE_ID);
                value = SILVER_COIN_VALUE;
            }
            else if (PickupType == PickupType.GoldCoin)
            {
                spriteHandler.SetSprite(GOLD_COIN_SPRITE_ID);
                value = GOLD_COIN_VALUE;
            }
            else if (PickupType == PickupType.Heart)
            {
                spriteHandler.SetSprite(HEART_SPRITE_ID);
                value = HEART_VALUE;
            }
            else if (PickupType == PickupType.Key)
            {
                spriteHandler.SetSprite(KEY_SPRITE_ID);
                value = KEY_VALUE;
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            coinCollectSound = gameWorld.Content.Load<SoundEffect>("Audio/coin_collect");
            heartCollectSound = gameWorld.Content.Load<SoundEffect>("Audio/heart_collect");
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);

            movementHandler.Update(gameTime);

            if (isDropped)
            {
                if (!hasStartedDrop)
                {
                    startBounce();
                    hasStartedDrop = true;
                }

                lifeTimer += (long)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (lifeTimer >= LIFETIME)
                {
                    Die();
                }
                if (lifeTimer >= BLINK_START_TIME && !spriteHandler.IsBlinking)
                {
                    spriteHandler.StartBlinking();
                }
            }
        }

        private void startBounce()
        {
            movementHandler.Start();
        }

        public void PlayCollectionSound()
        {
            if (this.PickupType == PickupType.Heart)
                heartCollectSound.Play(0.75f, 0, 0);
            else
                coinCollectSound.Play(0.75f, 0, 0);
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (other is Player && thisHitBox.IsId(BOUNDING_BOX_ID) && canBeCollected)
            {
                Player player = (Player)other;
                player.CollectPickup(this);
                PlayCollectionSound();
                this.Die();
            }
        }

        public override void OnMovementEvent(MovementEvent movementEvent)
        {
            if (movementEvent == MovementEvent.CollisionWithGround && !canBeCollected)
            {
                canBeCollected = true;
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

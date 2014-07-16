using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TileEngine;

namespace Adventure
{
    public class Pot : LiftableEntity, Breakable, PickupDropper
    {
        private const float PICKUP_DROP_CHANCE = 0.9f;
        private const int DAMAGE = 2;

        private AnimatedSprite sprite;

        public Pot(GameWorld game, Area area)
            : base(game, area)
        {
            IsPassable = false;
            Damage = DAMAGE;

            Rectangle bounds = new Rectangle(0, 0, 26, 26);
            sprite = new AnimatedSprite(bounds);

            CurrentSprite = sprite;
        }

        public override void LoadContent()
        {
            sprite.Sprite = game.Content.Load<Texture2D>("Sprites/Environment/pot_small");
        }

        public void StartBreaking()
        {
            isAlive = false;
        }

        public float DropChance
        {
            get { return PICKUP_DROP_CHANCE; }
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
            if (other is Enemy)
            {
                if (isThrown)
                {
                    Velocity = Vector2.Zero;
                    StartBreaking();
                }
                    
            }
        }

        protected override void land()
        {
            StartBreaking();
        }
    }
}

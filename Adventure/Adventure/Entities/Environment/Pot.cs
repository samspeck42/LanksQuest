using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TileEngine;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public class Pot : CarriableEntity, Breakable, PickupDropper
    {
        private const float PICKUP_DROP_CHANCE = 0.9f;
        private const int DAMAGE = 2;

        private Sprite sprite;
        private SoundEffect breakingSound;

        public Pot(GameWorld game, Area area)
            : base(game, area)
        {
            IsPassable = false;
            Damage = DAMAGE;

            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 26;
            hitBoxHeight = 26;
            Vector2 origin = new Vector2(0, 0);
            sprite = new Sprite(origin);

            CurrentSprite = sprite;

            diesOutsideArea = true;
        }

        public override void LoadContent()
        {
            sprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/pot_small");
            breakingSound = game.Content.Load<SoundEffect>("Audio/pot_breaking");
        }

        public void StartBreaking()
        {
            isAlive = false;
            breakingSound.Play(0.5f, 0, 0);
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

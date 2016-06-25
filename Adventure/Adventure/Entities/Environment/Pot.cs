using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TileEngine;
using Microsoft.Xna.Framework.Audio;
using Adventure.Maps;
using Adventure.Entities.Enemies;

namespace Adventure.Entities.Environment
{
    public class Pot : CarriableEntity, Breakable, PickupDropper
    {
        private const string NORMAL_SPRITE_ID = "normal_sprite";

        public override bool IsObstacle { get { return !isThrown; } }
        public override int Damage { get { return 2; } }
        public override bool CanStartInteraction { get { return !isThrown && !isBroken; } }
        public bool CanBeBroken { get { return !isThrown && !isBroken; } }

        public float PickupDropChance { get { return 0.9f; } }
        public Vector2 PickupDropPosition
        {
            get { return new Vector2(Center.X, Center.Y + 2); }
        }

        private SoundEffect breakingSound;
        private bool isBroken = false;

        public Pot(GameWorld game, Map map, Area area)
            : base(game, map, area, 0)
        {
            BoundingBox.RelativeX = -11;
            BoundingBox.RelativeY = -11;
            BoundingBox.Width = 22;
            BoundingBox.Height = 22;

            ObstacleCollisionBox.RelativeX = -11;
            ObstacleCollisionBox.RelativeY = -11;
            ObstacleCollisionBox.Width = 22;
            ObstacleCollisionBox.Height = 22;

            Vector2 origin = new Vector2(12, 14);
            Sprite sprite = new Sprite("Sprites/Environment/pot_small", this, origin);
            spriteHandler.AddSprite(NORMAL_SPRITE_ID, sprite);
            spriteHandler.SetSprite(NORMAL_SPRITE_ID);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            breakingSound = gameWorld.Content.Load<SoundEffect>("Audio/pot_breaking");
        }

        public void StartBreaking()
        {
            Die();
            isBroken = true;

            area.SpawnPickup(this);
            breakingSound.Play(0.5f, 0, 0);
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            if (other is Enemy && thisHitBox.IsId(BOUNDING_BOX_ID) && this.isThrown)
            {
                Enemy enemy = (Enemy)other;
                if (enemy.TakesDamageFromPot(otherHitBox))
                {
                    isThrown = false;
                    StartBreaking();
                    enemy.TakeDamage(this, KnockBackType.FaceDirection);
                }
            }
        }

        protected override void land()
        {
            base.land();

            StartBreaking();
        }
    }
}

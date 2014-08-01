using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Adventure
{
    public class EnemyProjectile : Entity
    {
        private const float FIRE_SPEED = 2.4f;
        private const int ANIMATION_DELAY = 6;
        private const int LIFETIME = 600;
        private const int DAMAGE = 1;

        private Sprite bulletSprite;
        private Sprite fireballSprite;

        private int lifeTimer = 0;
        private EnemyProjectileType type;
        public EnemyProjectileType Type { get { return type; } }

        public EnemyProjectile(GameWorld game, Area area, EnemyProjectileType type)
            : base(game, area)
        {
            this.type = type;

            Vector2 origin = new Vector2(0, 0);
            bulletSprite = new Sprite(origin, 3, ANIMATION_DELAY);
            origin = new Vector2(12, 12);
            fireballSprite = new Sprite(origin, 3, ANIMATION_DELAY);

            if (type == EnemyProjectileType.Bullet)
            {
                CurrentSprite = bulletSprite;
                hitBoxWidth = 16;
                hitBoxHeight = 16;
            }
            else if (type == EnemyProjectileType.Fireball)
            {
                hitBoxOffset = new Vector2(-10, -10);
                CurrentSprite = fireballSprite;
                hitBoxWidth = 20;
                hitBoxHeight = 20;
            }
            Damage = DAMAGE;
            IsAffectedByWallCollisions = true;
            IsInAir = true;
        }

        public override void LoadContent()
        {
            bulletSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/bullet");
            fireballSprite.Texture = game.Content.Load<Texture2D>("Sprites/Enemies/fireball");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            lifeTimer++;
            if (lifeTimer >= LIFETIME || JustCollidedWithWall)
                isAlive = false;
        }

        public void Fire(float angle)
        {
            Velocity.X = (float)Math.Cos(angle) * FIRE_SPEED;
            Velocity.Y = (float)Math.Sin(angle) * FIRE_SPEED;

            if (type == EnemyProjectileType.Fireball)
                CurrentSprite.Rotation = angle;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Player)
                isAlive = false;
        }
    }

    public enum EnemyProjectileType
    {
        Bullet,
        Fireball
    }
}

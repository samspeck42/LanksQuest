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

        private AnimatedSprite bulletSprite;
        private AnimatedSprite fireballSprite;

        private int lifeTimer = 0;
        private EnemyProjectileType type;
        public EnemyProjectileType Type { get { return type; } }

        public EnemyProjectile(GameWorld game, Area area, EnemyProjectileType type)
            : base(game, area)
        {
            this.type = type;

            Rectangle bounds = new Rectangle(0, 0, 16, 16);
            bulletSprite = new AnimatedSprite(bounds, 3, ANIMATION_DELAY);
            bounds = new Rectangle(2, 2, 20, 20);
            fireballSprite = new AnimatedSprite(bounds, 3, ANIMATION_DELAY);

            if (type == EnemyProjectileType.Bullet)
                CurrentSprite = bulletSprite;
            else if (type == EnemyProjectileType.Fireball)
                CurrentSprite = fireballSprite;

            Damage = DAMAGE;
            IsAffectedByWallCollisions = true;
            IsInAir = true;
        }

        public override void LoadContent()
        {
            bulletSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/bullet");
            fireballSprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/fireball");
        }

        public override void Update()
        {
            base.Update();

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adventure.Maps;

namespace Adventure.Entities
{
    public abstract class Mob : ActivatingEntity, ShadowOwner
    {
        public virtual int MaxHealth { get { return 1; } }
        public int Health { get { return health; } }

        public virtual ShadowSizes ShadowSize { get { return ShadowSizes.Medium; } }
        public virtual Vector2 ShadowCenter
        {
            get
            {
                return new Vector2(
                    this.ObstacleCollisionBox.Left + (this.ObstacleCollisionBox.Width / 2),
                    this.ObstacleCollisionBox.Bottom - 5);
            }
        }

        private int health;


        public Mob(GameWorld game, Map map, Area area)
            : base(game, map, area) { }

        public virtual void TakeDamage(Entity other, KnockBackType knockBackType)
        {
            setHealth(health - other.Damage);
        }

        protected void setHealth(int health)
        {
            this.health = health <= MaxHealth ? health : MaxHealth;
        }

        protected void increaseHealth(int amount)
        {
            setHealth(health + amount);
        }

        protected void decreaseHealth(int amount)
        {
            setHealth(health - amount);
        }
    }

    public enum KnockBackType
    {
        None,
        HitAngle,
        FaceDirection
    }
}

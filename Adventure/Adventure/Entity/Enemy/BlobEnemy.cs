using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class BlobEnemy : Enemy, PickupDropper, Activatable
    {
        private const float PICKUP_DROP_CHANCE = 0.75f;

        const float WALK_SPEED = 1.0f;
        const int WALK_ANIMATION_DELAY = 6;
        const int MOVE_TIME = 60;
        const int MAX_HEALTH = 2;
        const int DAMAGE = 1;

        private int moveTimer = MOVE_TIME;
        private int[] moves = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
        private int currentMove;

        private AnimatedSprite sprite;

        public BlobEnemy(GameWorld game, Area area)
            : base(game, area)
        {
            Rectangle bounds = new Rectangle(2, 2, 22, 24);
            sprite = new AnimatedSprite(bounds, 8, WALK_ANIMATION_DELAY);

            CurrentSprite = sprite;

            currentMove = moves.Count();

            MaxHealth = MAX_HEALTH;
            Health = MaxHealth;
            Damage = DAMAGE;

            CanLeaveArea = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            sprite.Sprite = game.Content.Load<Texture2D>("Sprites/Enemies/blob_enemy");
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void updateAI()
        {
            moveTimer++;
            if (moveTimer >= MOVE_TIME)
            {
                moveTimer = 0;
                Velocity = Vector2.Zero;

                currentMove++;
                if (currentMove >= moves.Count())
                {
                    currentMove = 0;
                    List<int> temp = new List<int>();
                    foreach (int k in moves)
                        temp.Add(k);
                    for (int k = 0; k < moves.Count(); k++)
                    {
                        int r = GameWorld.Random.Next(temp.Count);
                        moves[k] = temp.ElementAt(r);
                        temp.RemoveAt(r);
                    }

                }

                int n = moves[currentMove];
                switch (n)
                {
                    case 0:
                        Velocity.Y += WALK_SPEED;
                        break;
                    case 1:
                        Velocity.Y -= WALK_SPEED;
                        break;
                    case 2:
                        Velocity.X += WALK_SPEED;
                        break;
                    case 3:
                        Velocity.X -= WALK_SPEED;
                        break;
                    default:
                        break;
                }
            }
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Pot)
            {
                Pot pot = (Pot)other;
                if (pot.IsThrown && state == EnemyState.Normal)
                    //!IsHurt)
                {
                    takeDamageFrom(pot);
                }
            }
            if (other is Arrow)
            {
                Arrow arrow = (Arrow)other;
                if (arrow.IsFired && state == EnemyState.Normal)
                    //!IsHurt)
                {
                    if (this.Contains(arrow.TipPosition))
                    {
                        takeDamageFrom(arrow);
                        arrow.HitEntity(this);
                    }
                }
            }
        }

        public override void  ReactToSwordHit(Player player)
        {
            //if (!IsHurt)
            if (state == EnemyState.Normal)
            {
                takeDamageFrom(player);
            }
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

        public float DropChance
        {
            get { return PICKUP_DROP_CHANCE; }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            
        }
    }
}

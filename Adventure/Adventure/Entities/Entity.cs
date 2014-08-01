using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;
using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace Adventure
{
    public abstract class Entity: IComparable
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Directions4 FaceDirection;
        public bool IsOnGround = false;
        public bool IsInAir = false;

        private Sprite currentSprite;
        public Sprite CurrentSprite { 
            get { return currentSprite; }
            set { currentSprite = value; }
        }

        //protected Vector2 hitBoxOffset = Vector2.Zero;
        //protected int hitBoxWidth = 0;
        //protected int hitBoxHeight = 0;
        public int Width { get { return BoundingBox.Width; } }
        public int Height { get { return BoundingBox.Height; } }

        //public Rectangle HitBox2
        //{
        //    get
        //    {
        //        return new Rectangle((int)Math.Round(Position.X + hitBoxOffset.X), (int)Math.Round(Position.Y + hitBoxOffset.Y),
        //            hitBoxWidth, hitBoxHeight);
        //    }
        //}
        //public Vector2 HitBoxPosition
        //{
        //    get
        //    {
        //        return new Vector2(Position.X + hitBoxOffset.X, Position.Y + hitBoxOffset.Y);
        //    }
        //    set
        //    {
        //        Position.X = value.X - hitBoxOffset.X;
        //        Position.Y = value.Y - hitBoxOffset.Y;
        //    }
        //}
        //public float HitBoxPositionX { set { HitBoxPosition = new Vector2(value, HitBoxPosition.Y); } }
        //public float HitBoxPositionY { set { HitBoxPosition = new Vector2(HitBoxPosition.X, value); } }

        public Vector2 Center
        {
            get { return new Vector2(BoundingBox.ActualX + (Width / 2), BoundingBox.ActualY + (Height / 2)); }
            set
            {
                BoundingBox.ActualX = value.X - (Width / 2);
                BoundingBox.ActualY = value.Y - (Height / 2);
            }
        }

        protected bool isAlive = true;
        public bool IsAlive { get { return isAlive; } }
        private bool justCollidedWithWall = false;
        public bool JustCollidedWithWall { get { return justCollidedWithWall; } }
        public virtual bool IsBlockedByObstacleEntities { get { return false; } }
        public virtual bool IsObstacle { get { return false; } }
        public bool CanLeaveArea = true;
        protected bool diesOutsideArea = false;
        public bool DiesOutsideArea { get { return diesOutsideArea; } }
        protected bool isActive = true;
        public bool IsActive { get { return isActive; } }
        protected bool shouldBeDrawnByArea = true;
        public bool ShouldBeDrawnByArea { get { return shouldBeDrawnByArea; } }
        public int MaxHealth;
        public int Health;
        public int Damage;
        private Point spawnCell = new Point();
        public Point SpawnCell { get { return spawnCell; } }
        protected string id = "";
        public string Id { get { return id; } }

        protected GameWorld game;
        public GameWorld Game { get { return game; } }
        protected Area area;
        public Area Area { get { return area; } }



        private HitBox boundingBox;
        public HitBox BoundingBox { get { return boundingBox; } }
        private List<HitBox> hitBoxes;
        public List<HitBox> HitBoxes { get { return hitBoxes; } }



        public Entity(GameWorld game, Area area)
        {
            Position = new Vector2();
            Velocity = new Vector2();
            Acceleration = new Vector2();

            currentSprite = null;

            this.game = game;
            this.area = area;


            boundingBox = new HitBox(this, HitBox.BOUNDING_BOX_ID);
            boundingBox.IsActive = true;
            hitBoxes = new List<HitBox>();
            hitBoxes.Add(boundingBox);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (CurrentSprite != null)
                CurrentSprite.Update(gameTime);

            if (Health > MaxHealth)
                Health = MaxHealth;

            Velocity += Acceleration;

            if (game != null)
            {
                if (IsBlockedByObstacleEntities)
                    handleObstacleCollisions();
                else
                    Position += Velocity;

                //if (!CanLeaveArea && game.CurrentArea != null)
                //{
                //    HitBoxPosition = 
                //        new Vector2(MathHelper.Clamp(HitBoxPosition.X, 0f, game.CurrentArea.WidthInPixels - Width),
                //        MathHelper.Clamp(HitBoxPosition.Y, 0f, game.CurrentArea.HeightInPixels - Height));
                //}
                if (diesOutsideArea && area != null && !area.IsEntityInside(this))
                {
                    isAlive = false;
                }
            }


        }

        private void handleObstacleCollisions()
        {
            justCollidedWithWall = false;

            int topCellY = (int)Math.Round(BoundingBox.Top) / Area.TILE_HEIGHT;
            int bottomCellY = (int)Math.Round(BoundingBox.Bottom - 1) / Area.TILE_HEIGHT;
            int leftCellX = (int)Math.Round(BoundingBox.Left) / Area.TILE_WIDTH;
            int rightCellX = (int)Math.Round(BoundingBox.Right - 1) / Area.TILE_WIDTH;
            int x = 0, y = 0;
            float collisionDist = 0;
            Point curCell = new Point();
            bool collided = false;
            List<Entity> entitiesCollidedWithX = new List<Entity>();
            List<TileCollision> impassableTileCollisions = new List<TileCollision>();
            impassableTileCollisions.Add(TileCollision.Obstacle);
            if (!CanLeaveArea)
                impassableTileCollisions.Add(TileCollision.Doorway);

            if (Velocity.X > 0 || Velocity.X < 0) //moving horizontally
            {
                // check impassable tile collisions
                for (int c = 0; c < 2; c++)
                {
                    if (Velocity.X > 0) //moving right
                    {
                        x = rightCellX + 1 + c;
                        collisionDist = (x * Area.TILE_WIDTH) - BoundingBox.Right;
                    }
                    else //moving left
                    {
                        x = leftCellX - 1 - c;
                        collisionDist = ((x + 1) * Area.TILE_WIDTH) - BoundingBox.Left;
                    }
                    for (y = topCellY; y <= bottomCellY; y++)
                    {
                        curCell = new Point(x, y);
                        if (impassableTileCollisions.Contains(game.CurrentArea.GetCollisionAtCell(curCell)) &&
                            Math.Abs(collisionDist) < Math.Abs(Velocity.X))
                        {
                            //a collision with an impassable tile will occur 
                            collided = true;
                            break;
                        }
                    }

                    if (collided)
                        break;
                }

                // check impassable entity collisions
                List<Entity> entitiesInPath = game.CurrentArea.GetObstacleEntitiesInCollisionPathX(this);
                foreach (Entity entity in entitiesInPath)
                {
                    float dist = Velocity.X > 0 ? entity.BoundingBox.Left - this.BoundingBox.Right : entity.BoundingBox.Right - this.BoundingBox.Left;
                    if (Math.Abs(dist) < Math.Abs(Velocity.X))
                    {
                        if (collided && (dist <= collisionDist))
                        {
                            collisionDist = dist;

                            if (dist < collisionDist)
                                entitiesCollidedWithX.Clear();

                            entitiesCollidedWithX.Add(entity);
                        }
                        else if (!collided)
                        {
                            collisionDist = dist;
                            collided = true;

                            entitiesCollidedWithX.Add(entity);
                        }
                    }
                }

                if (collided)
                {
                    Position.X += collisionDist;
                    Velocity.X = 0f;
                    Acceleration.X = 0f;
                    justCollidedWithWall = true;
                }
            }

            Position.X += Velocity.X;


            topCellY = (int)Math.Round(BoundingBox.Top) / Area.TILE_HEIGHT;
            bottomCellY = (int)Math.Round(BoundingBox.Bottom - 1) / Area.TILE_HEIGHT;
            leftCellX = (int)Math.Round(BoundingBox.Left) / Area.TILE_WIDTH;
            rightCellX = (int)Math.Round(BoundingBox.Right - 1) / Area.TILE_WIDTH;
            collisionDist = 0;
            collided = false;
            List<Entity> entitiesCollidedWithY = new List<Entity>();

            if (Velocity.Y > 0 || Velocity.Y < 0) //moving vertically
            {
                for (int r = 0; r < 2; r++)
                {
                    if (Velocity.Y > 0) //moving down
                    {
                        y = bottomCellY + 1 + r;
                        collisionDist = (y * Area.TILE_HEIGHT) - BoundingBox.Bottom;
                    }
                    else //moving up
                    {
                        y = topCellY - 1 - r;
                        collisionDist = ((y + 1) * Area.TILE_HEIGHT) - BoundingBox.Top;
                    }
                    for (x = leftCellX; x <= rightCellX; x++)
                    {
                        curCell = new Point(x, y);
                        if (impassableTileCollisions.Contains(game.CurrentArea.GetCollisionAtCell(curCell)) &&
                            Math.Abs(collisionDist) < Math.Abs(Velocity.Y))
                        {
                            //a collision with an impassable tile will occur
                            collided = true;
                            break;
                        }
                    }

                    if (collided)
                        break;
                }

                // check impassable entity collisions
                List<Entity> entitiesInPath = game.CurrentArea.GetObstacleEntitiesInCollisionPathY(this);
                foreach (Entity entity in entitiesInPath)
                {
                    float dist = Velocity.Y > 0 ? entity.BoundingBox.Top - this.BoundingBox.Bottom : entity.BoundingBox.Bottom - this.BoundingBox.Top;
                    if (Math.Abs(dist) < Math.Abs(Velocity.Y))
                    {
                        if (collided && (dist <= collisionDist))
                        {
                            collisionDist = dist;

                            if (dist < collisionDist)
                                entitiesCollidedWithY.Clear();
                            entitiesCollidedWithY.Add(entity);
                        }
                        else if (!collided)
                        {
                            collisionDist = dist;
                            collided = true;

                            entitiesCollidedWithY.Add(entity);
                        }
                    }
                }

                if (collided)
                {
                    Position.Y += collisionDist;
                    Velocity.Y = 0f;
                    Acceleration.Y = 0f;
                    justCollidedWithWall = true;
                }
            }

            Position.Y += Velocity.Y;

            List<Entity> entitiesCollidedWith = new List<Entity>();
            entitiesCollidedWith.AddRange(entitiesCollidedWithX);
            entitiesCollidedWith.AddRange(entitiesCollidedWithY);
            foreach (Entity e in entitiesCollidedWith)
            {
                this.OnEntityCollision(e, this.BoundingBox, e.BoundingBox);
                e.OnEntityCollision(this, e.BoundingBox, this.BoundingBox);
            }
        }

        public bool CollidesWith(Entity other)
        {
            return this.BoundingBox.CollidesWith(other.BoundingBox);
        }

        public bool Contains(Vector2 position)
        {
            return this.BoundingBox.Contains(position);
        }

        public static bool RectangleContains(Rectangle rectangle, Vector2 point)
        {
            return rectangle.Contains(new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)));
        }

        public abstract void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox);

        public abstract void LoadContent();

        public void LoadDataFromString(string data)
        {
            Dictionary<string, string> dataDict = parseData(data);
            processData(dataDict);
        }

        protected virtual void processData(Dictionary<string, string> dataDict)
        {
            string[] pos = dataDict["spawnCell"].Split(',');
            spawnCell = new Point(int.Parse(pos[0].Trim()), int.Parse(pos[1].Trim()));
            Vector2 spawnPosition = new Vector2(
                (spawnCell.X * Area.TILE_WIDTH) + (Area.TILE_WIDTH / 2),
                (spawnCell.Y * Area.TILE_HEIGHT) + (Area.TILE_HEIGHT / 2));
            //this.Origin = spawnPosition;
            this.Center = spawnPosition;
            if (dataDict.ContainsKey("id"))
            {
                this.id = dataDict["id"];
            }
        }

        public override string ToString()
        {
            return spawnCell.X.ToString() + "," + spawnCell.Y.ToString();
        }

        public virtual void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            CurrentSprite.Draw(spriteBatch, Position);
        }

        public static Entity CreateEntityFromString(string str, GameWorld game, Area area)
        {
            Entity entity = null;
            string entityTypeName = "Adventure." + str.Substring(0, str.IndexOf(" ")).Trim();
            string entityData = str.Substring(str.IndexOf(" ") + 1).Trim();

            entity = (Entity)Assembly.GetExecutingAssembly().CreateInstance(entityTypeName,
                    true,
                    0,
                    null,
                    new Object[] { game, area },
                    null,
                    null);

            if (entity != null)
            {
                entity.LoadDataFromString(entityData);
                if (game != null)
                    entity.LoadContent();
            }

            return entity;
        }

        protected Dictionary<string, string> parseData(string data)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();

            int depth = 0;
            char c;
            string str = "";
            char[] dataCharArray = data.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                c = dataCharArray[i];

                if (c == '(')
                {
                    depth++;
                    if (depth == 1)
                        str = "";
                    else
                        str += c;
                }
                else if (c == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        string key = str.Substring(0, str.IndexOf(':')).Trim();
                        string value = str.Substring(str.IndexOf(':') + 1).Trim();
                        dataDict.Add(key, value);
                    }
                    else
                        str += c;
                }
                else
                {
                    str += c;
                }
            }

            return dataDict;
        }

        /// <summary>
        /// Returns a hit box this entity owns with the given id.
        /// </summary>
        /// <param name="id">The id of the hit box to be returned</param>
        /// <returns></returns>
        public HitBox GetHitBoxById(string id)
        {
            foreach (HitBox hitBox in HitBoxes)
            {
                if (hitBox.IsId(id))
                    return hitBox;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of active hit boxes (i.e. hit boxes that will be used in entity collision detection)
        /// that this entity owns.
        /// </summary>
        /// <returns></returns>
        public List<HitBox> GetActiveHitBoxes()
        {
            List<HitBox> activeHitBoxes = new List<HitBox>();
            foreach (HitBox hitBox in HitBoxes)
            {
                if (hitBox.IsActive)
                    activeHitBoxes.Add(hitBox);
            }
            return activeHitBoxes;
        }

        public int CompareTo(object obj)
        {
            if (obj is Entity)
            {
                Entity other = (Entity)obj;
                if (other.IsOnGround && !this.IsOnGround)
                    return 1;
                else if (this.IsOnGround && !other.IsOnGround)
                    return -1;
                else if (other.IsInAir && !this.IsInAir)
                    return -1;
                else if (this.IsInAir && !other.IsInAir)
                    return 1;
                return (int)((this.BoundingBox.Bottom) - (other.BoundingBox.Bottom));
            }
            return 0;
        }
    }

    
}

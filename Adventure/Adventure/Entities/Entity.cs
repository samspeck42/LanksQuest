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
        public const string BOUNDING_BOX_ID = "bounding_box";

        public Vector2 Position = Vector2.Zero;
        public Directions4 FaceDirection = Directions4.Down;

        public Sprite CurrentSprite
        {
            get { return spriteHandler.CurrentSprite; }
        }

        public int Width { get { return BoundingBox.Width; } }
        public int Height { get { return BoundingBox.Height; } }

        public Vector2 Center
        {
            get { return new Vector2(BoundingBox.ActualX + (Width / 2), BoundingBox.ActualY + (Height / 2)); }
            set
            {
                BoundingBox.ActualX = value.X - (Width / 2);
                BoundingBox.ActualY = value.Y - (Height / 2);
            }
        }

        public virtual TileCollision ObstacleTileCollisions { get { return TileCollision.Wall; } }
        public virtual bool IsBlockedByObstacleEntities { get { return false; } }
        public virtual bool IsObstacle { get { return false; } }
        public virtual int Damage { get { return 0; } }
        public virtual DrawLayer DrawLayer { get { return DrawLayer.Middle; } }
        public virtual bool CanLeaveArea { get { return false; } }

        public bool IsAlive { get { return isAlive; } }
        public bool IsVisible { get { return isVisible; } }
        public bool ShouldBeDrawnByArea { get { return shouldBeDrawnByArea; } }
        public Point SpawnCell { get { return spawnCell; } }
        public string Id { get { return id; } }
        public GameWorld Game { get { return game; } }
        public Area Area { get { return area; } }

        public HitBox BoundingBox { get { return GetHitBoxById(BOUNDING_BOX_ID); } }
        /// <summary>
        /// Bounding box is used for obstacle collisions by default, override this property to specify a different
        /// hit box.
        /// </summary>
        public virtual HitBox ObstacleCollisionBox { get { return GetHitBoxById(BOUNDING_BOX_ID); } }
        public List<HitBox> HitBoxes { get { return hitBoxes; } }

        protected SpriteHandler spriteHandler;
        private bool isAlive = true;
        protected bool diesOutsideArea = false;
        protected bool isVisible = true;
        protected bool shouldBeDrawnByArea = true;
        private Point spawnCell = new Point();
        protected string id = "";

        protected GameWorld game;
        protected Area area;

        private List<HitBox> hitBoxes;


        public Entity(GameWorld game, Area area)
        {
            Position = new Vector2();
            
            this.spriteHandler = new SpriteHandler(this);

            this.game = game;
            this.area = area;

            hitBoxes = new List<HitBox>();

            HitBox boundingBox = new HitBox(this, BOUNDING_BOX_ID);
            boundingBox.IsActive = true;
            hitBoxes.Add(boundingBox);
        }

        public virtual void LoadContent()
        {
            spriteHandler.Load(game.Content);
        }

        /// <summary>
        /// Override and implement this method to provide the entity with update logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            spriteHandler.Draw(spriteBatch);

            //foreach (HitBox hitBox in GetActiveHitBoxes())
            //{
            //    spriteBatch.Draw(game.SquareTexture,
            //        hitBox.ToRectangle(),
            //        Color.White);
            //}
        }

        /// <summary>
        /// Kills this entity, causing it to be removed from the area on the next update.
        /// </summary>
        public virtual void Die()
        {
            isAlive = false;
        }

        /// <summary>
        /// Determines whether the bounding box of this entity collides with bounding box of another.
        /// </summary>
        /// <param name="other">The entity to perform a collision check with.</param>
        /// <returns>True if the entities collide, false if they don't.</returns>
        public bool CollidesWith(Entity other)
        {
            return this.BoundingBox.CollidesWith(other.BoundingBox);
        }

        public bool Contains(Vector2 point)
        {
            return this.BoundingBox.Contains(point);
        }

        /// <summary>
        /// Override and implement this method to provide the entity with entity collision logic.
        /// </summary>
        /// <param name="other">The entity collided with.</param>
        /// <param name="thisHitBox">The hit box belonging to this entity that was involved in the collision.</param>
        /// <param name="otherHitBox">The hit box belonging to the other entity that was involved in the collision.</param>
        public virtual void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox) { }

        protected virtual void processAttributeData(Dictionary<string, string> dataDict)
        {
            spawnCell = Parser.ParsePoint(dataDict["spawnCell"]);
            Vector2 spawnPosition = new Vector2(
                (spawnCell.X * Area.TILE_WIDTH) + (Area.TILE_WIDTH / 2),
                (spawnCell.Y * Area.TILE_HEIGHT) + (Area.TILE_HEIGHT / 2));
            this.Position = spawnPosition;
            if (dataDict.ContainsKey("id"))
            {
                this.id = dataDict["id"];
            }
            if (dataDict.ContainsKey("faceDirection"))
            {
                this.FaceDirection = (Directions4)Enum.Parse(typeof(Directions4), dataDict["faceDirection"]);
            }
        }

        public override string ToString()
        {
            return spawnCell.X.ToString() + "," + spawnCell.Y.ToString();
        }

        /// <summary>
        /// Creates an Entity from a string read from an area file.
        /// </summary>
        /// <param name="str">A string containing the entity data read from an area file.</param>
        /// <param name="game">A GameWorld instance to create this entity within.</param>
        /// <param name="area">An Area instance to create this entity within.</param>
        /// <returns>An Entity instance created from the data string.</returns>
        public static Entity CreateFromString(string str, GameWorld game, Area area)
        {
            Entity entity = null;
            string entityTypeName = "Adventure." + str.Substring(0, str.IndexOf(" ")).Trim();
            string entityDataString = str.Substring(str.IndexOf(" ") + 1).Trim();
            Dictionary<string, string> entityAttributeData = Parser.ParseAttributeData(entityDataString);

            entity = (Entity)Assembly.GetExecutingAssembly().CreateInstance(entityTypeName,
                    true,
                    0,
                    null,
                    new Object[] { game, area },
                    null,
                    null);

            if (entity != null)
            {
                entity.processAttributeData(entityAttributeData);
                if (game != null)
                    entity.LoadContent();
            }

            return entity;
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

        /// <summary>
        /// Override and implement this method to respond to a movement event.
        /// </summary>
        /// <param name="movementEvent">The movement event that occurred.</param>
        public virtual void OnMovementEvent(MovementEvent movementEvent) { }

        /// <summary>
        /// Determines whether the player should take damage when colliding with the
        /// given hit box.
        /// </summary>
        /// <param name="thisHitBox">The hit box  belonging to this entity that the
        /// player collided with.</param>
        /// <returns></returns>
        public virtual bool DamagesPlayer(HitBox thisHitBox, out KnockBackType knockBackType)
        {
            knockBackType = KnockBackType.None;
            return false;
        }

        public virtual bool ActivatesPressureSwitch()
        {
            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj is Entity)
            {
                Entity other = (Entity)obj;
                if (other.DrawLayer == DrawLayer.Low && this.DrawLayer != DrawLayer.Low)
                    return 1;
                else if (this.DrawLayer == DrawLayer.Low && other.DrawLayer != DrawLayer.Low)
                    return -1;
                else if (other.DrawLayer == DrawLayer.High && this.DrawLayer != DrawLayer.High)
                    return -1;
                else if (this.DrawLayer == DrawLayer.High && other.DrawLayer != DrawLayer.High)
                    return 1;
                return (int)((this.BoundingBox.Bottom) - (other.BoundingBox.Bottom));
            }
            return 0;
        }
    }

    public enum DrawLayer
    {
        High,
        Middle,
        Low
    }
}

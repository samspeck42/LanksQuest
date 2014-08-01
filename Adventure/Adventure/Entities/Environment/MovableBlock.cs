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
    public class MovableBlock : ActivatingEntity, Activatable
    {
        private const String SPRITE_IMAGE_NAME = "Tiles/dungeon_block";

        private Sprite sprite;
        private SoundEffect pushSound;

        private Point destinationCell;
        private bool isBeingPushed;
        private Directions4 pushDirection;
        private bool hasMoved = false;
        private List<Directions4> movableDirections = new List<Directions4>();

        public MovableBlock(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 32;
            hitBoxHeight = 32;
            Vector2 origin = new Vector2(0, 0);
            sprite = new Sprite(origin);

            CurrentSprite = sprite;
            IsAffectedByWallCollisions = false;
            IsPassable = false;
            IsOnGround = true;
            destinationCell = new Point();
            isBeingPushed = false;
            pushDirection = Directions4.Up;
        }

        public override void LoadContent()
        {
            sprite.Texture = game.Content.Load<Texture2D>("Tiles/dungeon_block");
            pushSound = game.Content.Load<SoundEffect>("Audio/block_push");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isBeingPushed)
                doPush();
        }

        public override void OnEntityCollision(Entity other)
        {
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("movableDirections"))
            {
                string[] movableDirectionsData = dataDict["movableDirections"].Split(';');
                foreach (string str in movableDirectionsData)
                {
                    movableDirections.Add((Directions4)int.Parse(str));
                }
            }
        }

        private void setDestinationCell(Directions4 direction)
        {
            destinationCell = Area.ConvertPositionToCell(Center);

            if (direction == Directions4.Left)
                destinationCell.X--;
            else if (direction == Directions4.Right)
                destinationCell.X++;
            else if (direction == Directions4.Up)
                destinationCell.Y--;
            else if (direction == Directions4.Down)
                destinationCell.Y++;
        }

        public bool CanBePushed(Directions4 direction)
        {
            setDestinationCell(direction);
            return movableDirections.Contains(direction) && !hasMoved &&
                area.GetCollisionAtCell(destinationCell) == TileEngine.TileCollision.Passable;
        }

        public void StartBeingPushed(Directions4 direction)
        {
            setDestinationCell(direction);
            IsPassable = true;
            isBeingPushed = true;
            pushDirection = direction;
            pushSound.Play(0.5f, 0, 0);
        }

        private void doPush()
        {
            if (pushDirection == Directions4.Left)
                Position.X = game.Player.HitBoxPosition.X - Width;
            else if (pushDirection == Directions4.Right)
                Position.X = game.Player.HitBoxPosition.X + game.Player.Width;
            else if (pushDirection == Directions4.Up)
                Position.Y = game.Player.HitBoxPosition.Y - Height;
            else if (pushDirection == Directions4.Down)
                Position.Y = game.Player.HitBoxPosition.Y + game.Player.Height;
        }

        public bool ReachedPushDestination()
        {
            float distanceToDestination = 0;

            if (pushDirection == Directions4.Left)
                distanceToDestination = Position.X - (destinationCell.X * Area.TILE_WIDTH);
            else if (pushDirection == Directions4.Right)
                distanceToDestination = (destinationCell.X * Area.TILE_WIDTH) - Position.X;
            else if (pushDirection == Directions4.Up)
                distanceToDestination = Position.Y - (destinationCell.Y * Area.TILE_HEIGHT);
            else if (pushDirection == Directions4.Down)
                distanceToDestination = (destinationCell.Y * Area.TILE_HEIGHT) - Position.Y;

            if (distanceToDestination <= 0)
                return true;
            return false;
        }

        public void EndPush()
        {
            Position = new Vector2(destinationCell.X * Area.TILE_WIDTH,
                destinationCell.Y * Area.TILE_HEIGHT);
            IsPassable = false;
            isBeingPushed = false;
            hasMoved = true;

            tryToTriggerActivations();
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }
    }
}

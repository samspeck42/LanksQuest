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
        private Directions pushDirection;
        private bool hasMoved = false;
        private List<Directions> movableDirections = new List<Directions>();

        public MovableBlock(GameWorld game, Area area)
            : base(game, area)
        {
            Rectangle bounds = new Rectangle(0, 0, 32, 32);
            sprite = new Sprite(bounds);

            CurrentSprite = sprite;
            IsAffectedByWallCollisions = false;
            IsPassable = false;
            IsOnGround = true;
            destinationCell = new Point();
            isBeingPushed = false;
            pushDirection = Directions.Up;
        }

        public override void LoadContent()
        {
            sprite.Texture = game.Content.Load<Texture2D>("Tiles/dungeon_block");
            pushSound = game.Content.Load<SoundEffect>("Audio/block_push");
        }

        public override void Update()
        {
            base.Update();

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
                    movableDirections.Add((Directions)int.Parse(str));
                }
            }
        }

        private void setDestinationCell(Directions direction)
        {
            destinationCell = Area.ConvertPositionToCell(Center);

            if (direction == Directions.Left)
                destinationCell.X--;
            else if (direction == Directions.Right)
                destinationCell.X++;
            else if (direction == Directions.Up)
                destinationCell.Y--;
            else if (direction == Directions.Down)
                destinationCell.Y++;
        }

        public bool CanBePushed(Directions direction)
        {
            setDestinationCell(direction);
            return movableDirections.Contains(direction) && !hasMoved &&
                area.GetCollisionAtCell(destinationCell) == TileEngine.TileCollision.Passable;
        }

        public void StartBeingPushed(Directions direction)
        {
            setDestinationCell(direction);
            IsPassable = true;
            isBeingPushed = true;
            pushDirection = direction;
            pushSound.Play(0.5f, 0, 0);
        }

        private void doPush()
        {
            if (pushDirection == Directions.Left)
                Position.X = game.Player.Position.X - Width;
            else if (pushDirection == Directions.Right)
                Position.X = game.Player.Position.X + game.Player.Width;
            else if (pushDirection == Directions.Up)
                Position.Y = game.Player.Position.Y - Height;
            else if (pushDirection == Directions.Down)
                Position.Y = game.Player.Position.Y + game.Player.Height;
        }

        public bool ReachedPushDestination()
        {
            float distanceToDestination = 0;

            if (pushDirection == Directions.Left)
                distanceToDestination = Position.X - (destinationCell.X * Area.TILE_WIDTH);
            else if (pushDirection == Directions.Right)
                distanceToDestination = (destinationCell.X * Area.TILE_WIDTH) - Position.X;
            else if (pushDirection == Directions.Up)
                distanceToDestination = Position.Y - (destinationCell.Y * Area.TILE_HEIGHT);
            else if (pushDirection == Directions.Down)
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

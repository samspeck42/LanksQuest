using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TileEngine;
using Microsoft.Xna.Framework.Audio;
using Adventure.Entities.MovementHandlers;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public class MovableBlock : ActivatingEntity, Interactable, Triggerable
    {
        private const string NORMAL_SPRITE_ID = "normal_sprite";

        public override bool IsObstacle { get { return true; } }
        public bool CanStartInteraction { get { return !isMoving; } }
        public bool MustBeAllignedWithToInteract { get { return true; } }

        private SoundEffect pushSound;

        private MovementHandler movementHandler = null;
        private bool isMoving = false;
        private bool hasMoved = false;
        private List<Directions4> movableDirections = new List<Directions4>();

        public MovableBlock(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            BoundingBox.RelativeX = -16;
            BoundingBox.RelativeY = -16;
            BoundingBox.Width = 32;
            BoundingBox.Height = 32;

            Vector2 origin = new Vector2(16, 16);
            Sprite sprite = new Sprite("Tiles/dungeon_block", this, origin);
            spriteHandler.AddSprite(NORMAL_SPRITE_ID, sprite);
            spriteHandler.SetSprite(NORMAL_SPRITE_ID);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            pushSound = gameWorld.Content.Load<SoundEffect>("Audio/block_push");
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            if (dataDict.ContainsKey("movableDirections"))
            {
                string[] movableDirectionsData = dataDict["movableDirections"].Split(';');
                foreach (string str in movableDirectionsData)
                {
                    movableDirections.Add((Directions4)Enum.Parse(typeof(Directions4), str));
                }
            }
        }

        public override bool ActivatesPressureSwitch()
        {
            return !isMoving;
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);

            if (isMoving && movementHandler != null)
            {
                movementHandler.Update(gameTime);

                if (movementHandler.IsFinished)
                {
                    isMoving = false;
                    hasMoved = true;

                    tryToTriggerActivations();
                }
            }
        }

        public void StartInteraction()
        {
            if (CanStartInteraction)
                gameWorld.Player.StartGrabbing(this);
        }

        public bool TryToStartMoving(Directions4 direction)
        {
            if (movableDirections.Contains(direction) && !hasMoved)
            {
                isMoving = true;
                Vector2 velocity = DirectionsHelper.GetDirectionVector(direction) * 80;
                movementHandler = new StraightMovementHandler(this, velocity, Area.CELL_WIDTH);
                movementHandler.Start();

                pushSound.Play(0.5f, 0, 0);
                return true;
            }
            return false;
        }

        public void TriggerOn()
        {
            isVisible = true;
        }

        public void TriggerOff()
        {
            isVisible = false;
        }
    }
}

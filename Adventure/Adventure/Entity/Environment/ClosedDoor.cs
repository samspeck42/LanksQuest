using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class ClosedDoor : Door, Activatable
    {
        private Sprite closedSprite;
        private Sprite openingSprite;
        private Sprite closingSprite;
        private bool isClosing = false;

        public ClosedDoor(GameWorld game, Area area)
            : base(game, area)
        {
            init();

            CurrentSprite = closedSprite;
        }

        public ClosedDoor(GameWorld game, Area area, Directions faceDirection)
            : base(game, area)
        {
            init();

            setFaceDirection(faceDirection);
        }

        private void init()
        {
            hitBoxOffset = new Vector2(-16, -16);
            hitBoxWidth = 32;
            hitBoxHeight = 32;

            Vector2 origin = new Vector2(16, 16);
            closedSprite = new Sprite(origin);
            openingSprite = new Sprite(origin, 8, 2, 1);
            closingSprite = new Sprite(origin, 8, 2, 1);

            CurrentSprite = closedSprite;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            closedSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/closed_door");
            openingSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/closed_door_opening");
            closingSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/closed_door_closing");
        }

        public override void Update()
        {
            base.Update();

            if (isOpening && CurrentSprite.IsDoneAnimating)
            {
                IsPassable = true;
                isOpen = true;
                isOpening = false;
            }
            else if (isClosing && CurrentSprite.IsDoneAnimating)
            {
                CurrentSprite = closedSprite;
                setFaceDirection(FaceDirection);
                isClosing = false;
                closeSound.Play(0.75f, 0, 0);
            }
        }

        public void Activate()
        {
            if (!isOpen)
                StartOpening();
        }

        public void Deactivate()
        {
            if (isOpen)
                StartClosing();
        }

        public override void StartOpening()
        {
            if (game.CurrentArea.Entities.Contains(this))
            {
                isOpening = true;
                CurrentSprite = openingSprite;
                CurrentSprite.ResetAnimation();
                setFaceDirection(FaceDirection);
                openSound.Play(1f, 0, 0);
            }
            else
            {
                IsPassable = true;
                isOpen = true;
            }
        }

        public void StartClosing()
        {
            IsPassable = false;
            isOpen = false;

            if (game.CurrentArea.Entities.Contains(this))
            {
                isClosing = true;
                CurrentSprite = closingSprite;
                CurrentSprite.ResetAnimation();
                setFaceDirection(FaceDirection);
            }
            else
            {
                CurrentSprite = closedSprite;
                setFaceDirection(FaceDirection);
            }
        }

        public override void OnEntityCollision(Entity other)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Effect changeColorsEffect)
        {
            if (!isOpen)
                base.Draw(spriteBatch, changeColorsEffect);
        }
    }
}

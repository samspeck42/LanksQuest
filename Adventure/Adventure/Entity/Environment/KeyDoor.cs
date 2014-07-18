using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class KeyDoor : Door
    {
        private Sprite sprite;
        private Sprite openingSprite;

        public KeyDoor(GameWorld game, Area area)
            : base(game, area)
        {
            init();
        }

        public KeyDoor(GameWorld game, Area area, Directions faceDirection)
            : base(game, area)
        {
            init();

            setFaceDirection(faceDirection);
        }

        private void init()
        {
            Rectangle bounds = new Rectangle(0, 0, 32, 32);
            sprite = new Sprite(bounds);
            openingSprite = new Sprite(bounds, 8, 2, 1);

            CurrentSprite = sprite;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            sprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/key_door");
            openingSprite.Texture = game.Content.Load<Texture2D>("Sprites/Environment/key_door_opening");
        }

        public override void Update()
        {
            base.Update();

            if (isOpening && CurrentSprite.IsDoneAnimating)
            {
                isAlive = false;
            }
        }

        public override void StartOpening()
        {
            isOpening = true;
            CurrentSprite = openingSprite;
            setFaceDirection(FaceDirection);
            openSound.Play(1f, 0, 0);
        }

        public override void OnEntityCollision(Entity other)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Adventure
{
    public abstract class Door : Entity
    {
        protected SoundEffect openSound;
        protected SoundEffect closeSound;

        protected bool isOpening = false;
        protected bool isOpen = false;

        public Door(GameWorld game, Area area)
            : base(game, area)
        {
            IsPassable = false;
        }

        public override void LoadContent()
        {
            openSound = game.Content.Load<SoundEffect>("Audio/door_open");
            closeSound = game.Content.Load<SoundEffect>("Audio/door_close");
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            setFaceDirection((Directions4)int.Parse(dataDict["faceDirection"]));
            isOpen = bool.Parse(dataDict["isOpen"]);

            if (isOpen)
                IsPassable = true;
        }

        public override string ToString()
        {
            return "(" + base.ToString() + ")(" + ((int)FaceDirection).ToString() + ")(" + isOpen.ToString() + ")";
        }

        protected void setFaceDirection(Directions4 faceDirection)
        {
            this.FaceDirection = faceDirection;

            if (FaceDirection == Directions4.Up)
                CurrentSprite.Rotation = MathHelper.Pi;
            else if (FaceDirection == Directions4.Down)
                CurrentSprite.Rotation = 0f;
            else if (FaceDirection == Directions4.Left)
                CurrentSprite.Rotation = MathHelper.PiOver2;
            else if (FaceDirection == Directions4.Right)
                CurrentSprite.Rotation = 3f * MathHelper.PiOver2;
        }

        public abstract void StartOpening();
    }
}

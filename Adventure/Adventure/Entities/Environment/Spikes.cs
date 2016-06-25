using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Adventure.Maps;

namespace Adventure.Entities.Environment
{
    public class Spikes : Entity, Triggerable
    {
        private const string UP_SPRITE_ID = "up_sprite";
        private const string DOWN_SPRITE_ID = "down_sprite";

        public override int Damage
        {
            get
            {
                return 1;
            }
        }
        public override DrawLayer DrawLayer
        {
            get
            {
                return DrawLayer.Low;
            }
        }
        public override bool IsObstacle
        {
            get
            {
                return isUp;
            }
        }

        private bool isUp = true;

        public Spikes(GameWorld game, Map map, Area area)
            : base(game, map, area)
        {
            BoundingBox.RelativeX = -16;
            BoundingBox.RelativeY = -16;
            BoundingBox.Width = 32;
            BoundingBox.Height = 32;

            Vector2 origin = new Vector2(16, 16);
            Sprite sprite = new Sprite("Sprites/Environment/spikes_activated", this, origin);
            spriteHandler.AddSprite(UP_SPRITE_ID, sprite);
            sprite = new Sprite("Sprites/Environment/spikes_deactivated", this, origin);
            spriteHandler.AddSprite(DOWN_SPRITE_ID, sprite);

            spriteHandler.SetSprite(UP_SPRITE_ID);
        }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            if (dataDict.ContainsKey("isUp"))
                isUp = bool.Parse(dataDict["isUp"]);

            if (!isUp)
            {
                spriteHandler.SetSprite(DOWN_SPRITE_ID);
            }
        }

        public override bool DamagesPlayer(HitBox thisHitBox, out KnockBackType knockBackType)
        {
            knockBackType = KnockBackType.None;
            //return isUp;
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            spriteHandler.Update(gameTime);
        }

        //public Vector2 GetKnockBackDirection()
        //{
        //    Vector2 direction = Vector2.Zero;
        //    bool spikesAbove = false, spikesBelow = false, spikesLeft = false, spikesRight = false;

        //    foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X, SpawnCell.Y - 1)))
        //    {
        //        if (entity is Spikes)
        //            spikesAbove = true;
        //    }
        //    foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X, SpawnCell.Y + 1)))
        //    {
        //        if (entity is Spikes)
        //            spikesBelow = true;
        //    }
        //    foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X - 1, SpawnCell.Y)))
        //    {
        //        if (entity is Spikes)
        //            spikesLeft = true;
        //    }
        //    foreach (Entity entity in area.GetActiveEntitiesAtCell(new Point(SpawnCell.X + 1, SpawnCell.Y)))
        //    {
        //        if (entity is Spikes)
        //            spikesRight = true;
        //    }

        //    if (spikesAbove && spikesBelow)
        //        direction = new Vector2(game.Player.Center.X - this.Center.X, 0);
        //    else if (spikesLeft && spikesRight)
        //        direction = new Vector2(0, game.Player.Center.Y - this.Center.Y);
        //    else
        //        direction = game.Player.Center - this.Center;

        //    direction.Normalize();
        //    return direction;
        //}

        public void TriggerOn()
        {
            isUp = true;
            spriteHandler.SetSprite(UP_SPRITE_ID);
        }

        public void TriggerOff()
        {
            isUp = false;
            spriteHandler.SetSprite(DOWN_SPRITE_ID);
        }
    }
}

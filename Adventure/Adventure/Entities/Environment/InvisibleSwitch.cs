using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class InvisibleSwitch : ActivatingEntity, Activatable
    {
        public InvisibleSwitch(GameWorld game, Area area)
            : base(game, area)
        {
            hitBoxOffset = Vector2.Zero;
            hitBoxWidth = 32;
            hitBoxHeight = 32;

            CurrentSprite = new Sprite(new Vector2(0, 0));
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

            if (dataDict.ContainsKey("isActive"))
            {
                isActive = bool.Parse(dataDict["isActive"]);
            }
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Player)
            {
                Player player = (Player)other;
                if (!hasTriggeredActivations && this.Contains(player.FootPosition))
                {
                    tryToTriggerActivations();
                }
            }
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

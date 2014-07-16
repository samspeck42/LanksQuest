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
            CurrentSprite = new AnimatedSprite(new Rectangle(0, 0, 32, 32));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class PlayerSpriteHandler
    {
        public const string STILL_SPRITES_ID = "still";
        public const string WALKING_SPRITES_ID = "walking";
        public const string ATTACKING_SPRITES_ID = "attacking";

        private Dictionary<string, SpriteSet> spriteSetDict = new Dictionary<string, SpriteSet>();
        private Sprite currentSprite;
        private Player player;

        public PlayerSpriteHandler(Player player)
        {
            this.player = player;
        }

        public void SetSpriteStill()
        {
            Sprite newSprite = spriteSetDict[STILL_SPRITES_ID].GetSprite(player.FaceDirection);
            currentSprite = newSprite;
        }

        public bool IsCurrentSpriteStill()
        {
            return spriteSetDict[STILL_SPRITES_ID].ContainsSprite(currentSprite);
        }

        public bool IsCurrentSpriteWalking()
        {
            return spriteSetDict[WALKING_SPRITES_ID].ContainsSprite(currentSprite);
        }

        public bool IsCurrentSpriteAttacking()
        {
            return spriteSetDict[ATTACKING_SPRITES_ID].ContainsSprite(currentSprite);
        }

        public bool IsCurrentSprite(string spriteSetId)
        {
            return spriteSetDict[spriteSetId].ContainsSprite(currentSprite);
        }
    }
}

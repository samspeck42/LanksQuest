using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adventure
{
    public class SpriteSet
    {
        private Sprite[] sprites = new Sprite[4];

        public SpriteSet(Sprite upSprite, Sprite downSprite, Sprite leftSprite, Sprite rightSprite)
        {
            sprites[0] = upSprite;
            sprites[1] = downSprite;
            sprites[2] = leftSprite;
            sprites[3] = rightSprite;
        }

        public Sprite GetSprite(Directions direction)
        {
            return sprites[(int)direction];
        }

        public bool ContainsSprite(Sprite sprite)
        {
            return sprites.Contains(sprite);
        }
    }
}

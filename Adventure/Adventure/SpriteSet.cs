using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Adventure
{
    public class SpriteSet
    {
        private Sprite[] sprites = new Sprite[4];

        public void SetSprite(Directions4 direction, Sprite sprite)
        {
            int index = (int)direction;
            if (index >= 0 && index < sprites.Length)
                sprites[index] = sprite;
        }

        public Sprite GetSprite(Directions4 direction)
        {
            int index = (int)direction;
            if (index < 0)
                index = 0;
            return sprites[index];
        }

        public bool ContainsSprite(Sprite sprite)
        {
            return sprites.Contains(sprite);
        }

        public void LoadContent(ContentManager content)
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite != null)
                    sprite.LoadContent(content);
            }
        }
    }
}

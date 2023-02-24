

using UnityEngine;

namespace Myd.Platform.Demo
{
    public partial class PlayerController
    {
        public float DashTrailTimer { get; set; }

        public static Vector2 NORMAL_SPRITE_SCALE = Vector2.one;
        public Vector2 DUCK_SPRITE_SCALE = new Vector2(1F, 0.75f);

        public Vector2 CurrSpriteScale { get; set; } = NORMAL_SPRITE_SCALE;

        //处理缩放
        private void UpdateSprite(float deltaTime)
        {
            Vector2 tempScale = Scale;
            tempScale.x = Mathf.MoveTowards(tempScale.x, CurrSpriteScale.x, 1.75f * deltaTime);
            tempScale.y = Mathf.MoveTowards(tempScale.y, CurrSpriteScale.y, 1.75f * deltaTime);
            Scale = tempScale;
        }
    }


}

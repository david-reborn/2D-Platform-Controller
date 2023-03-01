

using UnityEngine;

namespace Myd.Platform.Demo
{
    public partial class PlayerController
    {
        public float DashTrailTimer { get; set; }

        //public static Vector2 NORMAL_SPRITE_SCALE = Vector2.one;
        //public Vector2 DUCK_SPRITE_SCALE = new Vector2(1F, 0.75f);

        //public Vector2 CurrSpriteScale { get; set; } = NORMAL_SPRITE_SCALE;

        public Color NormalHairColor = new Color32(0xAC, 0x32, 0x32, 0xFF);//00FFA2
        public Color UsedHairColor = new Color32(0x44, 0xB7, 0xFF, 0xFF);
        public Color FlashHairColor = Color.white;


        ////处理缩放
        //private void UpdateSprite(float deltaTime)
        //{
        //    float tempScaleX = Mathf.MoveTowards(Scale.x, CurrSpriteScale.x, 1.75f * deltaTime);
        //    float tempScaleY = Mathf.MoveTowards(Scale.y, CurrSpriteScale.y, 1.75f * deltaTime);
        //    this.Scale = new Vector2(tempScaleX, tempScaleY);
        //}
    }


}

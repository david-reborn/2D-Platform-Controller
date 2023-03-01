using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    /// <summary>
    /// 角色渲染器，用于处理角色表现逻辑
    /// </summary>
    public class PlayerRenderer
    {
        public static Vector2 NORMAL_SPRITE_SCALE = Vector2.one;
        public static Vector2 DUCK_SPRITE_SCALE = new Vector2(1F, 0.75f);

        private Vector2 scale;
        private Vector2 currSpriteScale = NORMAL_SPRITE_SCALE;

        private SpriteRenderer spriteRenderer;
        public PlayerRenderer(Player player)
        {
            this.spriteRenderer = player.SpriteRenderer;
        }

        public void Init()
        {
            EventManager.Get().OnDuck += HandleOnDuck;
            EventManager.Get().OnFall += HandleOnFall;
            EventManager.Get().OnJump += HandleOnJump;
            EventManager.Get().OnFallLand += HandleOnFallLand;
        }

        public void Update(float deltaTime)
        {
            //更新
            UpdateSprite(deltaTime);

            //UpdateHair(deltaTime);
        }

        private void UpdateSprite(float deltaTime)
        {
            float tempScaleX = Mathf.MoveTowards(scale.x, currSpriteScale.x, 1.75f * deltaTime);
            float tempScaleY = Mathf.MoveTowards(scale.y, currSpriteScale.y, 1.75f * deltaTime);
            this.scale = new Vector2(tempScaleX, tempScaleY);

            this.spriteRenderer.transform.localScale = scale;
        }

        private void HandleOnDuck(bool enable)
        {
            if (enable)
            {
                this.scale = new Vector2(1.4f, .6f);
                this.currSpriteScale = DUCK_SPRITE_SCALE;
            }
            else
            {
                this.scale = new Vector2(.8f, 1.2f);
                this.currSpriteScale = NORMAL_SPRITE_SCALE;
            }
        }

        private void HandleOnJump()
        {
            this.scale = new Vector2(.6f, 1.4f);

            //this.player.PlayJumpEffect();

            //蹬墙的粒子效果
        }

        //根据下落速度，进行缩放表现
        private void HandleOnFall(float ySpeed)
        {
            float half = Constants.MaxFall + (Constants.FastMaxFall - Constants.MaxFall) * .5f;
            if (ySpeed <= half)
            {
                float spriteLerp = Mathf.Min(1f, (ySpeed - half) / (Constants.FastMaxFall - half));
                Vector2 scale = Vector2.zero;
                scale.x = Mathf.Lerp(1f, 0.5f, spriteLerp);
                scale.y = Mathf.Lerp(1f, 1.5f, spriteLerp);
                this.scale = scale;
            }
        }

        private void HandleOnFallLand(float ySpeed)
        {
            float squish = Mathf.Min(ySpeed / Mathf.Abs(Constants.FastMaxFall), 1);
            float scaleX = Mathf.Lerp(1, 1.6f, squish);
            float scaleY = Mathf.Lerp(1, 0.4f, squish);
            this.scale = new Vector2(scaleX, scaleY);
        }

    }
}
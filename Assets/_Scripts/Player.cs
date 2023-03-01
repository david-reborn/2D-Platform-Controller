using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public struct ControllerParams
    {
        public bool UseCornerCorrection;
    }

    public interface IPlayerContext
    {

    }
    public class Player : MonoBehaviour, IPlayerContext
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private TrailRenderer trailRenderer;
        [SerializeField]
        private ParticleSystem vfxMoveDust;
        [SerializeField]
        private ParticleSystem vfxJumpDust;
        [SerializeField]
        private ParticleSystem vfxFallDust;
        [Tooltip("参数")]
        [SerializeField]
        [Header("使用边界校正")]
        private bool UseCornerCorrection;

        PlayerController controller;
        
        public SpriteRenderer SpriteRenderer => this.spriteRenderer;

        private void OnValidate()
        {
            if (controller == null)
                return;
            Debug.Log("==刷新控制器参数");
            controller.ResetControllerParams(new ControllerParams() { UseCornerCorrection = this.UseCornerCorrection });
        }

        void Start()
        {
            Init();
            controller = new PlayerController(this);
            controller.Init(this.transform.position);
            
            this.vfxJumpDust.Stop();
        }

        void Update()
        {
            //响应输入
            Input.Update(Time.deltaTime);

            //更新角色控制器
            controller.Update(Time.deltaTime);

            //更新角色渲染器
            Render();
        }

        private void Render()
        {
            RenderSprite(Time.deltaTime);

            Vector2 scale = this.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (int)controller.Facing;
            this.transform.localScale = scale;
            this.transform.position = controller.Position;

            //if (!lastFrameOnGround && this.controller.OnGround)
            //{
            //    this.vfxMoveDust.Play();
            //}
            //if (lastFrameOnGround && !this.controller.OnGround)
            //    this.vfxMoveDust.Stop();
            //lastFrameOnGround = this.controller.OnGround;

            //if (this.controller.IsFall)
            //{
            //    PlayFallEffect();
            //}
        }

        public void PlayJumpEffect()
        {
            this.vfxJumpDust.transform.position = this.spriteRenderer.transform.position;
            this.vfxJumpDust.Play();
        }

        public void PlayFallEffect()
        {
            this.vfxFallDust.transform.position = this.spriteRenderer.transform.position;
            this.vfxFallDust.Play();
        }

        public void SetTrailColor(Gradient gradient)
        {
            trailRenderer.colorGradient = gradient;
        }

        #region Renderer
        public static Vector2 NORMAL_SPRITE_SCALE = Vector2.one;
        public static Vector2 DUCK_SPRITE_SCALE = new Vector2(1F, 0.75f);

        private Vector2 scale;
        private Vector2 currSpriteScale = NORMAL_SPRITE_SCALE;

        private void Init()
        {
            EventManager.Get().OnDuck += HandleOnDuck;
            EventManager.Get().OnFall += HandleOnFall;
            EventManager.Get().OnJump += HandleOnJump;
            EventManager.Get().OnFallLand += HandleOnFallLand;
        }

        private void RenderSprite(float deltaTime)
        {
            float tempScaleX = Mathf.MoveTowards(scale.x, currSpriteScale.x, 1.75f * deltaTime);
            float tempScaleY = Mathf.MoveTowards(scale.y, currSpriteScale.y, 1.75f * deltaTime);
            this.scale = new Vector2(tempScaleX, tempScaleY);

            this.spriteRenderer.transform.localScale = scale;

            //处理Tail的效果
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
        #endregion
    }

    //测试用的绘制接口
    public enum EGizmoDrawType
    {
        SlipCheck,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public struct ControllerParams
    {
        public bool UseCornerCorrection;
    }

    //上下文数据，用于在多个组件间传递
    public class PlayerContext
    {
        public Vector2 Speed;  //当前的移动速度
    }
    public class Player : MonoBehaviour
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

        PlayerContext context;
        PlayerController controller;
        PlayerRenderer renderer;
        
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
            context = new PlayerContext();
            controller = new PlayerController(this, new ControllerParams() { UseCornerCorrection= this.UseCornerCorrection });
            controller.Init(this.transform.position);

            renderer = new PlayerRenderer(this);
            renderer.Init();
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
            renderer.Update(Time.deltaTime);
        }

        private bool lastFrameOnGround = false;

        private void Render()
        {
            Vector2 scale = this.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (int)controller.Facing;
            this.transform.localScale = scale;
            this.transform.position = controller.Position;
            //this.spriteRenderer.transform.localScale = controller.Scale;

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

        private void OnDrawGizmos()
        {
            if (controller == null)
                return;
            //Rect rect = new Rect(0, -0.25f, 0.8f, 1.1f);
            //Gizmos.DrawLine(controller.Position + rect.position, controller.Position + rect.position + Vector2.right);

            controller.Draw(EGizmoDrawType.SlipCheck);
        }

        public void SetTrailColor(Gradient gradient)
        {
            trailRenderer.colorGradient = gradient;
        }
    }

    //测试用的绘制接口
    public enum EGizmoDrawType
    {
        SlipCheck,
    }
}

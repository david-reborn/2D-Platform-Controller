using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{

    public struct ControllerParams
    {
        public bool UseCornerCorrection;
    }
    public class Player : MonoBehaviour
    {
        PlayerController controller;
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
            controller = new PlayerController(this, new ControllerParams() { UseCornerCorrection= this.UseCornerCorrection });
            controller.Init(this.transform.position);

            this.vfxJumpDust.Stop();
            //trailRenderer.material = new Material(Shader.Find("Sprites/Default"));

        }

        void Update()
        {
            Input.Update(Time.deltaTime);
            controller.Update(Time.deltaTime);
            Render();
        }

        private bool lastFrameOnGround = false;

        private void Render()
        {
            Vector2 scale = this.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (int)controller.Facing;
            this.transform.localScale = scale;
            this.transform.position = controller.Position;
            this.spriteRenderer.transform.localScale = controller.Scale;

            if (!lastFrameOnGround && this.controller.OnGround)
            {
                this.vfxMoveDust.Play();
            }
            if (lastFrameOnGround && !this.controller.OnGround)
                this.vfxMoveDust.Stop();
            lastFrameOnGround = this.controller.OnGround;

            if (this.controller.IsFall)
            {
                PlayFallEffect();
            }
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

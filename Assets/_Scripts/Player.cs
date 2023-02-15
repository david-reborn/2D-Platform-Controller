using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class Player : MonoBehaviour
    {
        PlayerController controller;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private ParticleSystem vfxMoveDust;
        void Start()
        {
            controller = new PlayerController(spriteRenderer);
            controller.Init(this.transform.position);
        }

        void Update()
        {
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
        }

        private void OnDrawGizmos()
        {
            if (controller == null)
                return;
            Rect rect = new Rect(0, -0.25f, 0.8f, 1.1f);
            Gizmos.DrawLine(controller.Position + rect.position, controller.Position + rect.position + Vector2.right);
        }
    }
}

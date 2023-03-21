using Myd.Platform.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform
{

    /// <summary>
    /// 这里是Unity下实现玩家表现接口
    /// </summary>
    public class PlayerRenderer : MonoBehaviour, ISpriteControl
    {
        [SerializeField]
        public SpriteRenderer spriteRenderer;
        private Vector2 scale;
        private Vector2 currSpriteScale;

        public Vector3 SpritePosition { get => this.spriteRenderer.transform.position; }

        public void Reload()
        {

        }

        public void Render(float deltaTime)
        {
            float tempScaleX = Mathf.MoveTowards(scale.x, currSpriteScale.x, 1.75f * deltaTime);
            float tempScaleY = Mathf.MoveTowards(scale.y, currSpriteScale.y, 1.75f * deltaTime);
            this.scale = new Vector2(tempScaleX, tempScaleY);
            this.spriteRenderer.transform.localScale = scale;
        }

        public void Trail(int face)
        {
            SceneEffectManager.Instance.Add(this.spriteRenderer, face, Color.white);
        }

        public void Scale(Vector2 scale)
        {
            this.scale = scale;
        }

        public void SetSpriteScale(Vector2 scale)
        {
            this.currSpriteScale = scale;
        }
    }

    //测试用的绘制接口
    public enum EGizmoDrawType
    {
        SlipCheck,
        ClimbCheck,
    }
}

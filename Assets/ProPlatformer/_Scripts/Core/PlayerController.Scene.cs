

using Myd.Platform.Core;
using UnityEngine;

namespace Myd.Platform
{

    /// <summary>
    /// Controller关于表现相关
    /// </summary>
    public partial class PlayerController 
    {
        private Vector2 cameraPosition;

        private Bounds bounds;

        protected void UpdateCamera(float deltaTime)
        {
            var from = cameraPosition;
            var target = CameraTarget;
            var multiplier = 1f;

            cameraPosition = from + (target - from) * (1f - (float)Mathf.Pow(0.01f / multiplier, deltaTime));
        }

        public Vector2 GetCameraPosition() 
        {
            return cameraPosition;
        }

        protected Vector2 CameraTarget
        {
            get
            {
                Vector2 at = new Vector2();
                Vector2 target = new Vector2(this.Position.x, this.Position.y);

                at.x = Mathf.Clamp(target.x, bounds.min.x + 3200 / 100 / 2f, bounds.max.x - 3200 / 100 / 2f);
                at.y = Mathf.Clamp(target.y, bounds.min.y + 1800 / 100 / 2f, bounds.max.y - 1800 / 100 / 2f);
                return at;
            }
        }
    }


}

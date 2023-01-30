using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class Player : MonoBehaviour
    {
        PlayerController controller;
        void Start()
        {
            controller = new PlayerController();

            controller.Init();

            controller.Position = this.transform.position;
        }

        void Update()
        {
            controller.Update(Time.deltaTime);

            controller.Render();

            this.transform.position = controller.Position;
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

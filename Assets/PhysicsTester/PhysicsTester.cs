using System.Collections;
using UnityEngine;

namespace Myd.Tester
{
    /// <summary>
    /// 进行各种的物理测试
    /// 包括Overlap和Cast
    /// </summary>
    public class PhysicsTester : MonoBehaviour
    {
        RaycastHit2D hit;

        public Transform base1;
        public Transform base2;
        void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryTest();
            }
        }

        private void TryTest()
        {
            Debug.Log($"============误差正值表示靠近,负值表示远离============");

            TryTestBoxCast(0);
            TryTestBoxCast(0.1f);
            TryTestBoxCast(0.01f);
            TryTestBoxCast(0.001f);
            TryTestBoxCast(-0.1f);
            TryTestBoxCast(-0.01f);
            TryTestBoxCast(-0.001f);

            TryTestBoxOverlap(0);
            TryTestBoxOverlap(0.1f);
            TryTestBoxOverlap(0.01f);
            TryTestBoxOverlap(0.001f);
            TryTestBoxOverlap(-0.1f);
            TryTestBoxOverlap(-0.01f);
            TryTestBoxOverlap(-0.001f);
        }

        private void TryTestBoxCast(float deviation)
        {
            Debug.Log($"============测试BoxCast,误差{deviation}============");
            hit = Physics2D.BoxCast((Vector2)base1.position + Vector2.up * 1, Vector2.one, 0, Vector2.down, 0.5f + deviation);
            if (hit)
            {
                Debug.Log($"命中:命中点[{hit.point.x},{hit.point.y}]");
            }
            else
            {
                Debug.Log($"未命中");
            }
        }

        private void TryTestBoxOverlap(float deviation)
        {
            Debug.Log($"============测试BoxOverlap,误差{deviation}============");
            bool overlap = Physics2D.OverlapBox((Vector2)base2.position + Vector2.up * 0.5f + Vector2.down * (deviation), Vector2.one, 0);
            if (overlap)
            {
                Debug.Log($"被覆盖");
            }
            else
            {
                Debug.Log($"未覆盖");
            }
        }
    }
}
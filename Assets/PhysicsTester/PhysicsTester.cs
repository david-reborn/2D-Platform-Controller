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

        public 

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
            Debug.Log($"============误差({0.002499998f==0.0025f})============");
            Debug.Log($"============误差正值表示靠近,负值表示远离============");
            TryTestBoxCast(0.01f);
            TryTestBoxOverlap(-0.02f);
            TryTestBoxOverlap(-0.021f);
            TryTestBoxOverlap(-0.0225f);
            TryTestBoxOverlap(-0.025f);
            TryTestBoxOverlap(-0.03f);
        }

        private void TryTestBoxCast(float deviation)
        {
            Debug.Log($"============测试BoxCast,误差{deviation}============");
            hit = Physics2D.BoxCast((Vector2)base1.position + Vector2.up * 1, Vector2.one, 0, Vector2.down, 0.5f + deviation);
            if (hit)
            {
                Debug.Log($"命中:命中点[{hit.point.x},{hit.point.y}===={hit.point.y==0.0025f}]");
            }
            else
            {
                Debug.Log($"未命中");
            }
        }

        private void TryTestBoxOverlap(float deviation)
        {
            Debug.Log($"============测试BoxOverlap,误差{deviation}============");
            bool overlap = Physics2D.OverlapBox((Vector2)base2.position + Vector2.up * (0.5f - deviation), Vector2.one * (1), 0);
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
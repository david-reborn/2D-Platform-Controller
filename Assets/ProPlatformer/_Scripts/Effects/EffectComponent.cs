
using Myd.Common;
using UnityEngine;

namespace Myd.Platform
{
    /// <summary>
    /// 特效组件
    /// </summary>
    public class EffectComponent : MonoBehaviour
    {
        [Header("启用拖尾特效")]
        public bool EnableTrailEffect;
        [SerializeField]
        [Header("Trail Effect")]
        private GameObject trailEffect;

        [SerializeField]
        [Header("头巾挂载节点")]
        private Transform HeadMount;

        private void Awake()
        {

        }
        public void OnValidate()
        {
            Debug.Log("=======更新所有特效配置参数");
            trailEffect.GetComponent<TrailRenderer>().enabled = (EnableTrailEffect);
        }

        public void LateUpdate()
        {
            if (EnableTrailEffect)
            {
                this.trailEffect.transform.position = HeadMount.transform.position;
            }
        }

    }

}

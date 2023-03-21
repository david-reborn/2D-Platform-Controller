using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform
{
    [CreateAssetMenu(fileName = "EffectParam", menuName = "Pro Platformer/Effect Param", order = 2)]
    public class EffectParam : ScriptableObject
    {
        public List<EffectConfig> effectConfigs;
    }

    [Serializable]
    public class EffectConfig
    {
        [SerializeField]
        public string Keyword;
        [SerializeField]
        public GameObject Prefab;
        [SerializeField]
        public int Size;    //特效个数如果大于1，则进行池化。单个则隐藏
    }
}
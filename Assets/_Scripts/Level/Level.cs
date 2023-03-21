using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Myd.Platform
{
    public class Level : MonoBehaviour
    {
        private Transform platformRootNode;
        [SerializeField]
        [Header("平台颜色组")]
        Color[] platformColor;

        public void Awake()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i).name == "Platforms")
                {
                    this.platformRootNode = this.transform.GetChild(i);
                }
            }
        }

        void Start()
        {
            foreach(var spriteRenderer in this.platformRootNode.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.color = platformColor[Random.Range(0, platformColor.Length)];
            }
        }
    }
}
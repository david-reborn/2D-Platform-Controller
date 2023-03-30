using System.Collections;
using UnityEngine;

namespace Myd.Platform
{
    
    public class RippleEffect : MonoBehaviour
    {
        //强度
        public int Intensity;
        public int WaveSpeed;
        public float TotalTime;
        public Material material;
        //起始缩放比率
        private Vector2 scale1;
        //结束缩放比率
        private Vector2 scale2;

        private float liveTime;
        private void Awake()
        {
            this.scale1 = Vector2.one;
            this.scale2 = Vector2.one * 2;
            material = this.GetComponent<SpriteRenderer>().material;
        }

        public void Update()
        {
            if (liveTime >= TotalTime)
            {
                this.gameObject.SetActive(false);
            }
            liveTime += Time.deltaTime;
            this.transform.localScale = this.transform.localScale + Vector3.one * WaveSpeed * Time.deltaTime;
            this.material.SetFloat("_DistortIntensity", (1 - Mathf.Clamp(liveTime / TotalTime, 0, 1)) * Intensity);
        }

        public void Ripple(Vector3 position)
        {
            this.gameObject.SetActive(true);
            this.transform.localScale = this.scale1;
            this.transform.position = position;
            this.liveTime = 0;
        }
    }
}
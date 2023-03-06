using System.Collections;
using UnityEngine;

namespace Assets._Scripts.PostProcess
{
    
    public class RippleEffect : MonoBehaviour
    {
        public Material material;

        private void Awake()
        {
        }
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }


    }
}
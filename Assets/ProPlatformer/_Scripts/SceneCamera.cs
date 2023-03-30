using Cinemachine;
using DG.Tweening;
using Myd.Common;
using Myd.Platform.Core;
using System.Collections;
using UnityEngine;


namespace Myd.Platform
{
    /// <summary>
    /// ÉãÏñ»ú
    /// </summary>
    public class SceneCamera : MonoBehaviour, ICamera
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Camera postCamera;

        private Vector2 offset;

        [SerializeField]
        private float ShakeStrength = 1;
        [SerializeField]
        private AnimationCurve ShakeCurve = new AnimationCurve(new Keyframe[] 
            { 
                new Keyframe(0, -1.4f, -7.9f, -7.9f),
                new Keyframe(0.27f, 0.78f, 23.4f, 23.4f),
                new Keyframe(0.54f, -0.12f, 22.6f, 22.6f),
                new Keyframe(0.75f, 0.042f, 9.23f, 9.23f),
                new Keyframe(0.9f, -0.02f, 5.8f, 5.8f),
                new Keyframe(0.95f, -0.006f, -3.0f, -3.0f),
                new Keyframe(1, 0, 0, 0)
            });

        public void SetCameraPosition(Vector2 cameraPosition)
        {
            this.mainCamera.transform.position = new Vector3(cameraPosition.x+offset.x, cameraPosition.y + offset.y, -10);
        }

        public void Shake(Vector2 dir, float duration)
        {
            StartCoroutine(DOShake(dir, duration));
            //Vector3 orignalPosition = transform.position;
            //float elapsed = 0f;

            //while (elapsed < duration)
            //{
            //    float x = Random.Range(-1f, 1f) * magnitude;
            //    float y = Random.Range(-1f, 1f) * magnitude;

            //    transform.position = new Vector3(x, y, -10f);
            //    elapsed += Time.deltaTime;
            //    yield return 0;
            //}
            //transform.position = orignalPosition;
        }

        public IEnumerator DOShake(Vector2 dir, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = ShakeCurve.Evaluate(elapsed / duration) * ShakeStrength;
                float x = dir.x * t;
                float y = dir.y * t;

                offset = new Vector2(x, y);
                elapsed += Time.deltaTime;
                yield return null;
            }
            offset = Vector2.zero;
        }
    }
}
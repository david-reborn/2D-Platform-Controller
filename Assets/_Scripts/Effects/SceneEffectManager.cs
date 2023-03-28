

using Myd.Common;
using Myd.Platform.Core;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Myd.Platform
{
    /// <summary>
    /// 场景特效管理器
    /// </summary>
    public class SceneEffectManager: MonoBehaviour, IEffectControl
    { 
        public static SceneEffectManager Instance; 

        [SerializeField]
        private ParticleSystem vfxMoveDust;
        [SerializeField]
        private ParticleSystem vfxJumpDust;
        [SerializeField]
        private ParticleSystem vfxLandDust;
        [SerializeField]
        private ParticleSystem vfxDashLine;
        [SerializeField]
        private RippleEffect vfxRippleEffect;
        [SerializeField]
        private GameObject vfxSpeedRing;

        public void Awake()
        {
            Instance = this;
        }

        public void Reload()
        {
        }

        [SerializeField]
        private TrailSnapshot trailSnapshotPrefab;

        private TrailSnapshot[] snapshots = new TrailSnapshot[64];

        public void Add(SpriteRenderer renderer, int facing, Color color, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
        {
            Vector2 scale = renderer.transform.localScale;
            Add(renderer.transform.position, renderer.sprite, scale, facing, color, 2, duration, frozenUpdate, useRawDeltaTime);
        }

        private TrailSnapshot Add(Vector2 position, Sprite sprite, Vector2 scale, int facing, Color color,
                int depth, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
        {
            for (int index = 0; index < this.snapshots.Length; ++index)
            {
                if (this.snapshots[index] == null)
                {
                    TrailSnapshot snapshot = Instantiate(trailSnapshotPrefab, this.transform);
                    snapshot.Init(index, position, sprite, scale, color, duration, depth, frozenUpdate, useRawDeltaTime, () => { SetSnapshot(index, null); });
                    this.snapshots[index] = snapshot;
                    return snapshot;
                }
            }
            return null;
        }

        public void SetSnapshot(int index, TrailSnapshot snapshot)
        {
            this.snapshots[index] = snapshot;
        }

        public void RestAllEffect()
        {
            this.vfxMoveDust.Play();
            this.vfxJumpDust.Stop();
            this.vfxLandDust.Stop();
            this.vfxDashLine.Stop();
        }

        public void JumpDust(Vector3 position, Color color, Vector2 dir)
        {
            this.vfxJumpDust.transform.position = position;
            this.vfxJumpDust.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
            var main = this.vfxJumpDust.main;
            main.startColor = color;
            this.vfxJumpDust.Play();
        }

        public void DashLine(Vector3 position, Vector2 dir)
        {
            this.vfxDashLine.transform.position = position;
            this.vfxDashLine.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
            this.vfxDashLine.GetComponent<ParticleSystem>().Play();
        }

        public void Ripple(Vector3 position)
        {
            this.vfxRippleEffect.Ripple(position);
        }

        public void CameraShake(Vector2 dir)
        {
            Game.Instance.CameraShake(dir, 0.2f);
        }

        public void Freeze(float freezeTime)
        {
            Game.Instance.Freeze(freezeTime);
        }

        public void LandDust(Vector3 position, Color color)
        {
            this.vfxLandDust.transform.position = position;
            var main = this.vfxLandDust.main;
            main.startColor = color;
            this.vfxLandDust.Play();
        }

        public void SpeedRing(Vector3 position, Vector2 dir)
        {
            GameObject speedRing = Instantiate(vfxSpeedRing, this.transform);
            speedRing.transform.position = position;
            speedRing.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
            speedRing.transform.localScale = Vector3.zero;
            DOTween.Sequence().Append(speedRing.transform.DOScale(Vector3.one * 1.2f, 1.5f).SetEase(DG.Tweening.Ease.OutCubic)).Join(speedRing.GetComponent<SpriteRenderer>().DOFade(0, 1.5f)).AppendCallback(() =>
            {
                Destroy(speedRing);
            });
        }

    }

}

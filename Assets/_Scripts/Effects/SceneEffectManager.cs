

using Myd.Common;
using Myd.Platform.Core;
using System.Collections.Generic;
using UnityEngine;

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
        private ParticleSystem vfxDashFlux;
        [SerializeField]
        private RippleEffect vfxRippleEffect;
        [SerializeField]
        private DashImpulse vfxDashImpulse;

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
            this.vfxDashFlux.Stop();
        }

        public void JumpDust(Vector3 position)
        {
            this.vfxJumpDust.transform.position = position;
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
            this.vfxDashImpulse.Shake(dir);
        }

        public void LandDust(Vector3 position)
        {
            this.vfxLandDust.transform.position = position;
            this.vfxLandDust.Play();
        }

        public void DashFlux(Vector2 dir, bool play)
        {
            if (play)
            {
                this.vfxDashFlux.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
                this.vfxDashFlux.Play();
            }
            else
            {
                this.vfxDashFlux.Stop();
            }
        }
    }

}



using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class SceneEffectManager: MonoBehaviour
    {
        public static SceneEffectManager instance;
        [SerializeField]
        private TrailSnapshot trailSnapshotPrefab;


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

        private TrailSnapshot[] snapshots = new TrailSnapshot[64];

        public void Awake()
        {
            SceneEffectManager.instance = this;
        }

        //public void Add(PlayerController player, Color color, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
        //{
        //    Vector2 scale = player.Renderer.transform.localScale;
        //    //Vector2 scale = new Vector2(playerSprite.Scale.x * (int)player.Facing, playerSprite.Scale.y);
        //    Add(player.Renderer.transform.position, player.Renderer.sprite, scale, (int)player.Facing, color, 2, duration, frozenUpdate, useRawDeltaTime);
        //}

        private TrailSnapshot Add(Vector2 position, Sprite sprite, Vector2 scale, int facing, Color color,
                int depth, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
        {
            for (int index = 0; index < instance.snapshots.Length; ++index)
            {
                if (instance.snapshots[index] == null)
                {
                    TrailSnapshot snapshot = Instantiate(trailSnapshotPrefab, this.transform);
                    snapshot.Init(index, position, sprite, scale, color, duration, depth, frozenUpdate, useRawDeltaTime);
                    instance.snapshots[index] = snapshot;
                    return snapshot;
                }
            }
            return null;
        }

        public void SetSnapshot(int index, TrailSnapshot snapshot)
        {
            this.snapshots[index] = snapshot;
        }

        public void PlayMoveEffect(bool play, Color color)
        {
            if (play)
            {
                ParticleSystem.MainModule settings = this.vfxMoveDust.GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(color);
                this.vfxMoveDust.Play();
            }
            else
            {
                this.vfxMoveDust.Stop();
            }
        }

        public void UpdateMoveEffect(Vector3 position)
        {
            this.vfxMoveDust.transform.position = position;
            
        }

        public void PlayJumpEffect(Vector3 position)
        {
            this.vfxJumpDust.transform.position = position;//this.spriteRenderer.transform.position;
            this.vfxJumpDust.Play();
        }

        public void PlayLandEffect(Vector3 position)
        {
            this.vfxLandDust.transform.position = position;//this.spriteRenderer.transform.position;
            this.vfxLandDust.Play();
        }

        public void PlayDashEffect(Vector3 position, Vector2 dir)
        {
            this.vfxDashLine.transform.position = position;
            this.vfxDashLine.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
            this.vfxDashLine.Play();

            this.vfxRippleEffect.Ripple(position);

            this.vfxDashImpulse.Shake(dir);
        }

        public void PlayDashFluxEffect(Vector2 dir, bool play)
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

        public void RestAllEffect()
        {
            this.vfxMoveDust.Play();
            this.vfxJumpDust.Stop();
            this.vfxLandDust.Stop();
            this.vfxDashLine.Stop();
            this.vfxDashFlux.Stop();
        }
    }

}

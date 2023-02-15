

using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class SceneEffectManager: MonoBehaviour
    {
        public static SceneEffectManager instance;
        [SerializeField]
        private TrailSnapshot trailSnapshotPrefab;

        private TrailSnapshot[] snapshots = new TrailSnapshot[64];

        public void Awake()
        {
            SceneEffectManager.instance = this;
        }

        private void Init()
        {
        }

        public void Add(PlayerController player, Color color, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
        {
            Vector2 scale = player.Renderer.transform.localScale;
            //Vector2 scale = new Vector2(playerSprite.Scale.x * (int)player.Facing, playerSprite.Scale.y);
            Add(player.Renderer.transform.position, player.Renderer.sprite, scale, (int)player.Facing, color, 2, duration, frozenUpdate, useRawDeltaTime);
        }

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
    }

}

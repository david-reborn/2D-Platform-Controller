using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform
{
    public class Level : MonoBehaviour
    {
        public int levelId;

        public Bounds Bounds;

        public Vector2 StartPosition;

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(StartPosition, 0.5f);
        }
    }
}
